using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SerializableCallback.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SerializableCallback.Editor
{
    [CustomPropertyDrawer(typeof(TargetConstraintAttribute))]
    [CustomPropertyDrawer(typeof(SerializableCallbackBase), true)]
    public class SerializableCallbackDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Without this, you can't edit fields above the SerializedProperty
            property.serializedObject.ApplyModifiedProperties();

#if UNITY_2019_1_OR_NEWER
            GUI.Box(position, "");
#else
		GUI.Box(position, "", (GUIStyle)
			"flow overlay box");
#endif
            position.y += 4;
            
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            property.serializedObject.Update();
            EditorGUI.BeginProperty(position, label, property);
            // Draw label
            var pos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var targetRect = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);

            // Get target
            
            var targetProp = property.FindPropertyRelative("_target");
            if (attribute is TargetConstraintAttribute constraintAttribute)
            {
                var targetType = constraintAttribute.targetType;
                EditorGUI.ObjectField(targetRect, targetProp, targetType, GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(targetRect, targetProp, GUIContent.none);
            }

            var target = targetProp.objectReferenceValue;
            if (target == null)
            {
                var helpBoxRect = new Rect(position.x + 8, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width - 16, EditorGUIUtility.singleLineHeight);
                var msg = "Call not set. Execution will be slower.";
                EditorGUI.HelpBox(helpBoxRect, msg, MessageType.Warning);
            }
            else
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                // Set target type name
                var isStatic = target is MonoScript;
                var monoScript = target as MonoScript;
                var targetType = isStatic ? monoScript.GetClass() : target.GetType();
                var targetTypeNameProperty = property.FindPropertyRelative("_targetTypeName");
                targetTypeNameProperty.stringValue = targetType.AssemblyQualifiedName;
                
                // Get method name
                var methodProp = property.FindPropertyRelative("_methodName");
                string methodName = methodProp.stringValue;

                // Get args
                var argProps = property.FindPropertyRelative("_args");
                var argTypes = GetArgTypes(argProps);

                // Get dynamic
                var dynamicProp = property.FindPropertyRelative("_dynamic");
                var dynamic = dynamicProp.boolValue;

                // Get active method
                var activeMethod = GetMethod(targetType, methodName, argTypes);

                var methodlabel = new GUIContent("n/a");
                if (activeMethod != null)
                {
                    methodlabel = new GUIContent(PrettifyMethod(activeMethod));
                }
                else if (!string.IsNullOrEmpty(methodName))
                {
                    methodlabel = new GUIContent("Missing (" + PrettifyMethod(methodName, argTypes) + ")");
                }

                var methodRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

                // Method select button
                pos = EditorGUI.PrefixLabel(methodRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(dynamic ? "Method (dynamic)" : "Method"));
                if (EditorGUI.DropdownButton(pos, methodlabel, FocusType.Keyboard))
                {
                    MethodSelector(property, isStatic);
                }

                if (activeMethod != null && !dynamic)
                {
                    // Args
                    var activeParameters = activeMethod.GetParameters();
                    var argRect = new Rect(position.x, methodRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
                    var types = new string[argProps.arraySize];
                    for (int i = 0; i < types.Length; i++)
                    {
                        var argProp = argProps.FindPropertyRelative("Array.data[" + i + "]");
                        var argLabel = new GUIContent(ObjectNames.NicifyVariableName(activeParameters[i].Name));

                        EditorGUI.BeginChangeCheck();
                        switch ((Arg.ArgType) argProp.FindPropertyRelative("argType").enumValueIndex)
                        {
                            case Arg.ArgType.Bool:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("boolValue"), argLabel);
                                break;
                            case Arg.ArgType.Int:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("intValue"), argLabel);
                                break;
                            case Arg.ArgType.Float:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("floatValue"), argLabel);
                                break;
                            case Arg.ArgType.String:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("stringValue"), argLabel);
                                break;
                            case Arg.ArgType.Object:
                                var typeProp = argProp.FindPropertyRelative("_typeName");
                                var objProp = argProp.FindPropertyRelative("objectValue");

                                if (typeProp != null)
                                {
                                    var objType = Type.GetType(typeProp.stringValue, false);
                                    EditorGUI.BeginChangeCheck();
                                    var obj = objProp.objectReferenceValue;
                                    obj = EditorGUI.ObjectField(argRect, argLabel, obj, objType, true);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        objProp.objectReferenceValue = obj;
                                    }
                                }
                                else
                                {
                                    EditorGUI.PropertyField(argRect, objProp, argLabel);
                                }
                                break;
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.FindPropertyRelative("dirty").boolValue = true;
                        }
                        argRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                EditorGUI.indentLevel = indent;
            }

            // Set indent back to what it was
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }

        private class MenuItem
        {
            public GenericMenu.MenuFunction action;
            public string path;
            public GUIContent label;

            public MenuItem(string path, string name, GenericMenu.MenuFunction action)
            {
                this.action = action;
                this.label = new GUIContent(path + '/' + name);
                this.path = path;
            }
        }
        
        private void MethodSelector(SerializedProperty property, bool isStatic)
        {
            // Return type constraint
            Type returnType = null;
            // Arg type constraint
            var argTypes = Type.EmptyTypes;

            // Get return type and argument constraints
            var dummy = GetDummyFunction(property);
            var genericTypes = GetGenericCallbackType(dummy).GetGenericArguments();
            
            // SerializableEventBase is always void return type
            if (dummy is SerializableEventBase)
            {
                returnType = typeof(void);
                if (genericTypes.Length > 0)
                {
                    argTypes = new Type[genericTypes.Length];
                    Array.Copy(genericTypes, argTypes, genericTypes.Length);
                }
            }
            else
            {
                if (genericTypes != null && genericTypes.Length > 0)
                {
                    // The last generic argument is the return type
                    returnType = genericTypes[genericTypes.Length - 1];
                    if (genericTypes.Length > 1)
                    {
                        argTypes = new Type[genericTypes.Length - 1];
                        Array.Copy(genericTypes, argTypes, genericTypes.Length - 1);
                    }
                }
            }

            var targetProp = property.FindPropertyRelative("_target");

            var dynamicItems = new List<MenuItem>();
            var staticItems = new List<MenuItem>();

            var targets = new List<(Object, Type)>();
            var originalTarget = targetProp.objectReferenceValue;
            if (isStatic)
            {
                var monoScript = originalTarget as MonoScript;
                targets.Add((originalTarget, monoScript.GetClass()));
            }
            else
            {
                targets.Add((originalTarget, originalTarget.GetType()));
            }

            if (originalTarget is Component component)
            {
                var components = component.GetComponents<Component>();
                var newTargets = components.Select(c => ((Object)c, c.GetType()));
                // clear to avoid duplicating the component
                targets.Clear();
                targets.AddRange(newTargets);
            }
            else if (originalTarget is GameObject go)
            {
                var components = go.GetComponents<Component>();
                var newTargets = components.Select(c => ((Object)c, c.GetType()));
                targets.AddRange(newTargets);
            }
            
            var bindingFlags = BindingFlags.Public | BindingFlags.Static;
            if (!isStatic)
            {
                bindingFlags |= BindingFlags.Instance;
            }

            foreach (var (target, type) in targets)
            {
                var methods = type.GetMethods(bindingFlags);

                for (int i = 0; i < methods.Length; i++)
                {
                    var method = methods[i];

                    // Skip methods with wrong return type
                    if (returnType != null && method.ReturnType != returnType)
                    {
                        continue;
                    }
                    // Skip methods with null return type
                    // if (method.ReturnType == typeof(void)) continue;
                    // Skip generic methods
                    if (method.IsGenericMethod)
                    {
                        continue;
                    }

                    var parms = method.GetParameters().Select(x => x.ParameterType).ToArray();

                    // Skip methods with more than 4 args
                    if (parms.Length > 4)
                    {
                        continue;
                    }
                    
                    // Skip methods with unsupported args
                    var nonSerializableArgTypes = parms.Any(x => !Arg.IsSupported(x));

                    var menuItmePath = $"{type.Name}/{method.DeclaringType.Name}";
                    if (!nonSerializableArgTypes)
                    {
                        string methodPrettyName = PrettifyMethod(method);
                        staticItems.Add(new MenuItem(menuItmePath, methodPrettyName, () => SetMethod(property, target, method, false)));
                    }

                    // Skip methods with wrong constrained args
                    if (argTypes.Length == 0 || !Enumerable.SequenceEqual(argTypes, parms))
                    {
                        continue;
                    }

                    dynamicItems.Add(new MenuItem(menuItmePath, method.Name, () => SetMethod(property, target, method, true)));
                }
            }

            // Construct and display context menu
            var menu = new GenericMenu();
            if (dynamicItems.Count > 0)
            {
                var paths = dynamicItems.GroupBy(x => x.path).Select(x => x.First().path).ToArray();
                foreach (string path in paths)
                {
                    menu.AddItem(new GUIContent(path + "/Dynamic " + PrettifyTypes(argTypes)), false, null);
                }
                for (int i = 0; i < dynamicItems.Count; i++)
                {
                    menu.AddItem(dynamicItems[i].label, false, dynamicItems[i].action);
                }
                foreach (string path in paths)
                {
                    menu.AddItem(new GUIContent(path + "/  "), false, null);
                    menu.AddItem(new GUIContent(path + "/Static parameters"), false, null);
                }
            }
            for (int i = 0; i < staticItems.Count; i++)
            {
                menu.AddItem(staticItems[i].label, false, staticItems[i].action);
            }

            if (menu.GetItemCount() == 0)
            {
                menu.AddDisabledItem(new GUIContent("No methods with return type '" + GetTypeName(returnType) + "'"));
            }
            menu.ShowAsContext();
        }

        private static Type GetGenericCallbackType(SerializableCallbackBase dummy)
        {
            var type = dummy.GetType();
            var genericArgumentsCount = type.GetGenericArguments().Length;
            if (genericArgumentsCount > 0)
            {
                return type;
            }
            else
            {
                return type.BaseType;
            }
        }

        string PrettifyMethod(string methodName, Type[] parmTypes)
        {
            var parmnames = PrettifyTypes(parmTypes);
            return methodName + "(" + parmnames + ")";
        }

        string PrettifyMethod(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }
            var parms = methodInfo.GetParameters();
            var parmnames = PrettifyTypes(parms.Select(x => x.ParameterType).ToArray());
            return GetTypeName(methodInfo.ReturnParameter.ParameterType) + " " + methodInfo.Name + "(" + parmnames + ")";
        }

        string PrettifyTypes(Type[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            return string.Join(", ", types.Select(x => GetTypeName(x)).ToArray());
        }

        MethodInfo GetMethod(Type targetType, string methodName, Type[] types)
        {
            var activeMethod = targetType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, CallingConventions.Any, types, null);
            return activeMethod;
        }

        private Type[] GetArgTypes(SerializedProperty argsProp)
        {
            var types = new Type[argsProp.arraySize];
            for (int i = 0; i < argsProp.arraySize; i++)
            {
                var argProp = argsProp.GetArrayElementAtIndex(i);
                var typeNameProp = argProp.FindPropertyRelative("_typeName");
                if (typeNameProp != null)
                {
                    types[i] = Type.GetType(typeNameProp.stringValue, false);
                }

                if (types[i] == null)
                {
                    types[i] = Arg.RealType((Arg.ArgType) argProp.FindPropertyRelative("argType").enumValueIndex);
                }
            }
            return types;
        }

        private void SetMethod(SerializedProperty property, Object target, MethodInfo methodInfo, bool dynamic)
        {
            var targetProp = property.FindPropertyRelative("_target");
            targetProp.objectReferenceValue = target;
            var isStaticProperty = property.FindPropertyRelative("_isStatic");
            isStaticProperty.boolValue = methodInfo.IsStatic;
            var methodProp = property.FindPropertyRelative("_methodName");
            methodProp.stringValue = methodInfo.Name;
            var dynamicProp = property.FindPropertyRelative("_dynamic");
            dynamicProp.boolValue = dynamic;
            var argProp = property.FindPropertyRelative("_args");
            var parameters = methodInfo.GetParameters();
            argProp.arraySize = parameters.Length;
            for (int i = 0; i < parameters.Length; i++)
            {
                argProp.FindPropertyRelative("Array.data[" + i + "].argType").enumValueIndex = (int) Arg.FromRealType(parameters[i].ParameterType);
                argProp.FindPropertyRelative("Array.data[" + i + "]._typeName").stringValue = parameters[i].ParameterType.AssemblyQualifiedName;
            }
            property.FindPropertyRelative("dirty").boolValue = true;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        private static string GetTypeName(Type t)
        {
            if (t == typeof(int)) return "int";
            else if (t == typeof(float)) return "float";
            else if (t == typeof(string)) return "string";
            else if (t == typeof(bool)) return "bool";
            else if (t == typeof(void)) return "void";
            else return t.Name;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lineheight = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            var targetProp = property.FindPropertyRelative("_target");
            var argProps = property.FindPropertyRelative("_args");
            var dynamicProp = property.FindPropertyRelative("_dynamic");
            var height = lineheight + lineheight;
            if (targetProp.objectReferenceValue != null && !dynamicProp.boolValue)
            {
                height += argProps.arraySize * lineheight;
            }
            height += 8;
            return height;
        }

        private static SerializableCallbackBase GetDummyFunction(SerializedProperty prop)
        {
            string stringValue = prop.FindPropertyRelative("_typeName").stringValue;
            var type = Type.GetType(stringValue, false);
            SerializableCallbackBase result;
            if (type == null)
            {
                return null;
            }
            else
            {
                result = (Activator.CreateInstance(type) as SerializableCallbackBase);
            }
            return result;
        }
    }
}
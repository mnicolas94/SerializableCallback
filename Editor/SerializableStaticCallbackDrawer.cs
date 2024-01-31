using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using B83;
using SerializableCallback.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SerializableCallback.Editor
{
    [CustomPropertyDrawer(typeof(TargetConstraintAttribute))]
    [CustomPropertyDrawer(typeof(SerializableStaticCallbackBase), true)]
    public class SerializableStaticCallbackDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Without this, you can't edit fields above the SerializedProperty
            property.serializedObject.ApplyModifiedProperties();

            // Indent label
            // label.text = " " + label.text;

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
            Rect pos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            Rect targetRect = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);

            // Get target
            SerializedProperty targetProp = property.FindPropertyRelative("_targetType");
            var target = (SerializableMonoScript) targetProp.boxedValue;
            if (attribute is TargetConstraintAttribute constraintAttribute)
            {
                Type targetType = constraintAttribute.targetType;
                EditorGUI.ObjectField(targetRect, targetProp, targetType, GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(targetRect, targetProp, GUIContent.none);
            }

            if (target == null || target.Type == null)
            {
                Rect helpBoxRect = new Rect(position.x + 8, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width - 16, EditorGUIUtility.singleLineHeight);
                string msg = "Call not set. Execution will be slower.";
                EditorGUI.HelpBox(helpBoxRect, msg, MessageType.Warning);
            }
            else
            {
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                // Get method name
                SerializedProperty methodProp = property.FindPropertyRelative("_methodName");
                string methodName = methodProp.stringValue;

                // Get args
                SerializedProperty argProps = property.FindPropertyRelative("_args");
                Type[] argTypes = GetArgTypes(argProps);

                // Get dynamic
                SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
                bool dynamic = dynamicProp.boolValue;

                // Get active method
                MethodInfo activeMethod = GetMethod(target, methodName, argTypes);

                GUIContent methodlabel = new GUIContent("n/a");
                if (activeMethod != null)
                {
                    methodlabel = new GUIContent(PrettifyMethod(activeMethod));
                }
                else if (!string.IsNullOrEmpty(methodName))
                {
                    methodlabel = new GUIContent("Missing (" + PrettifyMethod(methodName, argTypes) + ")");
                }

                Rect methodRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

                // Method select button
                pos = EditorGUI.PrefixLabel(methodRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(dynamic ? "Method (dynamic)" : "Method"));
                if (EditorGUI.DropdownButton(pos, methodlabel, FocusType.Keyboard))
                {
                    MethodSelector(property);
                }

                if (activeMethod != null && !dynamic)
                {
                    // Args
                    ParameterInfo[] activeParameters = activeMethod.GetParameters();
                    Rect argRect = new Rect(position.x, methodRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
                    string[] types = new string[argProps.arraySize];
                    for (int i = 0; i < types.Length; i++)
                    {
                        SerializedProperty argProp = argProps.FindPropertyRelative("Array.data[" + i + "]");
                        GUIContent argLabel = new GUIContent(ObjectNames.NicifyVariableName(activeParameters[i].Name));

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
                                SerializedProperty typeProp = argProp.FindPropertyRelative("_typeName");
                                SerializedProperty objProp = argProp.FindPropertyRelative("objectValue");

                                if (typeProp != null)
                                {
                                    Type objType = Type.GetType(typeProp.stringValue, false);
                                    EditorGUI.BeginChangeCheck();
                                    Object obj = objProp.objectReferenceValue;
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
        
        private void MethodSelector(SerializedProperty property)
        {
            // Return type constraint
            Type returnType = null;
            // Arg type constraint
            Type[] argTypes = new Type[0];

            // Get return type and argument constraints
            SerializableStaticCallbackBase dummy = GetDummyFunction(property);
            Type[] genericTypes = GetGenericCallbackType(dummy).GetGenericArguments();
            if (genericTypes.Length > 0)
            {
                // The last generic argument is the return type
                returnType = genericTypes[genericTypes.Length - 1];
                if (genericTypes.Length > 1)
                {
                    argTypes = new Type[genericTypes.Length - 1];
                    Array.Copy(genericTypes, argTypes, genericTypes.Length - 1);
                }
            }

            SerializedProperty targetProp = property.FindPropertyRelative("_targetType");

            List<MenuItem> dynamicItems = new List<MenuItem>();
            List<MenuItem> staticItems = new List<MenuItem>();

            var target = (SerializableMonoScript) targetProp.boxedValue;
            MethodInfo[] methods = target.Type.GetMethods(BindingFlags.Public | BindingFlags.Static);

            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];

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

                Type[] parms = method.GetParameters().Select(x => x.ParameterType).ToArray();

                // Skip methods with more than 4 args
                if (parms.Length > 4)
                {
                    continue;
                }
                
                // Skip methods with unsupported args
                var nonSerializableArgTypes = parms.Any(x => !Arg.IsSupported(x));

                if (!nonSerializableArgTypes)
                {
                    string methodPrettyName = PrettifyMethod(methods[i]);
                    staticItems.Add(new MenuItem(target.Type.Name + "/" + methods[i].DeclaringType.Name, methodPrettyName, () => SetMethod(property, target, method, false)));
                }

                // Skip methods with wrong constrained args
                if (argTypes.Length == 0 || !Enumerable.SequenceEqual(argTypes, parms))
                {
                    continue;
                }

                dynamicItems.Add(new MenuItem(target.Type.Name + "/" + methods[i].DeclaringType.Name, methods[i].Name, () => SetMethod(property, target, method, true)));
            }

            // Construct and display context menu
            GenericMenu menu = new GenericMenu();
            if (dynamicItems.Count > 0)
            {
                string[] paths = dynamicItems.GroupBy(x => x.path).Select(x => x.First().path).ToArray();
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

        private static Type GetGenericCallbackType(SerializableStaticCallbackBase dummy)
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
            string parmnames = PrettifyTypes(parmTypes);
            return methodName + "(" + parmnames + ")";
        }

        string PrettifyMethod(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }
            ParameterInfo[] parms = methodInfo.GetParameters();
            string parmnames = PrettifyTypes(parms.Select(x => x.ParameterType).ToArray());
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

        MethodInfo GetMethod(SerializableMonoScript target, string methodName, Type[] types)
        {
            MethodInfo activeMethod = target.Type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Any, types, null);
            return activeMethod;
        }

        private Type[] GetArgTypes(SerializedProperty argsProp)
        {
            Type[] types = new Type[argsProp.arraySize];
            for (int i = 0; i < argsProp.arraySize; i++)
            {
                SerializedProperty argProp = argsProp.GetArrayElementAtIndex(i);
                SerializedProperty typeNameProp = argProp.FindPropertyRelative("_typeName");
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

        private void SetMethod(SerializedProperty property, SerializableMonoScript target, MethodInfo methodInfo, bool dynamic)
        {
            SerializedProperty targetProp = property.FindPropertyRelative("_targetType");
            targetProp.boxedValue = target;
            SerializedProperty methodProp = property.FindPropertyRelative("_methodName");
            methodProp.stringValue = methodInfo.Name;
            SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
            dynamicProp.boolValue = dynamic;
            SerializedProperty argProp = property.FindPropertyRelative("_args");
            ParameterInfo[] parameters = methodInfo.GetParameters();
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
            float lineheight = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            SerializedProperty targetProp = property.FindPropertyRelative("_targetType");
            SerializedProperty argProps = property.FindPropertyRelative("_args");
            SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
            float height = lineheight + lineheight;
            if (targetProp.boxedValue != null && !dynamicProp.boolValue)
            {
                height += argProps.arraySize * lineheight;
            }
            height += 8;
            return height;
        }

        private static SerializableStaticCallbackBase GetDummyFunction(SerializedProperty prop)
        {
            string stringValue = prop.FindPropertyRelative("_typeName").stringValue;
            Type type = Type.GetType(stringValue, false);
            SerializableStaticCallbackBase result;
            if (type == null)
            {
                return null;
            }
            else
            {
                result = (Activator.CreateInstance(type) as SerializableStaticCallbackBase);
            }
            return result;
        }
    }
}
using System;
using UnityEditor;
using UnityEngine;

namespace SerializableCallback.Editor
{
    [CustomPropertyDrawer(typeof(SerializableValueCallback<>))]
    public class SerializableValueCallbackDrawer : PropertyDrawer
    {
        private SerializedProperty _modeProperty;
        private SerializedProperty _valueProperty;
        private SerializedProperty _callbackProperty;
        
        private static GUIStyle _popupStyle;

        private Mode GetEnumMode => Enum.Parse<Mode>(_modeProperty.enumNames[_modeProperty.enumValueIndex]);
        private SerializedProperty ValidProperty => GetEnumMode == Mode.Value ? _valueProperty : _callbackProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _modeProperty = property.FindPropertyRelative(SerializableValueCallback<object>.Editor_PropertyMode);
            _valueProperty = property.FindPropertyRelative(SerializableValueCallback<object>.Editor_PropertyValue);
            _callbackProperty = property.FindPropertyRelative(SerializableValueCallback<object>.Editor_PropertyCallback);

            _popupStyle ??= new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
            {
                imagePosition = ImagePosition.ImageOnly
            };
            
            label = EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            // Calculate rect for mode selection button
            Rect buttonRect = new Rect(position);
            buttonRect.yMin += _popupStyle.margin.top;
            buttonRect.yMax = buttonRect.yMin + EditorGUIUtility.singleLineHeight;
            buttonRect.width = _popupStyle.fixedWidth + _popupStyle.margin.right;
            buttonRect.x = position.width - buttonRect.width;

            position.width -= buttonRect.width;

            var enumValueIndex = EditorGUI.Popup(
                buttonRect, _modeProperty.enumValueIndex, _modeProperty.enumDisplayNames, _popupStyle);
            _modeProperty.enumValueIndex = enumValueIndex;
            
            EditorGUI.PropertyField(position, ValidProperty, label);
            
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _modeProperty = property.FindPropertyRelative(SerializableValueCallback<object>.Editor_PropertyMode);
            _valueProperty = property.FindPropertyRelative(SerializableValueCallback<object>.Editor_PropertyValue);
            _callbackProperty = property.FindPropertyRelative(SerializableValueCallback<object>.Editor_PropertyCallback);
            
            return EditorGUI.GetPropertyHeight(ValidProperty, label);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Utilities.DateTime
{
    [CustomPropertyDrawer(typeof(DateStruct))]
    public class DateStructDrawer : PropertyDrawer
    {
        static readonly GUIContent LabelYear = new GUIContent("Y");
        static readonly GUIContent LabelMonth = new GUIContent("M");
        static readonly GUIContent LabelDay = new GUIContent("D");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float labelWidthOrigin = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 15f;

            // Calculate rects
            const float InputFieldWidth = 75f;
            const float InputFieldSpacing = 5f;
            var rectY = new Rect(position.x, position.y, InputFieldWidth, position.height);
            var rectM = new Rect(position.x + InputFieldWidth + InputFieldSpacing, position.y, InputFieldWidth, position.height);
            var rectD = new Rect(position.x + 2 * (InputFieldWidth + InputFieldSpacing), position.y, InputFieldWidth, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(rectY, property.FindPropertyRelative("year"), LabelYear);
            EditorGUI.PropertyField(rectM, property.FindPropertyRelative("month"), LabelMonth);
            EditorGUI.PropertyField(rectD, property.FindPropertyRelative("day"), LabelDay);

            EditorGUIUtility.labelWidth = labelWidthOrigin;

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
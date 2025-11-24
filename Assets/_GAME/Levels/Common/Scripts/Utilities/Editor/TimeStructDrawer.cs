using Codice.Client.BaseCommands;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Utilities.DateTime
{
    [CustomPropertyDrawer(typeof(TimeStruct))]
    public class TimeStructDrawer : PropertyDrawer
    {
        static readonly GUIContent LabelHour = new GUIContent("h");
        static readonly GUIContent LabelMinute = new GUIContent("m");
        static readonly GUIContent LabelSecond = new GUIContent("s");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var hour = property.FindPropertyRelative("hour");
            var minute = property.FindPropertyRelative("minute");
            var second = property.FindPropertyRelative("second");

            Rect contentPosition = EditorGUI.PrefixLabel(position, label);
            float thirdWidth = contentPosition.width / 3;

            Rect dayRect = new Rect(contentPosition.x, contentPosition.y, thirdWidth, contentPosition.height);
            Rect monthRect = new Rect(contentPosition.x + thirdWidth, contentPosition.y, thirdWidth, contentPosition.height);
            Rect yearRect = new Rect(contentPosition.x + 2 * thirdWidth, contentPosition.y, thirdWidth, contentPosition.height);

            EditorGUI.PropertyField(yearRect, second, LabelHour);
            EditorGUI.PropertyField(monthRect, minute, LabelMinute);
            EditorGUI.PropertyField(dayRect, hour, LabelSecond);

            EditorGUI.EndProperty();
        }
    }
}
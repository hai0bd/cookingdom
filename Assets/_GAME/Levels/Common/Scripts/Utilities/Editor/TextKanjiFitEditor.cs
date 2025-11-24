using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Utilities
{
    [CustomEditor(typeof(TextKanjiFit))]
    public class TextKanjiFitEditor : GraphicEditor
    {
        SerializedProperty m_Text;
        SerializedProperty m_FontData;
        SerializedProperty isKanji;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
            isKanji = serializedObject.FindProperty("isKanji");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            EditorGUILayout.PropertyField(isKanji);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(ValueCurveAttribute))]
public class ValueCurveDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // First get the attribute since it contains the range for the slider
        ValueCurveAttribute curveAttr = attribute as ValueCurveAttribute;

        AnimationCurve curve = property.animationCurveValue;
        if (curve != null && curve.length > 0)
        {
            if (curveAttr.isShowX && curveAttr.isShowY)
            {
                // show both
                EditorGUI.PropertyField(position, property, new GUIContent(string.Format("{0} ({1};{2}) - ({3};{4})", label.text, curve.keys[0].time, curve.keys[0].value, curve.keys[curve.length - 1].time, curve.keys[curve.length - 1].value)));
            }
            else if (curveAttr.isShowX)
            {
                // only show time
                EditorGUI.PropertyField(position, property, new GUIContent(string.Format("{0} [{1};{2}]", label.text, curve.keys[0].time, curve.keys[curve.length - 1].time)));
            }
            else if (curveAttr.isShowY)
            {
                // only show value
                EditorGUI.PropertyField(position, property, new GUIContent(string.Format("{0} [{1};{2}]", label.text, curve.keys[0].value, curve.keys[curve.length - 1].value)));
            }
            else
            {
                // do nothing
                EditorGUI.PropertyField(position, property, new GUIContent(label.text));
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, new GUIContent(label.text + " [?,?]"));
        }
    }
}
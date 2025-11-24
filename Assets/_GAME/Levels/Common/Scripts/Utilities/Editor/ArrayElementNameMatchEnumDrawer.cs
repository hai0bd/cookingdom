using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ArrayElementNameMatchEnumAttribute))]
public class ArrayElementNameMatchEnumDrawer : PropertyDrawer
{
    private string[] _enumNames = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        ArrayElementNameMatchEnumAttribute arrayAttribute = attribute as ArrayElementNameMatchEnumAttribute;
        if (_enumNames == null) _enumNames = System.Enum.GetNames(arrayAttribute.enumType);

        int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
        EditorGUI.PropertyField(position, property, new GUIContent((pos < _enumNames.Length) ? _enumNames[pos] : "_"));

        EditorGUI.EndProperty();
    }
}
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfEnumAttribute))]
public class ShowIfEnumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfEnumAttribute condition = (ShowIfEnumAttribute)attribute;

        SerializedProperty enumProp = property.serializedObject.FindProperty(condition.enumFieldName);

        if (enumProp != null && enumProp.enumValueIndex == condition.enumValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfEnumAttribute condition = (ShowIfEnumAttribute)attribute;

        SerializedProperty enumProp = property.serializedObject.FindProperty(condition.enumFieldName);

        if (enumProp != null && enumProp.enumValueIndex == condition.enumValue)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        else
        {
            return 0f;
        }
    }
}

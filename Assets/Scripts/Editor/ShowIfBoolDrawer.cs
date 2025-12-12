using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfBoolAttribute))]
public class ShowIfBoolDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfBoolAttribute condition = (ShowIfBoolAttribute)attribute;
        SerializedProperty boolProp = property.serializedObject.FindProperty(condition.boolFieldName);

        if (boolProp != null && boolProp.propertyType == SerializedPropertyType.Boolean)
        {
            if (boolProp.boolValue == condition.expectedValue)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Invalid bool reference");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfBoolAttribute condition = (ShowIfBoolAttribute)attribute;
        SerializedProperty boolProp = property.serializedObject.FindProperty(condition.boolFieldName);

        if (boolProp != null && boolProp.propertyType == SerializedPropertyType.Boolean)
        {
            if (boolProp.boolValue == condition.expectedValue)
                return EditorGUI.GetPropertyHeight(property, label, true);
            else
                return 0f;
        }
        else
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}

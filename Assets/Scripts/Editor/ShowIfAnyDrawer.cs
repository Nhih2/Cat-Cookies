using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfAnyBoolAttribute))]
public class ShowIfAnyBoolDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAnyBoolAttribute condition = (ShowIfAnyBoolAttribute)attribute;
        if (ShouldShow(property, condition))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAnyBoolAttribute condition = (ShowIfAnyBoolAttribute)attribute;
        if (ShouldShow(property, condition))
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        return 0f;
    }

    private bool ShouldShow(SerializedProperty property, ShowIfAnyBoolAttribute condition)
    {
        foreach (var boolName in condition.boolFieldNames)
        {
            SerializedProperty boolProp = property.serializedObject.FindProperty(boolName);
            if (boolProp != null && boolProp.propertyType == SerializedPropertyType.Boolean)
            {
                if (boolProp.boolValue == condition.expectedValue)
                    return true; // found one that matches
            }
        }
        return false;
    }
}

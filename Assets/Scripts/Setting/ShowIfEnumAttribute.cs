using UnityEngine;

public class ShowIfEnumAttribute : PropertyAttribute
{
    public string enumFieldName;
    public int enumValue;

    public ShowIfEnumAttribute(string enumFieldName, int enumValue)
    {
        this.enumFieldName = enumFieldName;
        this.enumValue = enumValue;
    }
}

public class ShowIfBoolAttribute : PropertyAttribute
{
    public string boolFieldName;
    public bool expectedValue;

    public ShowIfBoolAttribute(string boolFieldName, bool expectedValue = true)
    {
        this.boolFieldName = boolFieldName;
        this.expectedValue = expectedValue;
    }
}

public class ShowIfAnyBoolAttribute : PropertyAttribute
{
    public string[] boolFieldNames;
    public bool expectedValue;

    public ShowIfAnyBoolAttribute(bool expectedValue = true, params string[] boolFieldNames)
    {
        this.boolFieldNames = boolFieldNames;
        this.expectedValue = expectedValue;
    }
}
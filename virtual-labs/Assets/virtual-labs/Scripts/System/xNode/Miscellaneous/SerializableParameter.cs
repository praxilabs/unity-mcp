using System;
using UnityEngine;

[Serializable]
public class SerializableParameter
{
    public int intValue;
    public float floatValue;
    public string stringValue;
    public bool boolValue;
    public Vector3 vector3Value;
    public UnityEngine.Object objectValue;

    public string enumTypeName;
    public int enumValueIndex;

    public string typeName = "";

    public object GetValue()
    {
        if (string.IsNullOrEmpty(typeName))
            return null;

        if (typeName.Contains("Int"))
            return intValue;

        else if (typeName.Contains("Single"))
            return floatValue;
        else if (typeName.Contains("String"))
            return stringValue;
        else if (typeName.Contains("Boolean"))
            return boolValue;
        else if (typeName.Contains("Vector3"))
            return vector3Value;
        else if (typeName.Contains("Enum"))
        {
            Type enumType = Type.GetType(enumTypeName);
            Array enumValues = Enum.GetValues(enumType);

            return (Enum)enumValues.GetValue(enumValueIndex);
        }

        return objectValue;
    }
    public void SetValue(object value)
    {
        if (value == null) return;

        if (!typeName.Contains("Enum"))
            typeName = value.GetType().FullName;

        if (typeName.Contains("Int"))
            intValue = (int)value;
        else if (typeName.Contains("Single"))
            floatValue = (float)value;
        else if (typeName.Contains("String"))
            stringValue = (string)value;
        else if (typeName.Contains("Boolean"))
            boolValue = (bool)value;
        else if (typeName.Contains("Vector3"))
            vector3Value = (Vector3)value;
        else if (typeName.Contains("Enum"))
            SetEnumValue((Enum)value);
        else
            objectValue = value as UnityEngine.Object;
    }

    public void SetEnumValue(Enum newValue)
    {
        enumTypeName = newValue.GetType().AssemblyQualifiedName;
        enumValueIndex = Convert.ToInt32(newValue);
    }
}
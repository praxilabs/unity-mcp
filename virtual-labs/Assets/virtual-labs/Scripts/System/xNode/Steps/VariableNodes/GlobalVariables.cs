using UnityEngine;

[System.Serializable]
public class GlobalVariables
{
    public string name;
    [HideInInspector] public GlobalVariableTypes previousType;
    public GlobalVariableTypes type;

    public int intValue;
    public float floatValue;
    public string stringValue;
    public bool boolValue;

    public Color colorValue;
    public Vector2 vector2Value;
    public Vector3 vector3Value;

    public UnityEngine.Object objectValue;

    public object Value
    {
        get
        {
            switch (type)
            {
                case GlobalVariableTypes.Int: return intValue;
                case GlobalVariableTypes.Float: return floatValue;
                case GlobalVariableTypes.String: return stringValue;
                case GlobalVariableTypes.Bool: return boolValue;
                case GlobalVariableTypes.Color: return colorValue;
                case GlobalVariableTypes.Vector2: return vector2Value;
                case GlobalVariableTypes.Vector3: return vector3Value;
                case GlobalVariableTypes.Object: return objectValue;
                default: return null;
            }
        }
        set
        {
            switch (type)
            {
                case GlobalVariableTypes.Int: intValue = (int)value; break;
                case GlobalVariableTypes.Float: floatValue = (float)value; break;
                case GlobalVariableTypes.String: stringValue = (string)value; break;
                case GlobalVariableTypes.Bool: boolValue = (bool)value; break;
                case GlobalVariableTypes.Color: colorValue = (Color)value; break;
                case GlobalVariableTypes.Vector2: vector2Value = (Vector2)value; break;
                case GlobalVariableTypes.Vector3: vector3Value = (Vector3)value; break;
                case GlobalVariableTypes.Object: objectValue = (UnityEngine.Object)value; break;
                default: break;
            }
        }
    }
}

public enum GlobalVariableTypes
{
    None, Int, Float, String, Bool, Color, Vector2, Vector3, Object
}
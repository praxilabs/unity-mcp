using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GlobalVariables))]
public class GlobalVariablesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        DrawNameProperty(position, property);
        GlobalVariableTypes currentType = DrawTypeProperty( position, property);

        CheckTypeChange(property, currentType);
        DrawValueProperty(currentType, position, property);

        EditorGUI.EndProperty();
    }

    private void DrawNameProperty(Rect position, SerializedProperty property)
    {
        SerializedProperty nameProperty = property.FindPropertyRelative("name");

        Rect nameRect = new Rect(position.x, position.y, position.width / 1.5f, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(nameRect, nameProperty);
    }

    private GlobalVariableTypes DrawTypeProperty(  Rect position, SerializedProperty property)
    {
        SerializedProperty typeProperty = property.FindPropertyRelative("type");
        GlobalVariableTypes currentType = (GlobalVariableTypes)typeProperty.enumValueIndex;

        Rect typeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width / 4f, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(typeRect, typeProperty, GUIContent.none);

        return currentType;
    }

    private void DrawValueProperty(GlobalVariableTypes currentType, Rect position, SerializedProperty property)
    {
        Rect valueRect = new Rect(position.x + position.width / 4f + 5, position.y + EditorGUIUtility.singleLineHeight + 2, position.width / 1.35f, EditorGUIUtility.singleLineHeight);

        UpdateTypePropertyField(valueRect, property, currentType);
    }

    /// <summary>
    /// Check if type has changed in inspector
    /// </summary>
    private void CheckTypeChange(SerializedProperty property, GlobalVariableTypes currentType)
    {
        SerializedProperty previousTypeProperty = property.FindPropertyRelative("previousType");
        GlobalVariableTypes previousType = (GlobalVariableTypes)previousTypeProperty.enumValueIndex;

        if (currentType != previousType)
            previousTypeProperty.enumValueIndex = (int)currentType;
    }

    /// <summary>
    /// Update displayed property field according to selected type in inspector
    /// </summary>
    private void UpdateTypePropertyField(Rect position, SerializedProperty property,GlobalVariableTypes currentType)
    {
        switch (currentType)
        {
            case GlobalVariableTypes.Int:
                SerializedProperty intValueProperty = property.FindPropertyRelative("intValue");
                intValueProperty.intValue = EditorGUI.IntField(position,  intValueProperty.intValue);
                break;
            case GlobalVariableTypes.Float:
                SerializedProperty floatValueProperty = property.FindPropertyRelative("floatValue");
                floatValueProperty.floatValue = EditorGUI.FloatField(position,   floatValueProperty.floatValue);
                break;
            case GlobalVariableTypes.String:
                SerializedProperty stringValueProperty = property.FindPropertyRelative("stringValue");
                stringValueProperty.stringValue = EditorGUI.TextField(position, stringValueProperty.stringValue);
                break;
            case GlobalVariableTypes.Bool:
                SerializedProperty boolValueProperty = property.FindPropertyRelative("boolValue");
                boolValueProperty.boolValue = EditorGUI.Toggle(position, boolValueProperty.boolValue);
                break;
            case GlobalVariableTypes.Color:
                SerializedProperty colorValueProperty = property.FindPropertyRelative("colorValue");
                colorValueProperty.colorValue = EditorGUI.ColorField(position, colorValueProperty.colorValue);
                break;
            case GlobalVariableTypes.Vector2:
                SerializedProperty vector2ValueProperty = property.FindPropertyRelative("vector2Value");
                vector2ValueProperty.vector2Value = EditorGUI.Vector2Field(position, GUIContent.none, vector2ValueProperty.vector2Value);
                break;
            case GlobalVariableTypes.Vector3:
                SerializedProperty vector3ValueProperty = property.FindPropertyRelative("vector3Value");
                vector3ValueProperty.vector3Value = EditorGUI.Vector3Field(position, GUIContent.none, vector3ValueProperty.vector3Value);
                break;
            case GlobalVariableTypes.Object:
                SerializedProperty objectValueProperty = property.FindPropertyRelative("objectValue");
                objectValueProperty.objectReferenceValue = EditorGUI.ObjectField(position, objectValueProperty.objectReferenceValue, typeof(UnityEngine.Object), true);
                break;
            default:
                EditorGUI.LabelField(position, "Select a type to enter a value.");
                break;
        }
    }

    /// <summary>
    /// Set new field height singleLineHeight * 2 + 4
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + 4;
    }
}
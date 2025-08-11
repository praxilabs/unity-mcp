using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TimeSpanWrapper))]
public class TimeSpanDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty hoursProp = property.FindPropertyRelative("hours");
        SerializedProperty minutesProp = property.FindPropertyRelative("minutes");
        SerializedProperty secondsProp = property.FindPropertyRelative("seconds");

        EditorGUI.BeginProperty(position, label, property);

        // Create a label with the name of the TimeSpan field
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label);

        // Create Rects for the hours, minutes, and seconds fields
        float fieldWidth = position.width / 3;
        Rect hoursRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, fieldWidth - 4, EditorGUIUtility.singleLineHeight);
        Rect minutesRect = new Rect(position.x + fieldWidth, position.y + EditorGUIUtility.singleLineHeight, fieldWidth - 4, EditorGUIUtility.singleLineHeight);
        Rect secondsRect = new Rect(position.x + 2 * fieldWidth, position.y + EditorGUIUtility.singleLineHeight, fieldWidth - 4, EditorGUIUtility.singleLineHeight);

        // Display fields for hours, minutes, and seconds
        hoursProp.intValue = EditorGUI.IntField(hoursRect, "Hr", hoursProp.intValue);
        minutesProp.intValue = EditorGUI.IntField(minutesRect, "Min", minutesProp.intValue);
        secondsProp.intValue = EditorGUI.IntField(secondsRect, "Sec", secondsProp.intValue);

        // Ensure values stay within valid ranges
        hoursProp.intValue = Mathf.Clamp(hoursProp.intValue, 0, 23);
        minutesProp.intValue = Mathf.Clamp(minutesProp.intValue, 0, 59);
        secondsProp.intValue = Mathf.Clamp(secondsProp.intValue, 0, 59);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }
}
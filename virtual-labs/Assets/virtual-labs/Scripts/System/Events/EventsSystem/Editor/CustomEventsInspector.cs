using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventScriptCreator))]
public class CustomEventsInspector : Editor
{
    private string _editScriptButtonName;

    private EventScriptCreator _eventsScriptable;

    private void OnEnable()
    {
        _eventsScriptable = (EventScriptCreator)target;
    }

    public override void OnInspectorGUI()
    {
        if (((EventScriptCreator)target).scriptAlreadyCreated)
            _editScriptButtonName = "Update";
        else
            _editScriptButtonName = "Create";

        base.OnInspectorGUI();

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.normal.textColor = Color.green; // Change the text color
        buttonStyle.fontSize = 13;
        if (GUILayout.Button("Add Events List ", buttonStyle))
        {
            _eventsScriptable.AddEventTypeToTheList();
        }

        GUILayout.Space(EditorGUIUtility.singleLineHeight);
        GUIStyle buttonStyle2 = new GUIStyle(GUI.skin.button);
        buttonStyle2.normal.textColor = Color.green; // Change the text color
        buttonStyle2.fontSize = 15;

        if (GUILayout.Button(_editScriptButtonName, buttonStyle2))
        {
            if (target.name.Contains(" "))
            {
                Debug.LogError("Script name can't have spaces");
                return;
            }
            if (int.TryParse(target.name[0].ToString(), out int num))
            {
                Debug.LogError("Script name can't start with number");
                return;
            }
            _eventsScriptable.UpdateEventScript();
        }
    }

}
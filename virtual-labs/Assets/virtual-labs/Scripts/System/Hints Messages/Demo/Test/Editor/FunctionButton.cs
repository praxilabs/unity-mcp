
using Praxilabs.UIs;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SampleHintMessage))]
public class FunctionButton : Editor
{
    /*
    private int step = 1;
    private StepsStateType state = StepsStateType.Upcoming;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SampleHintMessage myScript = (SampleHintMessage)target;


        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Custom Editor Section", EditorStyles.boldLabel);

        EditorGUILayout.Space(); // Add space after the header
        
        step = EditorGUILayout.IntField("Editor Step Test", step);
        
        state = (StepsStateType)EditorGUILayout.EnumPopup("State ", state);
        
        if (GUILayout.Button("Change Step State"))
        {
            myScript.UpdateStepState(step,state);
        }
    }*/
}

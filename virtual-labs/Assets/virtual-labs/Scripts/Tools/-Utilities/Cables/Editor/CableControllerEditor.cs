// using Praxilabs.Utility.Editor;
// using UnityEditor;
// using UnityEngine;

// [CustomEditor(typeof(CableController))]
// public class CableControllerEditor : Editor
// {
//     private CableController _cableController;

//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();

//         _cableController = (CableController)target;

//         GeneratePlugs();
//         GenerateSplineButton();
//     }

//     private void GeneratePlugs()
//     {
//         GUILayout.BeginHorizontal();
//         GUILayout.FlexibleSpace();
//         if (GUILayout.Button("Update Head", ButtonStyle.Style(100, 25, "#ECB176", false, "#000000")))
//         {
//             _cableController.UpdateHeadPlug();
//             EditorUtility.SetDirty(_cableController);
//         } 
//         if (GUILayout.Button("Update Tail", ButtonStyle.Style(100, 25, "#ECB176", false, "#000000")))
//         {
//             _cableController.GenerateTailPlug();
//             EditorUtility.SetDirty(_cableController);
//         }
//         GUILayout.FlexibleSpace();
//         GUILayout.EndHorizontal();
//     }

//     private void GenerateSplineButton()
//     {
//         GUILayout.BeginHorizontal();
//         GUILayout.FlexibleSpace();
//         if (GUILayout.Button("Generate Spline", ButtonStyle.Style(200, 25, "#6F4E37", false, "#FFFFFFFF")))
//         {
//             _cableController.GenerateSpline();
//             EditorUtility.SetDirty(_cableController);
//         }
//         GUILayout.FlexibleSpace();
//         GUILayout.EndHorizontal();
//     }
// }
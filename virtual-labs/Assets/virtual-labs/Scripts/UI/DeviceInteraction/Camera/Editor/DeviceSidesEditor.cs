// using Praxilabs.Utility.Editor;
// using UnityEditor;
// using UnityEngine;

// [CustomEditor(typeof(DeviceSideController))]
// public class DeviceSidesEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();

//         DeviceSideController device = (DeviceSideController)target;

//         if (GUILayout.Button("Instantiate Around Device", ButtonStyle.Style(null, 20, "#705C53", false, "#FFFFFF")))
//         {
//             device.InstantiateAroundDevice();
//         }
//     }
// }
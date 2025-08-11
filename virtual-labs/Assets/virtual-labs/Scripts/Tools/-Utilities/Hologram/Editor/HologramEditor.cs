using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Hologram))]
public class HologramEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Collect Renderers From Children"))
        {
            Hologram hologram = (Hologram)target;
            hologram.CollectRenderersFromChildren(true);
            EditorUtility.SetDirty(hologram);
        }
    }
}



using UnityEngine;
using XNodeEditor;

namespace Praxilabs.xNode.Editor
{
    public static class ResetGraph
    {
        private static Vector2 _previousGridPosition;
        private static float _previousZoom;

        public static void ResetGraphView()
        {
            if (NodeEditorWindow.current.panOffset != _previousGridPosition && NodeEditorWindow.current.panOffset == Vector2.zero)
            {
                NodeEditorWindow.current.panOffset = _previousGridPosition;
                NodeEditorWindow.current.zoom = _previousZoom;
            }
            else if (_previousGridPosition != NodeEditorWindow.current.panOffset)
            {
                StorePreviousGridPosition();
            }
        }

        public static void StorePreviousGridPosition()
        {
            _previousGridPosition = NodeEditorWindow.current.panOffset;
            _previousZoom = NodeEditorWindow.current.zoom;
        }
    }
}
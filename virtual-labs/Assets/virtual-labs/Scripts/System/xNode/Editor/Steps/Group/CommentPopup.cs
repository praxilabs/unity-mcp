using UnityEditor;
using UnityEngine;
using XNode.NodeGroups;

namespace XNodeEditor
{
    public class CommentPopup : EditorWindow
    {
        private NodeGroup group { get { return _group != null ? _group : _group = target as NodeGroup; } }
        private NodeGroup _group;

        private const string inputControlName = "commentInput";

        public static CommentPopup current { get; private set; }
        public Object target;
        public string input;

        private bool firstFrame = true;

        public static CommentPopup Show(NodeGroup target)
        {
            if (current != null) current.Close();
            current = EditorWindow.GetWindow<CommentPopup>(true, "Enter Comment", true);
            current.target = target;
            current.input = target.comment;
            current.minSize = new Vector2(250, 88);
            current.maxSize = new Vector2(250, 88);
            current.UpdatePositionToMouse();
            return current;
        }

        private void UpdatePositionToMouse()
        {
            if (Event.current == null) return;
            Vector3 mousePoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            position = new Rect(mousePoint.x - position.width * 0.5f, mousePoint.y - 10, position.width, position.height);
        }

        private void OnLostFocus()
        {
            Close();
        }

        private void OnGUI()
        {
            SetFirstFrame();
            SetNextControlName();
            ListenToPressedEvent();
        }

        private void SetFirstFrame()
        {
            if (!firstFrame) return;

            UpdatePositionToMouse();
            firstFrame = false;
        }

        private void SetNextControlName()
        {
            GUI.SetNextControlName(inputControlName);
            input = input ?? string.Empty;
            input = GUILayout.TextArea(input, 130, GUILayout.ExpandHeight(true));
            EditorGUI.FocusTextInControl(inputControlName);
        }

        private void ListenToPressedEvent()
        {
            Event e = Event.current;

            if (GUILayout.Button("Apply") || (e.isKey && e.keyCode == KeyCode.Return))
            {
                group.comment = input;
                Close();
                target.TriggerOnValidate();
            }

            if (e.isKey && e.keyCode == KeyCode.Escape)
                Close();
        }

        private void OnDestroy()
        {
            EditorGUIUtility.editingTextField = false;
        }
    }
}
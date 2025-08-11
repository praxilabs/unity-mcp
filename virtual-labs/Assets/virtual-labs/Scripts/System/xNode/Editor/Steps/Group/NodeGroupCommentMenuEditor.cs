using UnityEditor;
using UnityEngine;
using XNode.NodeGroups;

namespace XNodeEditor.NodeGroups
{
    public class NodeGroupCommentMenuEditor
    {
        private static GUIStyle _nodeHeaderStyle;

        static NodeGroupCommentMenuEditor()
        {
            _nodeHeaderStyle = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                fontSize = 16,
                normal = { textColor = Color.cyan }
            };
        }

        public void SetDisplayComment(NodeGroup group)
        {
            GUILayout.Label(group.comment, _nodeHeaderStyle, GUILayout.Height(30));
        }

        public void AddCommentToContextMenu(GenericMenu menu, NodeGroup group)
        {
            menu.AddItem(new GUIContent("Add Comment"), false, () => { AddCommentToSelectedNode(group); });
        }

        public void AddCommentToSelectedNode(NodeGroup group)
        {
            if (Selection.objects.Length == 1 && Selection.activeObject is XNode.Node)
            {
                CommentPopup.Show(group);
            }
        }
    }
}
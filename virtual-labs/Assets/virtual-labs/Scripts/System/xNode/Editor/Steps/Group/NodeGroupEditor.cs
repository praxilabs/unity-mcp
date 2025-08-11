using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNode.NodeGroups;

namespace XNodeEditor.NodeGroups
{
    [CustomNodeEditor(typeof(NodeGroup))]
    public class NodeGroupEditor : NodeEditor
    {
        private NodeGroup group { get { return _group != null ? _group : _group = target as NodeGroup; } }
        private NodeGroup _group;
        public static Texture2D corner { get { return _corner != null ? _corner : _corner = Resources.Load<Texture2D>("xnode_corner"); } }
        private static Texture2D _corner;

        private Vector2 _size;
        private Rect _headerRect;
        private bool _isDragging;
        private bool _isSelectingHeader;

        private NodeGroupCommentMenuEditor _commentMenu = new NodeGroupCommentMenuEditor();

        public override void OnHeaderGUI()
        {
            _commentMenu.SetDisplayComment(group);
            _headerRect = GUILayoutUtility.GetLastRect();

            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (_headerRect.Contains(e.mousePosition))
                    {
                        _isSelectingHeader = true;
                        GetSelection();
                    }
                    break;
            }
        }

        public override void OnBodyGUI(XNode.Node currentNode)
        {
            if (!group.isGroup) return;
            Rect groupRect = GUILayoutUtility.GetRect(group.width, group.height);

            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDrag:
                    HandleMouseDrag(e);
                    break;
                case EventType.MouseDown:
                    if (e.button == 0 && groupRect.Contains(e.mousePosition))
                    {
                        _isSelectingHeader = false;
                        HandleMouseDown(e);
                    }
                    else
                        HandleMouseDownDrag(e);
                    break;
                case EventType.MouseUp:
                    HandleMouseUp();
                    break;
                case EventType.Repaint:
                    HandleRepaint();
                    break;
            }

            DrawGroupCollapseButton();

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            GUI.DrawTexture(new Rect(group.width - 34, group.height + EditorGUIUtility.singleLineHeight + 66, 24, 24), corner);
        }

        private void DrawGroupCollapseButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            float groupButtonStyleWidth = group.width - (group.width * 0.2f);
            if (GUILayout.Button(group.groupCollapsed ? "Expand" : "Collapse", StyleHelperxNode.Style(groupButtonStyleWidth, 50f, "#202020", false, "#FFFFFF")))
            {
                OnCollapseButtonClick();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            _commentMenu.AddCommentToContextMenu(menu, group);
            base.AddContextMenuItems(menu);
        }

        public override int GetWidth()
        {
            return group.width;
        }

        public override Color GetTint()
        {
            return group.color;
        }

        public static void AddMouseRect(Rect rect)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeUpLeft);
        }

        /// <summary>
        /// GetSelection() is a function that handles selection of nodes and reroutes within a group. 
        /// If the Selection contains the current target, it gets the current selection and adds the 
        /// nodes within the group to it. If the group is not collapsed, it also adds the reroutes within the group to the selection. 
        /// It then ensures that the group layer is lower than the nodes inside it, and sets the Selection.objects to the new selection. 
        /// The function uses various checks to ensure that only the nodes and reroutes within the group are selected.
        /// </summary>
        private void GetSelection()
        {
            if (!Selection.Contains(target)) return;

            List<UnityEngine.Object> selection = Selection.objects.ToList();
            // Select Nodes
            if (group.groupCollapsed)
                selection.AddRange(group.nodes);
            else
            {
                group.nodes = group.GetNodes();
                selection.AddRange(group.nodes);
            }

            EnsureGroupLayerIsLowerThanNodesInsideIt();

            // Select Reroutes
            foreach (XNode.Node node in target.graph.nodes)
            {
                if (node != null && !group.groupCollapsed)
                {
                    foreach (NodePort port in node.Ports)
                    {
                        for (int i = 0; i < port.ConnectionCount; i++)
                        {
                            List<Vector2> reroutes = port.GetReroutePoints(i);
                            for (int k = 0; k < reroutes.Count; k++)
                            {
                                Vector2 p = reroutes[k];
                                if (p.x < group.position.x) continue;
                                if (p.y < group.position.y) continue;
                                if (p.x > group.position.x + group.width) continue;
                                if (p.y > group.position.y + group.height + 30) continue;
                                if (NodeEditorWindow.current.selectedReroutes.Any(x => x.port == port && x.connectionIndex == i && x.pointIndex == k)) continue;
                                NodeEditorWindow.current.selectedReroutes.Add(
                                    new Internal.RerouteReference(port, i, k)
                                );
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            Selection.objects = selection.Distinct().ToArray();
        }

        /// <summary>
        /// Move node group to the bottom of the nodes list in graph so that nodes on the group can be dragged and clicked
        /// </summary>
        private void EnsureGroupLayerIsLowerThanNodesInsideIt()
        {
            for (int i = 0; i < group.nodes.Count; i++)
            {
                if (group.graph.nodes.IndexOf(group.nodes[i]) < group.graph.nodes.IndexOf(group))
                {
                    group.graph.nodes.Remove(group);
                    group.graph.nodes.Insert(0, group);
                    break;
                }
            }
        }

        /// <summary>
        /// This function is a callback for the collapse button click event. It toggles the collapsed state of the group and updates the size and collapsed state of the nodes in the group accordingly.
        /// </summary>
        private void OnCollapseButtonClick()
        {
            if (!group.groupCollapsed)
                group.nodes = group.GetNodes();

            for (int i = 0; i < group.nodes.Count; i++)
            {
                //NodeEditor nodeEditor = GetEditor(group.nodes[i], NodeEditorWindow.current);
                StepNode currentNode = group.nodes[i] as StepNode;
                if (currentNode != null)
                {
                    if (!group.groupCollapsed)
                    {
                        group.nodes[i].groupCollapsed = true;
                        //nodeEditor.SetWidth(0);
                        currentNode.width = 0;
                    }
                    else
                    {
                        group.nodes[i].groupCollapsed = false;
                        //nodeEditor.SetWidth(nodeEditor.GetWidthOriginal());
                        currentNode.width = currentNode.originalWidth;
                    }
                }
            }
            if (!group.groupCollapsed)
            {
                group.lastWidth = group.width;
                group.lastheight = group.height;
                group.width = group.collapsedWidth;
                group.height = group.collapsedHeight;
            }
            else
            {
                group.width = group.lastWidth;
                group.height = group.lastheight;
            }
            group.groupCollapsed = !group.groupCollapsed;
        }

        /// <summary>
        /// This function handles the mouse drag event for a group. 
        /// If a group is being dragged (isDragging is true), it updates the width and height of the group based on the position of the mouse cursor.
        /// It also repaints the NodeEditorWindow to reflect the changes made to the group.
        /// </summary>
        /// <param name="e">Unity GUI event</param>
        private void HandleMouseDrag(Event e)
        {
            if (_isDragging)
            {
                group.width = Mathf.Max(200, (int)e.mousePosition.x + 16);
                group.height = Mathf.Max(100, (int)e.mousePosition.y - 34);
                NodeEditorWindow.current.Repaint();
            }
        }

        /// <summary>
        /// Handles selecting group
        /// </summary>
        private void HandleMouseDown(Event e)
        {
            XNode.Node clickedNode = Selection.activeObject as XNode.Node;

            if (clickedNode != null && clickedNode != group) return;

            // If no node was clicked, select the group
            Selection.activeObject = group;
            e.Use();
        }

        /// <summary>
        /// This function handles the mouse down event for a group. If the left button is clicked (e.button == 0), 
        /// it checks if the mouse click is within the lower-right corner of the group, and sets isDragging to true if it is.
        /// </summary>
        /// <param name="e">Unity GUI event</param>
        private void HandleMouseDownDrag(Event e)
        {
            if (e.button != 0) return;

            if (NodeEditorWindow.current.nodeSizes.TryGetValue(target, out _size))
            {
                Rect lowerRight = new Rect(_size.x - 34, _size.y - 34, 30, 30);
                if (lowerRight.Contains(e.mousePosition))
                {
                    _isDragging = true;
                }
            }
        }

        /// <summary>
        /// This function handles the mouse up event for a group. It sets isDragging to false and calls GetSelection() to get the current selection.
        /// </summary>
        private void HandleMouseUp()
        {
            _isDragging = false;

            if (_isSelectingHeader)
                GetSelection();
        }

        /// <summary>
        /// This function handles the repaint event for a group. 
        /// It gets the size of the group and calculates the position of the lower-right corner of the group. 
        /// It then converts the position to a window position using NodeEditorWindow.current.GridToWindowRect, and adds a mouse rect at the position using AddMouseRect. 
        /// This is done in the onLateGUI delegate of the NodeEditorWindow.
        /// </summary>
        private void HandleRepaint()
        {
            if (NodeEditorWindow.current.nodeSizes.TryGetValue(target, out _size))
            {
                Rect lowerRight = new Rect(target.position, new Vector2(30, 30));
                lowerRight.y += _size.y - 34;
                lowerRight.x += _size.x - 34;
                lowerRight = NodeEditorWindow.current.GridToWindowRect(lowerRight);
                NodeEditorWindow.current.onLateGUI += () => AddMouseRect(lowerRight);
            }
        }
    }
}

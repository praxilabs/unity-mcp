using System;
using System.Collections.Generic;
using UnityEngine;

namespace XNode.NodeGroups {
	[CreateNodeMenu("Group", 10)]
	[Serializable]
	[NodeWidth(300)]
public class NodeGroup : Node {

		[Input(backingValue = ShowBackingValue.Never)] public NodeGroup entry;
		[Output] public NodeGroup exit;
		public string comment;

		public int width = 400;
		public int height = 400;
        public int lastWidth = 400;
        public int lastheight = 400;
        public Color color = new Color(1f, 1f, 1f, 0.1f);
		public int collapsedWidth = 250;
		public int collapsedHeight = 150;

		public List<Node> nodes = new List<Node>();

        [HideInInspector] public Vector2 ExpandedSize;

        protected override void OnEnable()
        {
            if (isGroup == false)
            {
				isGroup = true;
            }
			base.OnEnable();
        }

        public override object GetValue(NodePort port) {
			return null;
		}

		/// <summary> Gets nodes in this group </summary>
		public List<Node> GetNodes() {
			List<Node> result = new List<Node>();
			foreach (Node node in graph.nodes) {
				if (node == this) continue;
				if (node == null) continue;
				if (node.position.x < this.position.x) continue;
				if (node.position.y < this.position.y) continue;
				if (node.position.x > this.position.x + width) continue;
				if (node.position.y > this.position.y + height + 30) continue;
				result.Add(node);
			}
			return result;
		}

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            if (from.node == this && from.fieldName == "exit" && !(to.node is NodeGroup))
                from.Disconnect(to);
        }
    }
}

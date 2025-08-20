from typing import Dict, Any
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection

def register_set_node_as_first_step_tools(mcp: FastMCP):
    """Register SetNodeAsFirstStep tools with the MCP server."""

    @mcp.tool()
    def set_node_as_first_step(
        ctx: Context,
        graph_path: str,
        node_name: str
    ) -> Dict[str, Any]:
        """
        Sets a specific node as the first step in a StepsGraph.

        Args:
            graph_path: Path to the StepsGraph asset (e.g., "Assets/Testing/Graphs/NewStepsGraph.asset").
            node_name: Name of the node to set as first step (e.g., "ClickStep_-7330").

        Returns:
            Dictionary with results ('success', 'message', 'nodeName', etc.).
        """
        try:
            connection = get_unity_connection()
            params = {
                "graphPath": graph_path,
                "nodeName": node_name
            }
            result = connection.send_command("set_node_as_first_step", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to set node as first step: {str(e)}"
            }

    @mcp.tool()
    def list_graph_nodes(
        ctx: Context,
        graph_path: str
    ) -> Dict[str, Any]:
        """
        Lists all nodes in a StepsGraph with their details.

        Args:
            graph_path: Path to the StepsGraph asset.

        Returns:
            Dictionary with list of nodes and their information.
        """
        try:
            connection = get_unity_connection()
            params = {
                "graphPath": graph_path
            }
            result = connection.send_command("list_graph_nodes", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to list graph nodes: {str(e)}"
            }

    @mcp.tool()
    def check_node_exists(
        ctx: Context,
        graph_path: str,
        node_name: str
    ) -> Dict[str, Any]:
        """
        Checks if a node with the given name exists in the graph.

        Args:
            graph_path: Path to the StepsGraph asset.
            node_name: Name of the node to check.

        Returns:
            Dictionary with existence check results.
        """
        try:
            # First get all nodes
            nodes_result = list_graph_nodes(ctx, graph_path)
            
            if not nodes_result.get("success"):
                return nodes_result
            
            # Check if node exists
            nodes = nodes_result.get("data", {}).get("nodes", [])
            node_exists = any(
                node.get("name") == node_name or 
                node.get("name", "").lower() == node_name.lower() or
                node_name.lower() in node.get("name", "").lower()
                for node in nodes
            )
            
            matching_nodes = [
                node for node in nodes 
                if node.get("name") == node_name or 
                   node.get("name", "").lower() == node_name.lower() or
                   node_name.lower() in node.get("name", "").lower()
            ]
            
            return {
                "success": True,
                "exists": node_exists,
                "nodeName": node_name,
                "matchingNodes": matching_nodes,
                "totalNodes": len(nodes),
                "message": f"Node '{node_name}' {'exists' if node_exists else 'does not exist'} in graph"
            }
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to check node existence: {str(e)}"
            }
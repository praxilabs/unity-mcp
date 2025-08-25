"""
XNode Graph Management Tools

This module provides tools for managing XNode graphs, including creating nodes,
deleting nodes, setting positions, and managing node parameters.
"""

from typing import Dict, Any, List, Optional
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection


def register_manage_xnode_node_tools(mcp: FastMCP):
    """Register all XNode management tools with the MCP server."""

    @mcp.tool()
    def create_xnode_node(
        ctx: Context,
        graph_path: str,
        node_type_name: str,
        position_x: float = 0,
        position_y: float = 0
    ) -> Dict[str, Any]:
        """
        Creates a new node in an existing xNode graph.

        Args:
            graph_path: Path to the NodeGraph asset (e.g., "Assets/Testing/Graphs/NewStepsGraph.asset").
            node_type_name: Name of the node type to create (e.g., "ClickStep", "DelayStep").
            position_x: X position of the node in the graph editor.
            position_y: Y position of the node in the graph editor.

        Returns:
            Dictionary with results ('success', 'message', 'nodeId', etc.).
        """
        try:
            connection = get_unity_connection()
            params = {
                "graphPath": graph_path,
                "nodeTypeName": node_type_name,
                "positionX": position_x,
                "positionY": position_y
            }
            result = connection.send_command("create_xnode_node", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to create XNode node: {str(e)}"
            }

    @mcp.tool()
    def delete_xnode_node(
        ctx: Context,
        graph_path: str,
        node_identifier: str,
        identifier_type: str = "name"
    ) -> Dict[str, Any]:
        """
        Deletes a node from an existing xNode graph.

        Args:
            graph_path: Path to the NodeGraph asset (e.g., "Assets/Testing/Graphs/NewStepsGraph.asset").
            node_identifier: Node name (string) or instance ID (int) to delete.
            identifier_type: How to identify the node - "name" or "id" (default: "name").

        Returns:
            Dictionary with results ('success', 'message', 'deletedNode', etc.).
        """
        try:
            connection = get_unity_connection()
            params = {
                "graphPath": graph_path,
                "nodeIdentifier": node_identifier,
                "identifierType": identifier_type
            }
            result = connection.send_command("delete_xnode_node", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to delete XNode node: {str(e)}"
            }

    @mcp.tool()
    def delete_multiple_nodes(
        ctx: Context,
        graph_path: str,
        node_identifiers: List[str],
        identifier_type: str = "name"
    ) -> Dict[str, Any]:
        """
        Deletes multiple nodes from an xNode graph.

        Args:
            graph_path: Path to the NodeGraph asset.
            node_identifiers: List of node names or IDs to delete.
            identifier_type: How to identify nodes - "name" or "id" (default: "name").

        Returns:
            Dictionary with results including success count and any failures.
        """
        try:
            connection = get_unity_connection()
            params = {
                "graphPath": graph_path,
                "nodeIdentifiers": node_identifiers,
                "identifierType": identifier_type
            }
            result = connection.send_command("delete_multiple_nodes", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to delete multiple nodes: {str(e)}"
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
            connection = get_unity_connection()
            params = {
                "graphPath": graph_path,
                "nodeName": node_name
            }
            result = connection.send_command("check_node_exists", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to check node existence: {str(e)}"
            }

    @mcp.tool()
    def set_node_position(
        ctx: Context,
        graph_path: str,
        node_name: str,
        position_x: float,
        position_y: float
    ) -> Dict[str, Any]:
        """
        Sets the position of a specific node in an xNode graph.

        Args:
            graph_path: Path to the NodeGraph asset (e.g., "Assets/Testing/Graphs/NewStepsGraph.asset").
            node_name: Name of the node to modify (e.g., "ClickStep_-64856").
            position_x: X position of the node in the graph editor.
            position_y: Y position of the node in the graph editor.

        Returns:
            Dictionary with results ('success', 'message', 'data', etc.).
        """
        try:
            connection = get_unity_connection()
            
            # First, get the node's instance ID from the node name
            list_params = {
                "graphPath": graph_path
            }
            list_result = connection.send_command("list_graph_nodes", list_params)
            
            if not list_result.get("success", False):
                return {
                    "success": False,
                    "error": f"Failed to list graph nodes: {list_result.get('error', 'Unknown error')}"
                }
            
            # Find the node by name and get its instance ID
            node_id = None
            nodes = list_result.get("data", {}).get("nodes", [])
            for node in nodes:
                if node.get("name") == node_name:
                    node_id = str(node.get("instanceId"))
                    break
            
            if node_id is None:
                return {
                    "success": False,
                    "error": f"Node '{node_name}' not found in graph"
                }
            
            # Now set the position using the node ID
            params = {
                "graphPath": graph_path,
                "nodeId": node_id,
                "positionX": position_x,
                "positionY": position_y
            }
            result = connection.send_command("set_node_position", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to set node position: {str(e)}"
            }

    @mcp.tool()
    def make_connection_between_nodes(
        ctx: Context,
        graph_path: str,
        from_node: str,
        to_node: str,
        from_port: Optional[str] = None,
        to_port: Optional[str] = None
    ) -> Dict[str, Any]:
        """
        Creates a connection between two nodes in a NodeGraph.

        Args:
            graph_path: Path to the NodeGraph asset (required).
            from_node: Source node name (string) or instance ID (int) (required).
            to_node: Target node name (string) or instance ID (int) (required).
            from_port: Output port name on source node (optional, defaults to first output port).
            to_port: Input port name on target node (optional, defaults to first input port).

        Returns:
            Dictionary with results including success status, message, and connection details.
        """
        try:
            connection = get_unity_connection()
            params = {
                "graphPath": graph_path,
                "fromNode": from_node,
                "toNode": to_node
            }
            
            if from_port:
                params["fromPort"] = from_port
            if to_port:
                params["toPort"] = to_port
                
            result = connection.send_command("make_connection_between_nodes", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to create connection: {str(e)}"
            }

    @mcp.tool()
    def list_available_node_types(
        ctx: Context
    ) -> Dict[str, Any]:
        """
        Lists all available node types that can be created in xNode graphs.

        Returns:
            Dictionary with list of available node types and their information.
        """
        try:
            connection = get_unity_connection()
            params = {}
            result = connection.send_command("list_available_node_types", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to list available node types: {str(e)}"
            }

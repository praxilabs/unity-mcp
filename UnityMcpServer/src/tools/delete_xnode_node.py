from typing import Dict, Any, Union
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection

def register_delete_xnode_node_tools(mcp: FastMCP):
    """Register DeleteXNodeNode tools with the MCP server."""

    @mcp.tool()
    def delete_xnode_node(
        ctx: Context,
        graph_path: str,
        node_identifier: Union[str, int],
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
                "error": f"Failed to delete xNode node: {str(e)}"
            }

    @mcp.tool()
    def delete_node_by_name(
        ctx: Context,
        graph_path: str,
        node_name: str
    ) -> Dict[str, Any]:
        """
        Deletes a node from an xNode graph by its name.
        
        Convenience wrapper for delete_xnode_node using node name.

        Args:
            graph_path: Path to the NodeGraph asset.
            node_name: Name of the node to delete.

        Returns:
            Dictionary with deletion results.
        """
        return delete_xnode_node(ctx, graph_path, node_name, "name")

    @mcp.tool()
    def delete_node_by_id(
        ctx: Context,
        graph_path: str,
        node_id: int
    ) -> Dict[str, Any]:
        """
        Deletes a node from an xNode graph by its instance ID.
        
        Convenience wrapper for delete_xnode_node using node ID.

        Args:
            graph_path: Path to the NodeGraph asset.
            node_id: Instance ID of the node to delete.

        Returns:
            Dictionary with deletion results.
        """
        return delete_xnode_node(ctx, graph_path, node_id, "id")

    @mcp.tool()
    def delete_multiple_nodes(
        ctx: Context,
        graph_path: str,
        node_identifiers: list,
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

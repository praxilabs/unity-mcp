from typing import Dict, Any, Optional
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection

def register_manage_registry_data_tools(mcp: FastMCP):
    """Register ManageRegistryData tools with the MCP server."""

    @mcp.tool()
    def list_registry_parents(
        ctx: Context,
        graph_path: str
    ) -> Dict[str, Any]:
        """
        Lists all parent objects (prefab names) from the registry associated with a graph.

        Args:
            graph_path: Path to the StepsGraph asset (e.g., "Assets/Testing Graphs/NewStepsGraph.asset").

        Returns:
            Dictionary with results ('success', 'message', 'parents', 'count', etc.).
        """
        try:
            connection = get_unity_connection()
            params = {
                "graphPath": graph_path
            }
            result = connection.send_command("list_registry_parents", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to list registry parents: {str(e)}"
            }

    @mcp.tool()
    def list_registry_all(
        ctx: Context,
        graph_path: str
    ) -> Dict[str, Any]:
        """
        Lists all items in the registry (flattened list of parent-child combinations).

        Args:
            graph_path: Path to the StepsGraph asset (e.g., "Assets/Testing Graphs/NewStepsGraph.asset").

        Returns:
            Dictionary with results ('success', 'message', 'items', 'count', etc.).
        """
        try:
            connection = get_unity_connection()
            params = {
                "graphPath": graph_path
            }
            result = connection.send_command("list_registry_all", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to list all registry items: {str(e)}"
            }

    @mcp.tool()
    def list_registry_children(
        ctx: Context,
        graph_path: str,
        parent_name: str
    ) -> Dict[str, Any]:
        """
        Lists all children of a specific parent from the registry.

        Args:
            graph_path: Path to the StepsGraph asset (e.g., "Assets/Testing Graphs/NewStepsGraph.asset").
            parent_name: Name of the parent object to get children for (e.g., "Tools", "Cube").

        Returns:
            Dictionary with results ('success', 'message', 'children', 'count', etc.).
        """
        try:
            connection = get_unity_connection()
            params = {
                "graphPath": graph_path,
                "parentName": parent_name
            }
            result = connection.send_command("list_registry_children", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to list registry children: {str(e)}"
            }
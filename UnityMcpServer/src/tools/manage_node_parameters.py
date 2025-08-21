from typing import Dict, Any, Union
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection

def register_manage_node_parameters_tools(mcp: FastMCP):
    """Register ManageNodeParameters tools with the MCP server."""

    @mcp.tool()
    def manage_node_parameters(
        ctx: Context,
        action: str,
        graph_path: str,
        node_name: str,
        parameter_name: str = None,
        parameter_value: Any = None
    ) -> Dict[str, Any]:
        """
        Manages node parameters in xNode graphs (set, get, list).

        Args:
            action: Action to perform - "set", "get", or "list" (required).
            graph_path: Path to the NodeGraph asset (required).
            node_name: Name of the target node (required).
            parameter_name: Name of the parameter (required for "set" and "get" actions).
            parameter_value: Value to set (required for "set" action).

        Returns:
            Dictionary with results including success status, message, and data.
        """
        try:
            # Prepare parameters for Unity command
            params = {
                "action": action,
                "graph_path": graph_path,
                "node_name": node_name
            }
            
            # Add optional parameters if provided
            if parameter_name is not None:
                params["parameter_name"] = parameter_name
            if parameter_value is not None:
                params["parameter_value"] = parameter_value
            
            connection = get_unity_connection()
            result = connection.send_command("manage_node_parameters", params)
            return result
            
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to manage node parameters: {str(e)}"
            }

    @mcp.tool()
    def set_node_parameter(
        ctx: Context,
        graph_path: str,
        node_name: str,
        parameter_name: str,
        parameter_value: Any
    ) -> Dict[str, Any]:
        """
        Sets a parameter value on a specific node.
        
        Convenience wrapper for manage_node_parameters with "set" action.

        Args:
            graph_path: Path to the NodeGraph asset.
            node_name: Name of the target node.
            parameter_name: Name of the parameter to set.
            parameter_value: Value to set for the parameter.

        Returns:
            Dictionary with set operation results.
        """
        return manage_node_parameters(
            ctx, "set", graph_path, node_name, parameter_name, parameter_value
        )

    @mcp.tool()
    def get_node_parameter(
        ctx: Context,
        graph_path: str,
        node_name: str,
        parameter_name: str
    ) -> Dict[str, Any]:
        """
        Gets a parameter value from a specific node.
        
        Convenience wrapper for manage_node_parameters with "get" action.

        Args:
            graph_path: Path to the NodeGraph asset.
            node_name: Name of the target node.
            parameter_name: Name of the parameter to get.

        Returns:
            Dictionary with parameter value and metadata.
        """
        return manage_node_parameters(
            ctx, "get", graph_path, node_name, parameter_name
        )

    @mcp.tool()
    def list_node_parameters(
        ctx: Context,
        graph_path: str,
        node_name: str
    ) -> Dict[str, Any]:
        """
        Lists all available parameters for a specific node.
        
        Convenience wrapper for manage_node_parameters with "list" action.

        Args:
            graph_path: Path to the NodeGraph asset.
            node_name: Name of the target node.

        Returns:
            Dictionary with list of all parameters and their values.
        """
        return manage_node_parameters(
            ctx, "list", graph_path, node_name
        )
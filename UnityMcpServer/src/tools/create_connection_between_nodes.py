from typing import Dict, Any, Union
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection

def register_make_connection_tools(mcp: FastMCP):
    """Register MakeAConnectionBetweenNodes tool with the MCP server."""

    @mcp.tool()
    def make_connection_between_nodes(
        ctx: Context,
        graph_path: str,
        from_node: Union[str, int],
        to_node: Union[str, int],
        from_port: str = None,
        to_port: str = None
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
            # Prepare parameters for Unity command
            params = {
                "graphPath": graph_path,
                "fromNode": from_node,
                "toNode": to_node
            }
            
            # Add optional port parameters if provided
            if from_port is not None:
                params["fromPort"] = from_port
            if to_port is not None:
                params["toPort"] = to_port
            
            connection = get_unity_connection()
            result = connection.send_command("make_connection_between_nodes", params)
            return result
            
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to make connection between nodes: {str(e)}"
            }

    @mcp.tool()
    def connect_nodes_by_name(
        ctx: Context,
        graph_path: str,
        from_node_name: str,
        to_node_name: str,
        from_port: str = None,
        to_port: str = None
    ) -> Dict[str, Any]:
        """
        Creates a connection between two nodes using their names.
        
        Convenience wrapper for make_connection_between_nodes using node names.

        Args:
            graph_path: Path to the NodeGraph asset.
            from_node_name: Name of the source node.
            to_node_name: Name of the target node.
            from_port: Output port name (optional).
            to_port: Input port name (optional).

        Returns:
            Dictionary with connection results.
        """
        return make_connection_between_nodes(
            ctx, graph_path, from_node_name, to_node_name, from_port, to_port
        )

    @mcp.tool()
    def connect_nodes_by_id(
        ctx: Context,
        graph_path: str,
        from_node_id: int,
        to_node_id: int,
        from_port: str = None,
        to_port: str = None
    ) -> Dict[str, Any]:
        """
        Creates a connection between two nodes using their instance IDs.
        
        Convenience wrapper for make_connection_between_nodes using node IDs.

        Args:
            graph_path: Path to the NodeGraph asset.
            from_node_id: Instance ID of the source node.
            to_node_id: Instance ID of the target node.
            from_port: Output port name (optional).
            to_port: Input port name (optional).

        Returns:
            Dictionary with connection results.
        """
        return make_connection_between_nodes(
            ctx, graph_path, from_node_id, to_node_id, from_port, to_port
        )
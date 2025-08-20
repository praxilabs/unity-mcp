from typing import Dict, Any, Optional
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection

def register_create_xnode_node_tools(mcp: FastMCP):
    """Register CreateXNodeNode tools with the MCP server."""

    @mcp.tool()
    def create_xnode_node(
        ctx: Context,
        graph_path: str,
        node_type_name: str,
        position_x: float = 0.0,
        position_y: float = 0.0
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
                "error": f"Failed to create xNode node: {str(e)}"
            }

    @mcp.tool()
    def list_available_node_types(ctx: Context) -> Dict[str, Any]:
        """
        Lists all available node types that can be created in xNode graphs.

        Returns:
            Dictionary with list of available node types and their information.
        """
        try:
            connection = get_unity_connection()
            result = connection.send_command("list_available_node_types", {})
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to list node types: {str(e)}"
            }

    @mcp.tool()
    def create_step_node(
        ctx: Context,
        graph_path: str,
        step_type: str,
        position_x: float = 0.0,
        position_y: float = 0.0
    ) -> Dict[str, Any]:
        """
        Creates a specific step node in an existing StepsGraph.
        
        Args:
            graph_path: Path to the StepsGraph asset.
            step_type: Type of step to create ("ClickStep", "DelayStep", "ExperimentStep", etc.).
            position_x: X position of the node.
            position_y: Y position of the node.

        Returns:
            Dictionary with results.
        """
        # Common step types mapping
        step_type_map = {
            "click": "ClickStep",
            "delay": "DelayStep", 
            "experiment": "ExperimentStep",
            "condition": "ConditionStep",
            "camera": "CameraStep",
            "animation": "AnimationStep"
        }
        
        # Normalize step type
        normalized_type = step_type_map.get(step_type.lower(), step_type)
        
        return create_xnode_node(ctx, graph_path, normalized_type, position_x, position_y)
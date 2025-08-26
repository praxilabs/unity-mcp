from mcp.server.fastmcp import FastMCP, Context
from typing import Dict, Any, Optional
from unity_connection import get_unity_connection

def register_print_tools(mcp: FastMCP):
    """Register Print tool with the MCP server."""

    @mcp.tool()
    def print(ctx: Context, value: Optional[str] = None) -> Dict[str, Any]:
        """Prints a custom message or 'Hello, World!' to Unity's console.

        Args:
            value: Optional custom message to print. If not provided, defaults to 'Hello, World!'.

        Returns:
            Dictionary with results ('success', 'message', 'data').
        """
        try:
            connection = get_unity_connection()
            
            # Prepare arguments for Unity
            args = {}
            if value is not None:
                args["value"] = value
            
            result = connection.send_command("print", args)
            
            return {
                "success": True,
                "message": f"Value '{value or 'Hello, World!'}' printed to Unity console",
                "data": result
            }
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to print message: {str(e)}"
            }
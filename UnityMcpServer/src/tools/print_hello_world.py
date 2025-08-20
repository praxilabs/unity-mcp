from mcp.server.fastmcp import FastMCP, Context
from typing import Dict, Any
from unity_connection import get_unity_connection

def register_print_hello_world_tools(mcp: FastMCP):
    """Register PrintHelloWorld tool with the MCP server."""

    @mcp.tool()
    def print_hello_world(ctx: Context) -> Dict[str, Any]:
        """Prints 'Hello, World!' to Unity's console.

        Returns:
            Dictionary with results ('success', 'message').
        """
        try:
            connection = get_unity_connection()
            result = connection.send_command("print_hello_world", {})
            
            return {
                "success": True,
                "message": "Hello World printed to Unity console",
                "data": result
            }
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to print Hello World: {str(e)}"
            }
from typing import Dict, Any
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection
import os

def register_create_scriptable_object_tools(mcp: FastMCP):
    """Register CreateScriptableObject tool with the MCP server."""

    @mcp.tool()
    def create_scriptable_object(
        ctx: Context,
        scriptable_object_type: str,
        asset_name: str,
        folder: str = "Assets/Testing/"
    ) -> Dict[str, Any]:
        """
        Creates a new ScriptableObject of the specified type in the given folder.

        Args:
            scriptable_object_type: The type of ScriptableObject to create (e.g., "StepsGraph", "ExperimentData").
            asset_name: The name for the asset (without extension).
            folder: Folder path where the asset will be created.

        Returns:
            Dictionary with results ('success', 'message', 'assetPath', 'timestamp').
        """
        try:
            # Ensure folder ends with /
            if not folder.endswith("/"):
                folder += "/"
            
            # Create full asset path with the provided asset name
            asset_path = f"{folder}{asset_name}.asset"
            
            connection = get_unity_connection()
            params = {
                "scriptableObjectType": scriptable_object_type,
                "assetPath": asset_path,
                "assetName": asset_name
            }
            result = connection.send_command("create_scriptable_object", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to create ScriptableObject: {str(e)}"
            }
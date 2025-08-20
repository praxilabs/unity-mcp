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
        folder: str = "Assets/Testing/"
    ) -> Dict[str, Any]:
        """
        Creates a new ScriptableObject of the specified type in the given folder.

        Args:
            scriptable_object_type: The type of ScriptableObject to create (e.g., "StepsGraph", "ExperimentData").
            folder: Folder path where the asset will be created.

        Returns:
            Dictionary with results ('success', 'message', 'assetPath', 'timestamp').
        """
        try:
            # Generate appropriate asset name based on the ScriptableObject type
            if scriptable_object_type.lower() in ["stepsgraph", "steps_graph"]:
                asset_name = "NewStepsGraph"
            elif scriptable_object_type.lower() in ["experimentdata", "experiment_data"]:
                asset_name = "NewExperimentData"
            elif scriptable_object_type.lower() in ["itemregistry", "item_registry"]:
                asset_name = "NewItemRegistry"
            else:
                # Default naming pattern: "New" + ScriptableObject type
                asset_name = f"New{scriptable_object_type}"
            
            # Ensure folder ends with /
            if not folder.endswith("/"):
                folder += "/"
            
            # Create full asset path
            asset_path = f"{folder}{asset_name}.asset"
            
            connection = get_unity_connection()
            params = {
                "scriptableObjectType": scriptable_object_type,
                "assetPath": asset_path
            }
            result = connection.send_command("create_scriptable_object", params)
            return result
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to create ScriptableObject: {str(e)}"
            }
from mcp.server.fastmcp import FastMCP, Context
from typing import Dict, Any
from unity_connection import get_unity_connection
import re

def register_add_click_step_tools(mcp: FastMCP):
    """Register the add click step tool with the MCP server."""

    @mcp.tool()
    def add_click_step(
        ctx: Context,
        prompt: str = None,
        target_graph_path: str = None,
        target_name: str = None,
        step_id: str = None
    ) -> Dict[str, Any]:
        """Adds a click step to a StepsGraph with natural language support.
        
        Args:
            prompt: Natural language description of the click step to add (optional)
            target_graph_path: Path to the StepsGraph asset
            target_name: Target object path in format "prefabName/childName"
            step_id: Optional step ID
            
        Returns:
            Dictionary with results ('success', 'message', 'data')
        """
        try:
            # Parse natural language prompt if provided
            if prompt and not (target_graph_path and target_name):
                # Extract graph path and target from prompt
                graph_match = re.search(r'in\s+([\w.]+\.asset)', prompt)
                target_match = re.search(r'in\s+the\s+(\w+)\s+in\s+the\s+(\w+)\s+button', prompt, re.IGNORECASE)
                
                if graph_match:
                    target_graph_path = graph_match.group(1)
                if target_match:
                    target_name = f"{target_match.group(1)}/{target_match.group(2)}"
                    
                if not target_graph_path or not target_name:
                    return {
                        "success": False,
                        "message": "Could not extract required information from prompt. Please provide explicit target_graph_path and target_name."
                    }

            # Prepare parameters for Unity
            params = {
                "targetGraphPath": target_graph_path,
                "targetName": target_name
            }
            
            if step_id:
                params["stepId"] = step_id

            # Send command to Unity
            response = get_unity_connection().send_command("add_click_step", params)
            
            if response.get("success"):
                return {
                    "success": True,
                    "message": "Click step added successfully",
                    "data": response.get("data")
                }
            else:
                return {
                    "success": False,
                    "message": response.get("error", "Failed to add click step")
                }

        except Exception as e:
            return {
                "success": False,
                "message": f"Error adding click step: {str(e)}"
            }
from typing import Any, Dict
from mcp.server.fastmcp import Context, FastMCP
from unity_connection import get_unity_connection
import re

def register_add_click_step_tools(mcp: FastMCP):
    """Register add click step tools with the MCP server."""
    
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
        return handle_add_click_step({
            "prompt": prompt,
            "target_graph_path": target_graph_path,
            "target_name": target_name,
            "step_id": step_id
        })

def handle_add_click_step(params: Dict[str, Any]) -> Dict[str, Any]:
    """
    Handle add click step tool requests.
    
    Args:
        params: Dictionary containing:
            - prompt: Natural language description (optional)
            - target_graph_path: Path to the StepsGraph asset
            - target_name: Target object path in format "prefabName/childName"
            - step_id: Optional step ID
    
    Returns:
        Dictionary containing the response from Unity
    """
    try:
        prompt = params.get('prompt')
        target_graph_path = params.get('target_graph_path')
        target_name = params.get('target_name')
        step_id = params.get('step_id')
        
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
        
        # Validate required parameters
        if not target_graph_path:
            return {"success": False, "message": "target_graph_path parameter is required"}
        
        if not target_name:
            return {"success": False, "message": "target_name parameter is required"}
            
        # Prepare parameters for Unity
        unity_params = {
            "targetGraphPath": target_graph_path,
            "targetName": target_name
        }
       
        if step_id:
            unity_params["stepId"] = step_id
            
        # Send command to Unity
        response = get_unity_connection().send_command("add_click_step", unity_params)
       
        # Process response from Unity
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
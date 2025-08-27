from typing import Any, Dict
from mcp.server.fastmcp import Context, FastMCP
from unity_connection import get_unity_connection
import re

def register_add_delay_step_tools(mcp: FastMCP):
    """Register add delay step tools with the MCP server."""
    
    @mcp.tool()
    def add_delay_step(
        ctx: Context,
        prompt: str = None,
        target_graph_path: str = None,
        delay_time: float = None,
        step_id: str = None
    ) -> Dict[str, Any]:
        """Adds a delay step to a StepsGraph with natural language support.
       
        Args:
            prompt: Natural language description of the delay step to add (optional)
            target_graph_path: Path to the StepsGraph asset
            delay_time: Delay time in seconds
            step_id: Optional step ID
           
        Returns:
            Dictionary with results ('success', 'message', 'data')
        """
        return handle_add_delay_step({
            "prompt": prompt,
            "target_graph_path": target_graph_path,
            "delay_time": delay_time,
            "step_id": step_id
        })

def handle_add_delay_step(params: Dict[str, Any]) -> Dict[str, Any]:
    """
    Handle add delay step tool requests.
    
    Args:
        params: Dictionary containing:
            - prompt: Natural language description (optional)
            - target_graph_path: Path to the StepsGraph asset
            - delay_time: Delay time in seconds
            - step_id: Optional step ID
    
    Returns:
        Dictionary containing the response from Unity
    """
    try:
        prompt = params.get('prompt')
        target_graph_path = params.get('target_graph_path')
        delay_time = params.get('delay_time')
        step_id = params.get('step_id')
        
        # Parse natural language prompt if provided
        if prompt and not (target_graph_path and delay_time is not None):
            # Extract graph path from prompt
            graph_match = re.search(r'(?:in|graph|asset:?\s*)?([\w-]+\.asset)', prompt, re.IGNORECASE)
            
            # Extract delay time from prompt - more flexible patterns
            delay_patterns = [
                r'(?:wait|delay|for:?\s*)?(\d+(?:\.\d+)?)\s*(?:seconds?|s)?',  # "wait 2.5 seconds", "delay 3s"
                r'(\d+(?:\.\d+)?)\s*(?:second|sec|s)\s*(?:wait|delay)?',        # "2.5 second delay"
                r'pause\s*(?:for)?\s*(\d+(?:\.\d+)?)',                         # "pause for 1.5"
            ]
            
            if graph_match:
                target_graph_path = graph_match.group(1)
                
            # Try each delay pattern
            for pattern in delay_patterns:
                delay_match = re.search(pattern, prompt, re.IGNORECASE)
                if delay_match and delay_time is None:
                    try:
                        delay_time = float(delay_match.group(1))
                        break
                    except ValueError:
                        continue
               
            if not target_graph_path or delay_time is None:
                return {
                    "success": False,
                    "message": "Could not extract required information from prompt. Please provide explicit target_graph_path and delay_time."
                }
        
        # Validate required parameters
        if not target_graph_path:
            return {"success": False, "message": "target_graph_path parameter is required"}
        
        if delay_time is None:
            return {"success": False, "message": "delay_time parameter is required"}
            
        # Validate delay_time is non-negative
        if delay_time < 0:
            return {"success": False, "message": "delay_time must be non-negative"}
            
        # Prepare parameters for Unity
        unity_params = {
            "targetGraphPath": target_graph_path,
            "delayTime": delay_time
        }
       
        if step_id:
            unity_params["stepId"] = step_id
            
        # Send command to Unity
        response = get_unity_connection().send_command("add_delay_step", unity_params)
       
        # Process response from Unity
        if response.get("success"):
            return {
                "success": True,
                "message": "Delay step added successfully",
                "data": response.get("data")
            }
        else:
            return {
                "success": False,
                "message": response.get("error", "Failed to add delay step")
            }
            
    except Exception as e:
        return {
            "success": False,
            "message": f"Error adding delay step: {str(e)}"
        }
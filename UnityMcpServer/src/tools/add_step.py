from typing import Any, Dict, Optional, List
from mcp.server.fastmcp import Context, FastMCP
from unity_connection import get_unity_connection
import re

def register_add_step_tools(mcp: FastMCP):
    """Register generic add step tools with the MCP server."""
    
    @mcp.tool()
    def add_step(
        ctx: Context,
        prompt: str = None,
        step_type: str = None,
        target_graph_path: str = None,
        delay_time: float = None,
        target_name: str = None,
        step_id: str = None,
        position: List[float] = None,
        tooltip: str = None
    ) -> Dict[str, Any]:
        """Adds a step to a StepsGraph with natural language support.
       
        Args:
            prompt: Natural language description of the step to add (optional)
            step_type: Type of step ('delay', 'click', etc.)
            target_graph_path: Path to the StepsGraph asset
            delay_time: Delay time in seconds (for delay steps)
            target_name: Target object name in format 'prefabName/childName' (for click steps)
            step_id: Optional step ID
            position: Optional position as [x, y]
            tooltip: Optional tooltip text
           
        Returns:
            Dictionary with results ('success', 'message', 'data')
        """
        return handle_add_step({
            "prompt": prompt,
            "step_type": step_type,
            "target_graph_path": target_graph_path,
            "delay_time": delay_time,
            "target_name": target_name,
            "step_id": step_id,
            "position": position,
            "tooltip": tooltip
        })

class StepParser:
    """Handles parsing of natural language prompts for step creation."""
    
    # Step type patterns and keywords
    STEP_TYPES = {
        'delay': {
            'keywords': ['delay', 'wait', 'pause', 'sleep'],
            'patterns': [
                r'(?:wait|delay|for:?\s*)?(\d+(?:\.\d+)?)\s*(?:seconds?|s)?',
                r'(\d+(?:\.\d+)?)\s*(?:second|sec|s)\s*(?:wait|delay)?',
                r'pause\s*(?:for)?\s*(\d+(?:\.\d+)?)',
            ]
        },
        'click': {
            'keywords': ['click', 'tap', 'press', 'button'],
            'patterns': [
                r'(?:click|tap|press)\s+(?:on\s+)?([\w-]+)(?:/([^/\s]+))?',
                r'(?:button|target)\s+([\w-]+)(?:/([^/\s]+))?',
            ]
        }
    }
    
    # Common patterns
    GRAPH_PATTERN = re.compile(r'(?:in|graph|asset:?\s*)?([\w-]+\.asset)', re.IGNORECASE)
    STEP_TYPE_PATTERN = re.compile(r'(?:add|create)\s+(?:a\s+)?(\w+)\s*(?:step)?', re.IGNORECASE)
    
    @classmethod
    def parse_prompt(cls, prompt: str) -> Dict[str, Any]:
        """Parse a natural language prompt to extract step parameters."""
        if not prompt:
            return {}
            
        result = {}
        prompt_lower = prompt.lower()
        
        # Extract graph path
        graph_match = cls.GRAPH_PATTERN.search(prompt)
        if graph_match:
            result['target_graph_path'] = graph_match.group(1)
        
        # Determine step type
        step_type = cls._determine_step_type(prompt_lower)
        if step_type:
            result['step_type'] = step_type
            
            # Parse type-specific parameters
            if step_type == 'delay':
                delay_time = cls._parse_delay_time(prompt)
                if delay_time is not None:
                    result['delay_time'] = delay_time
                    
            elif step_type == 'click':
                target_name = cls._parse_click_target(prompt)
                if target_name:
                    result['target_name'] = target_name
        
        return result
    
    @classmethod
    def _determine_step_type(cls, prompt_lower: str) -> Optional[str]:
        """Determine step type from prompt keywords."""
        # Check keywords for each step type
        for step_type, config in cls.STEP_TYPES.items():
            if any(keyword in prompt_lower for keyword in config['keywords']):
                return step_type
        
        # Fallback: try "add [type] step" pattern
        step_match = cls.STEP_TYPE_PATTERN.search(prompt_lower)
        if step_match:
            extracted_type = step_match.group(1).lower()
            if extracted_type in cls.STEP_TYPES:
                return extracted_type
                
        return None
    
    @classmethod
    def _parse_delay_time(cls, prompt: str) -> Optional[float]:
        """Parse delay time from prompt."""
        for pattern in cls.STEP_TYPES['delay']['patterns']:
            match = re.search(pattern, prompt, re.IGNORECASE)
            if match:
                try:
                    return float(match.group(1))
                except (ValueError, IndexError):
                    continue
        return None
    
    @classmethod
    def _parse_click_target(cls, prompt: str) -> Optional[str]:
        """Parse click target from prompt."""
        for pattern in cls.STEP_TYPES['click']['patterns']:
            match = re.search(pattern, prompt, re.IGNORECASE)
            if match:
                try:
                    prefab_name = match.group(1)
                    child_name = match.group(2) if match.lastindex > 1 and match.group(2) else prefab_name
                    return f"{prefab_name}/{child_name}"
                except IndexError:
                    continue
        return None

def handle_add_step(params: Dict[str, Any]) -> Dict[str, Any]:
    """
    Handle generic add step tool requests.
    
    Args:
        params: Dictionary containing step parameters
    
    Returns:
        Dictionary containing the response from Unity
    """
    try:
        prompt = params.get('prompt')
        step_type = params.get('step_type')
        target_graph_path = params.get('target_graph_path')
        
        # Parse prompt if provided and parameters are missing
        if prompt:
            parsed_params = StepParser.parse_prompt(prompt)
            
            # Use parsed values if original parameters are missing
            if not step_type and 'step_type' in parsed_params:
                step_type = parsed_params['step_type']
            if not target_graph_path and 'target_graph_path' in parsed_params:
                target_graph_path = parsed_params['target_graph_path']
            
            # Merge type-specific parsed parameters
            for key, value in parsed_params.items():
                if key not in ['step_type', 'target_graph_path'] and params.get(key) is None:
                    params[key] = value
        
        # Validate required parameters
        if not step_type:
            available_types = ', '.join(StepParser.STEP_TYPES.keys())
            return {
                "success": False,
                "message": f"Could not determine step type from prompt. Please specify 'step_type' parameter. Available types: {available_types}"
            }
        
        if not target_graph_path:
            return {
                "success": False,
                "message": "target_graph_path parameter is required"
            }
        
        # Validate step type
        if step_type not in StepParser.STEP_TYPES:
            available_types = ', '.join(StepParser.STEP_TYPES.keys())
            return {
                "success": False,
                "message": f"Unknown step type: '{step_type}'. Available types: {available_types}"
            }
        
        # Type-specific validation
        validation_result = _validate_step_params(step_type, params)
        if not validation_result['success']:
            return validation_result
        
        # Prepare Unity parameters
        unity_params = {
            "stepType": step_type,
            "targetGraphPath": target_graph_path
        }
        
        # Add optional parameters
        for param_name, unity_name in [
            ('delay_time', 'delayTime'),
            ('target_name', 'targetName'),
            ('step_id', 'stepId'),
            ('position', 'position'),
            ('tooltip', 'tooltip'),
            ('prompt', 'prompt')  # Include original prompt for Unity-side parsing
        ]:
            if params.get(param_name) is not None:
                unity_params[unity_name] = params[param_name]
        
        # Send command to Unity (assuming generic handler exists)
        response = get_unity_connection().send_command("add_step", unity_params)
        
        # Process response from Unity
        if response.get("success"):
            return {
                "success": True,
                "message": f"{step_type.capitalize()} step added successfully",
                "data": response.get("data")
            }
        else:
            return {
                "success": False,
                "message": response.get("error", f"Failed to add {step_type} step")
            }
            
    except Exception as e:
        return {
            "success": False,
            "message": f"Error adding step: {str(e)}"
        }

def _validate_step_params(step_type: str, params: Dict[str, Any]) -> Dict[str, Any]:
    """Validate step-specific parameters."""
    
    if step_type == 'delay':
        delay_time = params.get('delay_time')
        if delay_time is None:
            return {
                "success": False,
                "message": "delay_time parameter is required for delay steps"
            }
        if delay_time < 0:
            return {
                "success": False,
                "message": "delay_time must be non-negative"
            }
            
    elif step_type == 'click':
        target_name = params.get('target_name')
        if not target_name:
            return {
                "success": False,
                "message": "target_name parameter is required for click steps"
            }
        if '/' not in target_name:
            return {
                "success": False,
                "message": "target_name must be in format 'prefabName/childName'"
            }
    
    return {"success": True}

# Backwards compatibility - keep individual tool functions
def register_add_delay_step_tools(mcp: FastMCP):
    """Register delay step tools for backwards compatibility."""
    
    @mcp.tool()
    def add_delay_step(
        ctx: Context,
        prompt: str = None,
        target_graph_path: str = None,
        delay_time: float = None,
        step_id: str = None
    ) -> Dict[str, Any]:
        """Adds a delay step to a StepsGraph with natural language support."""
        return handle_add_step({
            "prompt": prompt,
            "step_type": "delay",
            "target_graph_path": target_graph_path,
            "delay_time": delay_time,
            "step_id": step_id
        })

def register_add_click_step_tools(mcp: FastMCP):
    """Register click step tools for backwards compatibility."""
    
    @mcp.tool()
    def add_click_step(
        ctx: Context,
        prompt: str = None,
        target_graph_path: str = None,
        target_name: str = None,
        step_id: str = None
    ) -> Dict[str, Any]:
        """Adds a click step to a StepsGraph with natural language support."""
        return handle_add_step({
            "prompt": prompt,
            "step_type": "click",
            "target_graph_path": target_graph_path,
            "target_name": target_name,
            "step_id": step_id
        })
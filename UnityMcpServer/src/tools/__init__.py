from .manage_script import register_manage_script_tools
from .manage_scene import register_manage_scene_tools
from .manage_editor import register_manage_editor_tools
from .manage_gameobject import register_manage_gameobject_tools
from .manage_asset import register_manage_asset_tools
from .manage_shader import register_manage_shader_tools
from .read_console import register_read_console_tools
from .execute_menu_item import register_execute_menu_item_tools
from .print import register_print_tools
from .create_scriptable_object import register_create_scriptable_object_tools
from .set_node_first_step import register_set_node_as_first_step_tools
from .create_connection_between_nodes import register_make_connection_tools
from .manage_node_parameters import register_manage_node_parameters_tools
from .manage_registry_data import register_manage_registry_data_tools
from .manage_xnode_node import register_manage_xnode_node_tools

def register_all_tools(mcp):
    """Register all refactored tools with the MCP server."""
    print("Registering Unity MCP Server refactored tools...")
    register_manage_script_tools(mcp)
    register_manage_scene_tools(mcp)
    register_manage_editor_tools(mcp)
    register_manage_gameobject_tools(mcp)
    register_manage_asset_tools(mcp)
    register_print_tools(mcp)
    register_create_scriptable_object_tools(mcp)
    register_manage_registry_data_tools(mcp)
    register_make_connection_tools(mcp)
    register_manage_shader_tools(mcp)
    register_read_console_tools(mcp)
    register_set_node_as_first_step_tools(mcp)
    register_manage_node_parameters_tools(mcp)
    register_execute_menu_item_tools(mcp)
    register_manage_xnode_node_tools(mcp)
    print("Unity MCP Server tool registration complete.")

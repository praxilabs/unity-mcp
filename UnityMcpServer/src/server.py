from mcp.server.fastmcp import FastMCP, Context, Image
import logging
from dataclasses import dataclass
from contextlib import asynccontextmanager
from typing import AsyncIterator, Dict, Any, List
from config import config
from tools import register_all_tools
from unity_connection import get_unity_connection, UnityConnection

# Configure logging using settings from config
logging.basicConfig(
    level=getattr(logging, config.log_level),
    format=config.log_format
)
logger = logging.getLogger("unity-mcp-server")

# Global connection state
_unity_connection: UnityConnection = None

@asynccontextmanager
async def server_lifespan(server: FastMCP) -> AsyncIterator[Dict[str, Any]]:
    """Handle server startup and shutdown."""
    global _unity_connection
    logger.info("Unity MCP Server starting up")
    try:
        _unity_connection = get_unity_connection()
        logger.info("Connected to Unity on startup")
    except Exception as e:
        logger.warning(f"Could not connect to Unity on startup: {str(e)}")
        _unity_connection = None
    try:
        # Yield the connection object so it can be attached to the context
        # The key 'bridge' matches how tools like read_console expect to access it (ctx.bridge)
        yield {"bridge": _unity_connection}
    finally:
        if _unity_connection:
            _unity_connection.disconnect()
            _unity_connection = None
        logger.info("Unity MCP Server shut down")

# Initialize MCP server
mcp = FastMCP(
    "unity-mcp-server",
    lifespan=server_lifespan
)

# Register all tools
register_all_tools(mcp)

# Test AddClickStep tool
# async def test_add_click_step():
#     # Example payload, adjust as needed for your tool's expected input
#     payload = { "step_description": "Click the Play button in the Unity Editor"
#         "target_graph_path": "C:\Dahdouha\virtual-labs-main\Assets\-AssetBundlesXnode\Phy\Photoelectric Effect Data (xNode)-20250604T123125Z-1-001\Add a click step in Assets\-AssetBundlesXnode\Phy\Photoelectric Effect Data (xNode)-20250604T123125Z-1-001\Photoelectric Effect Data (xNode)\PHY_Photoelectric_Effect_Stage_01.asset
# targetting PHY_Photoelectric_Effect_Stage_01/m_EditorClassIdentifier

# "
#     }
#     try:
#         # Call the tool using MCP's internal dispatch
#         result = await mcp.call_tool("add_click_step", payload)
#         logger.info(f"AddClickStep tool test result: {result}")
#     except Exception as e:
#         logger.error(f"AddClickStep tool test failed: {e}")

import asyncio

if __name__ == "__main__":
    # Run the test before starting the server
    # asyncio.run(test_add_click_step())
    mcp.run(transport='stdio')

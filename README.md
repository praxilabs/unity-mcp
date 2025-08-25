# Unity MCP Bridge ✨

#### Maintained by Praxilabs, the AI assistant for Unity.

[![](https://img.shields.io/badge/Unity-000000?style=flat&logo=unity&logoColor=blue 'Unity')](https://unity.com/releases/editor/archive)
[![python](https://img.shields.io/badge/Python-3.12-3776AB.svg?style=flat&logo=python&logoColor=white)](https://www.python.org)
[![](https://badge.mcpx.dev?status=on 'MCP Enabled')](https://modelcontextprotocol.io/introduction)
[![](https://img.shields.io/badge/License-MIT-red.svg 'MIT License')](https://opensource.org/licenses/MIT)

**Create your Unity apps with LLMs!**

Unity MCP Bridge connects Unity with AI assistants through the Model Context Protocol (MCP), enabling you to create Unity applications using natural language commands.

## 🤔 How It Works

Unity MCP Bridge uses a three-component architecture to connect AI assistants with Unity:

```
[AI Assistant (Cursor/Claude)] ←→ [Unity MCP Server (Python)] ←→ [Unity MCP Bridge (Unity Package)]
```

### 🔧 System Components

1. **AI Assistant** (Cursor, Claude Desktop, etc.)
   - Sends natural language commands
   - Receives tool responses and Unity data
   - Manages conversation context

2. **Unity MCP Server** (Python)
   - Translates MCP protocol to Unity commands
   - Manages tool execution and responses
   - Handles communication between AI and Unity

3. **Unity MCP Bridge** (Unity Package)
   - Executes Unity operations (create objects, manage assets, etc.)
   - Provides real-time Unity data and status
   - Manages Unity Editor state

### 🔄 Data Flow

1. **Command Flow**: AI Assistant → MCP Server → Unity Bridge → Unity Editor
2. **Response Flow**: Unity Editor → Unity Bridge → MCP Server → AI Assistant
3. **Real-time Updates**: Unity state changes are automatically reported back

### 🛠️ Tool Execution

When you ask an AI to "create a red cube", the system:
1. **Parses** the natural language request
2. **Maps** it to appropriate Unity MCP tools
3. **Executes** the commands in Unity
4. **Reports** back the results and any errors

## 🚀 Quick Start

### 1. GitHub Setup (Required First)
```bash
# Generate SSH key (recommended)
ssh-keygen -t rsa -b 4096 -C "your-email@example.com"

# Add to GitHub: Settings → SSH and GPG keys → New SSH key
# Test connection
ssh -T git@github.com
```

**📖 See [GitHub Setup Guide](docs/GITHUB-SETUP.md) for detailed instructions.**

### 2. Clone Repository
```bash
# SSH (recommended)
git clone --recurse-submodules git@github.com:praxilabs/unity-mcp.git
```

### 3. Install Unity Package
- Open Unity Hub → Create new 3D project
- Window → Package Manager → + → Add package from git URL
- Enter: `https://github.com/praxilabs/unity-mcp.git?path=/UnityMcpBridge`

### 4. Configure MCP Client
- Install [Cursor](https://cursor.sh/) or [Claude Desktop](https://claude.ai/download)
- Unity → Window → Unity MCP → Auto Configure
- Verify green status indicator 🟢

### 5. Test Setup
```python
# Test system connectivity
print_hello_world(random_string="test")

# Create a ScriptableObject
create_scriptable_object(scriptable_object_type="ExperimentData", folder="Assets/Testing/")

# List available node types
list_available_node_types()
```

## 📚 Documentation

- **[Tool Reference](docs/tools/README.md)** - Complete tool documentation
- **[System Explanation](docs/system-explanation/README.md)** - Detailed system architecture and components
- **[GitHub Setup](docs/GITHUB-SETUP.md)** - Repository and submodule setup

## 🛠️ Available Tools

### Experiment & Graph Tools
- **[XNode Node Management](docs/tools/xnode-node-management.md)** - Create, list, delete, and position nodes in XNode graphs
- **[Node Parameter Management](docs/tools/node-parameter-management.md)** - Set, get, and list node parameters
- **[Node Connection Tools](docs/tools/node-connection-tools.md)** - Create connections between nodes in XNode graphs

### Registry Management
- **[Registry Management](docs/tools/registry-management.md)** - List registry items and discover components

### Utility Tools
- **[Utility Tools](docs/tools/utility-tools.md)** - System verification and basic asset creation

**📖 See [Complete Tool Reference](docs/tools/README.md) for detailed documentation.**

## 🔧 System Requirements

- **Unity**: 2022.3 LTS or newer
- **Python**: 3.12 or newer
- **Operating System**: Windows 10+, macOS 10.15+, or Linux
- **Memory**: 8GB RAM minimum, 16GB recommended
- **MCP Client**: Cursor IDE, Claude Desktop, or compatible client

## 🆘 Troubleshooting

### Common Issues
- **Unity Bridge Not Connecting**: Ensure Unity Editor is open and package is installed
- **MCP Client Not Connecting**: Check server configuration and ports
- **Submodule Issues**: See [GitHub Setup Guide](docs/GITHUB-SETUP.md)

### Getting Help
- [GitHub Issues](https://github.com/praxilabs/unity-mcp/issues)
- [Tool Reference](docs/tools/README.md)

## 📄 License

MIT License. See [LICENSE](LICENSE) file.

---

**Ready to start?** Explore the [Tool Reference](docs/tools/README.md) or learn about the [System Architecture](docs/system-explanation/README.md)!

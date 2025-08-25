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

**📖 See [GitHub Setup Guide](Docs/GITHUB-SETUP.md) for detailed instructions.**

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

- **[Tool Reference](Docs/tools/README.md)** - Complete tool documentation
- **[System Explanation](Docs/system-explanation/README.md)** - Detailed system architecture and components
- **[GitHub Setup](Docs/GITHUB-SETUP.md)** - Repository and submodule setup

## 🎯 AI Development Framework

Unity MCP Bridge includes a comprehensive AI prompting framework based on the [Autonomous Agent Prompting Framework](https://gist.github.com/aashari/07cc9c1b6c0debbeb4f4d94a3a81339e) to optimize AI assistant interactions.

### 🧠 CursorRules Structure

```
CursorRules/
├── rules/
│   ├── General Rules/           # Core AI prompting framework
│   │   ├── core.mdc            # Autonomous Principal Engineer doctrine
│   │   ├── request.mdc         # Standard operating protocol
│   │   ├── refresh.mdc         # Root cause analysis protocol
│   │   ├── retro.mdc           # Self-improvement loop
│   │   ├── concise.mdc         # Communication optimization
│   │   └── no-absolute-right.mdc # Professional communication
│   └── Project Specific Rules/ # Unity MCP Bridge specific rules
│       ├── unity-mcp-essential.mdc    # Core Unity MCP concepts
│       ├── mcp-tools-reference.mdc    # Complete tool reference
│       └── step-nodes-reference.mdc   # XNode step node guide
```

### 🔧 Framework Components

#### **General Rules** (Universal AI Behavior)
- **[Core Doctrine](CursorRules/rules/General Rules/core.mdc)** - Autonomous Principal Engineer identity and operational protocols
- **[Request Protocol](CursorRules/rules/General Rules/request.mdc)** - Standard operating procedure for constructive work
- **[Refresh Protocol](CursorRules/rules/General Rules/refresh.mdc)** - Root cause analysis and remediation
- **[Retro Protocol](CursorRules/rules/General Rules/retro.mdc)** - Metacognitive self-improvement loop
- **[Communication Rules](CursorRules/rules/General Rules/concise.mdc)** - Optimized communication patterns
- **[Professional Standards](CursorRules/rules/General Rules/no-absolute-right.mdc)** - Professional interaction guidelines

#### **Project Specific Rules** (Unity MCP Bridge Context)
- **[Unity MCP Essentials](CursorRules/rules/Project Specific Rules/unity-mcp-essential.mdc)** - Core concepts and patterns
- **[MCP Tools Reference](CursorRules/rules/Project Specific Rules/mcp-tools-reference.mdc)** - Complete tool documentation
- **[Step Nodes Reference](CursorRules/rules/Project Specific Rules/step-nodes-reference.mdc)** - XNode step node patterns

### 🚀 Using the Framework

#### **For Development Tasks**
1. **Install Core Doctrine**: Set `core.mdc` as your global AI rule
2. **Use Request Protocol**: Paste `request.mdc` content for feature development
3. **Apply Project Rules**: Include relevant project-specific rules for context

#### **For Problem Solving**
1. **Use Refresh Protocol**: Apply `refresh.mdc` for persistent issues
2. **Follow Root Cause Analysis**: Systematic problem diagnosis and resolution

#### **For Continuous Improvement**
1. **End with Retro**: Use `retro.mdc` to capture learnings
2. **Update Doctrine**: Integrate lessons back into the core framework

### 🎯 Framework Benefits

- **Autonomous Operation**: AI agents work independently with minimal intervention
- **Systematic Approach**: Structured workflow ensures thorough analysis and execution
- **Quality Assurance**: Built-in verification and self-audit protocols
- **Continuous Learning**: Framework evolves based on project experiences
- **Professional Standards**: Maintains high-quality communication and execution

## 🛠️ Available Tools

### Experiment & Graph Tools
- **[XNode Node Management](Docs/tools/xnode-node-management.md)** - Create, list, delete, and position nodes in XNode graphs
- **[Node Parameter Management](Docs/tools/node-parameter-management.md)** - Set, get, and list node parameters
- **[Node Connection Tools](Docs/tools/node-connection-tools.md)** - Create connections between nodes in XNode graphs

### Registry Management
- **[Registry Management](Docs/tools/registry-management.md)** - List registry items and discover components

### Utility Tools
- **[Utility Tools](Docs/tools/utility-tools.md)** - System verification and basic asset creation

**📖 See [Complete Tool Reference](Docs/tools/README.md) for detailed documentation.**

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
- **Submodule Issues**: See [GitHub Setup Guide](Docs/GITHUB-SETUP.md)

### Getting Help
- [GitHub Issues](https://github.com/praxilabs/unity-mcp/issues)
- [Tool Reference](Docs/tools/README.md)

## 📄 License

MIT License. See [LICENSE](LICENSE) file.

---

**Ready to start?** Explore the [Tool Reference](Docs/tools/README.md) or learn about the [System Architecture](Docs/system-explanation/README.md)!

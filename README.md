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

**📖 See [GitHub Setup Guide](Docs/GITHUB-SETUP.md) for detailed instructions.**

### 2. Clone Repository
**📖 See [GitHub Setup Guide](Docs/GITHUB-SETUP.md) for detailed instructions.**


### 3. Install Unity Package
- Open Unity Hub → Create new 3D project
- Window → Package Manager → + → Add package from git URL
- Enter: `https://github.com/praxilabs/unity-mcp.git?path=/UnityMcpBridge`

### 4. Install MCP Server
- Go to **Window** → **Unity MCP** → **Installation Manager**
- Click the **Install Server** button 

### 5. Configure MCP Client
- Install [Cursor](https://cursor.sh/) or [Claude Desktop](https://claude.ai/download)
- Unity → Window → Unity MCP → Auto Configure
- Verify green status indicator 🟢

### 6. Test Setup
- Try generating any logic using Unity MCP Bridges available tools you can find tools in the **"🛠️ Overview on Tools"** section, for
 a more detailed look go to **[Tool Reference](Docs/tools/README.md)** .
- **📖 For system architecture details, see [System Explanation](Docs/system-explanation/README.md)** .

## 🎯 Using Cursor Rules Framework

The Unity MCP Bridge includes a comprehensive **Cursor Rules** framework that provides AI assistants with specialized knowledge for Unity development tasks.

### What are Cursor Rules?
Cursor Rules are `.mdc` files that contain domain-specific knowledge and guidelines for AI assistants. They help the AI understand:
- **Unity-specific workflows** and best practices
- **Graph logic patterns** for XNode-based experiments
- **Tool usage guidelines** and common scenarios
- **Error handling** and troubleshooting approaches

### Available Rule Categories

#### **Core Rules** (`CursorRules/rules/Core/`)
- `core.mdc` - Fundamental Unity MCP concepts and patterns
- `concise.mdc` - Guidelines for clear, minimal documentation
- `request.mdc` - How to handle user requests effectively
- `retro.mdc` - Retrospective analysis and improvement patterns

#### **Graph Logic Rules** (`CursorRules/rules/MCP/GraphLogic/`)
- **Control Flow** - Managing sequence execution and branching
- **Click Nodes** - Mouse interaction and UI element handling
- **Camera Nodes** - Camera positioning and movement logic
- **UI Nodes** - Popup messages, MCQs, and interface elements
- **Loop Nodes** - Iteration and conditional execution
- **Progress Nodes** - Experiment progression tracking
- **Table Nodes** - Data management and record keeping
- **Tool Nodes** - Collider toggling and utility operations
- **Utility Nodes** - Delays, freezing, and system utilities
- **Misc Nodes** - Animation and other specialized operations

### How to Use Cursor Rules

1. **Automatic Application**: Rules are automatically applied when using Cursor IDE with this project
2. **Context-Aware Suggestions**: AI will suggest appropriate tools and patterns based on your request
3. **Best Practice Guidance**: Rules ensure consistent, high-quality Unity development workflows
4. **Error Prevention**: Built-in guidelines help avoid common Unity development pitfalls

### Specialized Protocol Rules

#### **@request.mdc** - Feature Request Protocol
When you need to request new features, refactoring, or changes:
- **Structured Approach**: Follows a 5-phase protocol from reconnaissance to final verification
- **System-Wide Analysis**: Ensures all dependencies and impacts are considered
- **Zero-Trust Audit**: Mandatory self-audit to prevent regressions
- **Evidence-Based**: Requires empirical verification of all changes

#### **@retro.mdc** - Retrospective & Learning Protocol
After completing work, use this to:
- **Analyze Performance**: Review the entire session for successes and failures
- **Distill Lessons**: Extract durable, universal principles from the interaction
- **Update Doctrine**: Integrate learnings into the AI's operational rules
- **Continuous Improvement**: Evolve the AI's capabilities based on real experiences

#### **@refresh.mdc** - Bug Fix & Root Cause Analysis Protocol
For persistent bugs or issues:
- **Deep Diagnostics**: Systematic investigation beyond surface-level fixes
- **Root Cause Focus**: Identifies the absolute underlying cause, not just symptoms
- **Reproducible Testing**: Creates minimal test cases to verify fixes
- **Regression Prevention**: Ensures fixes don't introduce new problems

### Example Usage
When you ask the AI to "create a click sequence that shows a popup", the rules framework will:
- Suggest appropriate **ClickStep** and **UINodes**
- Guide you through proper **Control Flow** setup
- Recommend **Progress tracking** for experiment management
- Ensure proper **Error handling** and validation

### Customizing Rules
- Rules are located in `CursorRules/rules/` directory
- Each `.mdc` file contains specific domain knowledge
- Modify rules to match your project's specific requirements
- Rules follow markdown format with clear sections and examples

## 🛠️ Overview on Tools

### Experiment & Graph Tools
- **[XNode Node Management](Docs/tools/xnode-node-management.md)** - Create, list, delete, and position nodes in XNode graphs
- **[Node Parameter Management](Docs/tools/node-parameter-management.md)** - Set, get, and list node parameters
- **[Node Connection Tools](Docs/tools/node-connection-tools.md)** - Create connections between nodes in XNode graphs

### Registry Management
- **[Registry Management](Docs/tools/registry-management.md)** - List registry items and discover components

### Utility Tools
- **[Utility Tools](Docs/tools/utility-tools.md)** - System verification and basic asset creation

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

## 📚 Documentation

- **[Tool Reference](Docs/tools/README.md)** - Complete tool documentation
- **[System Explanation](Docs/system-explanation/README.md)** - Detailed system architecture and components
- **[GitHub Setup](Docs/GITHUB-SETUP.md)** - Repository and submodule setup

### Getting Help
- [GitHub Issues](https://github.com/praxilabs/unity-mcp/issues)

## 📄 License

MIT License. See [LICENSE](LICENSE) file.

---

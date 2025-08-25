# Unity MCP Bridge Documentation 📚

Complete documentation for Unity MCP Bridge - the AI-powered Unity development platform.

## 📖 Quick Navigation

### 🛠️ [Tool Reference](tools/README.md)
- Complete tool catalog with implementation details
- Parameter documentation and usage examples
- Internal code explanations

### 🏗️ [System Explanation](system-explanation/README.md)
- Detailed system architecture and components
- How the three-layer system works
- Data flow and communication patterns

### 🔧 [GitHub Setup](GITHUB-SETUP.md)
- Repository cloning with submodules
- SSH/HTTPS configuration
- Troubleshooting submodule issues

## 🎯 Tool Categories

### Experiment & Graph Tools
- **XNode Node Management** - Create, list, delete, and position nodes in XNode graphs
- **Node Parameter Management** - Set, get, and list node parameters
- **Node Connection Tools** - Create connections between nodes in XNode graphs

### Registry Management
- **Registry Exploration** - List registry items and discover components
- **Component Discovery** - Find available components and their methods

### Utility Tools
- **System Verification** - Test Unity MCP Bridge connectivity
- **Asset Creation** - Create ScriptableObject assets programmatically

## 🔍 Quick Reference

### Essential Tools
```python
# Test system connectivity
print_hello_world(random_string="test")

# Create experiments
create_xnode_node(graph_path="Assets/Experiment.asset", node_type_name="ClickStep")

# Manage parameters
manage_node_parameters(action="set", graph_path="Assets/Experiment.asset", node_name="ClickStep_123", parameter_name="_targetName", parameter_value={"prefabName": "Tools", "childName": "Cube (1)"})

# Connect nodes
make_connection_between_nodes(graph_path="Assets/Experiment.asset", from_node="ClickStep_123", to_node="DelayStep_456")
```

### Common Workflows
1. **Interactive Experiments** - Build node-based experiment flows
2. **Parameter Configuration** - Set and manage node parameters
3. **Graph Construction** - Create and connect nodes
4. **Registry Exploration** - Discover available components and methods
5. **System Verification** - Test and debug Unity MCP Bridge

## 🆘 Need Help?

- **Installation Issues**: See main [README.md](../README.md) for setup instructions
- **Tool Usage**: [Tool Reference](tools/README.md)
- **System Understanding**: [System Explanation](system-explanation/README.md)
- **GitHub Issues**: [GitHub Setup](GITHUB-SETUP.md)
- **Community Support**: [GitHub Issues](https://github.com/praxilabs/unity-mcp/issues)

---

**Start Here**: Main [README.md](../README.md) → [Tool Reference](tools/README.md) → [System Explanation](system-explanation/README.md)

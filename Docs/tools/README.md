# Unity MCP Bridge Tools Documentation 🛠️

Complete documentation for all Unity MCP Bridge tools with detailed internal implementation explanations.

## 📋 Overview

This section contains detailed documentation for each tool group in the Unity MCP Bridge system. Each tool documentation includes:
- **Implementation Details** - How the tool's code works internally
- **Code Snippets** - Key implementation patterns

## 🎯 Tool Groups

### XNode Node Management
- **[XNode Node Management](xnode-node-management.md)** - Create, list, delete, and position nodes in XNode graphs

### Node Parameter Management  
- **[Node Parameter Management](node-parameter-management.md)** - Set, get, and list node parameters

### Utility Tools
- **[Utility Tools](utility-tools.md)** - System verification and basic asset creation

### Registry Management
- **[Registry Management](registry-management.md)** - Manage experiment registry data and component information

### Node Connection Tools
- **[Node Connection Tools](node-connection-tools.md)** - Create connections between nodes in XNode graphs

## 🔧 Tool Inventory

### XNode Node Management Tools
- **create_xnode_node** - Create new nodes in XNode graphs
- **list_available_node_types** - List all available node types
- **delete_xnode_node** - Delete nodes by name or ID
- **delete_node_by_name** - Delete node by name (convenience)
- **delete_node_by_id** - Delete node by ID (convenience)  
- **delete_multiple_nodes** - Delete multiple nodes at once
- **set_node_as_first_step** - Set starting node for graph execution
- **list_graph_nodes** - List all nodes in a graph
- **check_node_exists** - Verify node existence
- **set_node_position** - Position nodes in graph editor


### Utility Tools
- **print** - System verification and debugging tool
- **create_scriptable_object** - Create ScriptableObject assets

### Node Connection Tools
- **manage_connection_between_nodes** - Main connection tool
- **connect_nodes_by_name** - Connect nodes by name (convenience)
- **connect_nodes_by_id** - Connect nodes by ID (convenience)
- **delete_connection_by_name** - Delete modes by name
- **delete_connection_by_id** - Delete modes by name
- **delete_connections_in_graph** - Delete all the nodes in a graph 
- **delete_connections_by_node** - Delete all the nodes in a graph 

## 🔍 Quick Reference

### Essential Tools
```python
# Create experiments
create_xnode_node(graph_path="Assets/Experiment.asset", node_type_name="ClickStep")


# Connect nodes
make_connection_between_nodes(graph_path="Assets/Experiment.asset", from_node="ClickStep_123", to_node="DelayStep_456")

# System verification
print(random_string="test")
```

### Common Workflows
1. **Interactive Experiments** - Build node-based experiment flows
2. **Parameter Configuration** - Set and manage node parameters
3. **Graph Construction** - Create and connect nodes
4. **Registry Exploration** - Discover available components and methods
5. **System Verification** - Test and debug Unity MCP Bridge

## 🆘 Need Help?

- **Installation Issues**: See main [README.md](../README.md) for setup instructions
- **Tool Usage**: Individual tool documentation files above
- **System Understanding**: [System Explanation](../system-explanation/README.md)
- **GitHub Issues**: [GitHub Setup](../GITHUB-SETUP.md)
- **Community Support**: [GitHub Issues](https://github.com/praxilabs/unity-mcp/issues)

---

**Start Here**: Main [README.md](../README.md) → [Tool Reference](README.md) → Individual Tool Documentation → [System Explanation](../system-explanation/README.md)

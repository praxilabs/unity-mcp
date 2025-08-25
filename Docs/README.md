# Unity MCP Bridge Documentation 📚

Complete documentation for Unity MCP Bridge - the AI assistant for Unity development.

## 🚀 Quick Navigation

**Start Here**: Main [README.md](../README.md) → [Tool Reference](tools/README.md) → [System Explanation](system-explanation/README.md)

## 📋 Documentation Overview

This documentation suite provides comprehensive coverage of Unity MCP Bridge:

- **[Main README](../README.md)** - Project overview, quick start, system architecture, and AI development framework
- **[Tool Reference](tools/README.md)** - Complete tool documentation and usage guides
- **[System Explanation](system-explanation/README.md)** - Detailed system architecture and extension guide

## 🛠️ Tool Categories

### **XNode Node Management**
Tools for creating, managing, and manipulating nodes in XNode graphs:
- Node creation and deletion
- Node positioning and configuration
- Node type discovery and listing
- First step configuration

### **Node Parameter Management**
Tools for managing node parameters and properties:
- Parameter setting and retrieval
- Parameter listing and discovery
- Complex type conversion and validation

### **Node Connection Tools**
Tools for creating and managing connections between nodes:
- Connection creation and validation
- Port discovery and management
- Connection routing and verification

### **Registry Management**
Tools for exploring and managing registry data:
- Registry item listing and discovery
- Component and method exploration
- Parent-child relationship management

### **Utility Tools**
Basic utility and verification tools:
- System connectivity testing
- ScriptableObject creation
- Basic asset management

## 📖 Quick Reference

### **Common Workflows**
- **Create XNode Graph**: `create_xnode_node` → `set_node_as_first_step` → `make_connection_between_nodes`
- **Manage Node Parameters**: `list_node_parameters` → `set_node_parameter` → `get_node_parameter`
- **Explore Registry**: `list_registry_parents` → `list_registry_children` → `get_child_components`

### **Tool Patterns**
- **Creation Tools**: Return success status and created object details
- **Management Tools**: Support multiple actions (create, modify, delete, list)
- **Discovery Tools**: Return structured data for exploration and analysis

## 🆘 Need Help?

- **[Tool Reference](tools/README.md)** - Detailed tool documentation
- **[System Explanation](system-explanation/README.md)** - Architecture and extension guide
- **[Main README](../README.md)** - Project overview and quick start
- **[GitHub Issues](https://github.com/praxilabs/unity-mcp/issues)** - Report bugs and request features

# Unity MCP Development Tools

Welcome to the Unity MCP development environment! This directory contains tools and utilities to streamline Unity MCP core development.

## 🚀 Available Development Features

### ✅ Development Deployment Scripts
Quick deployment and testing tools for Unity MCP core changes.


## Development Deployment Scripts

These deployment scripts help you quickly test changes to Unity MCP core code.

## Scripts

### `deploy-dev.bat`
Deploys your development code to the actual installation locations for testing.

**What it does:**
1. Backs up original files to a timestamped folder
2. Copies Unity Bridge code to Unity's package cache
3. Copies Python Server code to the MCP installation folder

**Usage:**
1. Run `deploy-dev.bat`
2. Enter Unity package cache path (example provided)
3. Enter server path (or use default: `%LOCALAPPDATA%\Programs\UnityMCP\UnityMcpServer\src`)
4. Enter backup location (or use default: `%USERPROFILE%\Desktop\unity-mcp-backup`)

### `restore-dev.bat`
Restores original files from backup.

**What it does:**
1. Lists available backups with timestamps
2. Allows you to select which backup to restore
3. Restores both Unity Bridge and Python Server files

## Finding Unity Package Cache Path

Unity package cache is typically located at:
```
X:\UnityProject\Library\PackageCache\com.praxilabs.unity-mcp@1.0.0
```

To find it:
1. Open Unity Package Manager
2. Select "Unity MCP" package
3. Right click on the package and "Show in Explorer"
4. Navigate to the path above with your username and version

## Workflow

1. **Make changes** to your source code in this directory
2. **Deploy** using `deploy-dev.bat`
3. **Test** in Unity (restart Unity Editor first)
4. **Iterate** - repeat steps 1-3 as needed
5. **Restore** original files when done using `restore-dev.bat`


## Troubleshooting

### "Path not found" errors running the .bat file
- Verify Unity package cache path is correct
- Check that Unity MCP package is actually installed
- Ensure server is installed via MCP client

### "Permission denied" errors
- Run cmd as Administrator
- Close Unity Editor before deploying
- Close any MCP clients before deploying

### "Backup not found" errors
- Run `deploy-dev.bat` first to create initial backup
- Check backup directory permissions
- Verify backup directory path is correct
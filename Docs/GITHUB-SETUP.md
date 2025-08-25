# Unity MCP Bridge Installation Guide 🔐

Setup guide for cloning the Unity MCP Bridge repository with virtual-labs submodule.

---

## Quick Setup (Choose One Method)

### Method 1: SSH Keys (Recommended - No passwords needed)

1. **Generate SSH key:**
```powershell
ssh-keygen -t rsa -b 4096 -C "your-email@example.com"
# Press Enter 3 times (default location, no passphrase)
```

2. **Copy public key:**
```powershell
Get-Content ~/.ssh/id_rsa.pub | Set-Clipboard
```

3. **Add to GitHub:**
   - Go to [GitHub Settings → SSH and GPG keys](https://github.com/settings/keys)
   - Click "New SSH key" 
   - Paste key, click "Add SSH key"

4. **Test connection:**
```powershell
ssh -T git@github.com
```

**Use SSH URLs:** `git@github.com:username/repository.git`

---

## Clone Unity MCP Bridge Repository

### For SSH (Recommended):

**Option 1: Clone with submodules in one command:**
```powershell
git clone --recurse-submodules git@github.com:praxilabs/unity-mcp.git
```

**Option 2: Clone first, then get submodules:**
```powershell
git clone git@github.com:praxilabs/unity-mcp.git
cd unity-mcp
git submodule update --init --recursive
```


## Fix Virtual Labs Submodule Issues

If `git submodule status` returns nothing or the `submodules/virtual-labs` directory is empty:

1. **Check what's wrong:**
```powershell
git ls-tree HEAD | findstr Submodules
# If empty, submodule was never properly added
```

2. **Fix it:**
```powershell
# Remove broken references
git submodule deinit -f Submodules/virtual-labs
git rm -f Submodules/virtual-labs
rm -rf .git/modules/Submodules/virtual-labs

# Add properly (SSH method)
git submodule add git@github.com:praxilabs/virtual-labs.git Submodules/virtual-labs

# Commit the fix
git add .gitmodules Submodules/virtual-labs
git commit -m "Fix virtual-labs submodule"
```

3. **Verify the submodule is working:**
```powershell
git submodule status
# Should show: [commit-hash] Submodules/virtual-labs (version-tag)
```

4. **For team members cloning the repository:**
```powershell
git clone --recurse-submodules git@github.com:praxilabs/unity-mcp.git
```

---

## Common Issues

**"Repository not found"** → Check if you have access to [praxilabs/unity-mcp](https://github.com/praxilabs/unity-mcp) and [praxilabs/virtual-labs](https://github.com/praxilabs/virtual-labs)

**"Permission denied"** → Re-run `ssh -T git@github.com` or regenerate your [Personal Access Token](https://github.com/settings/personal-access-tokens/tokens)


**Submodule not found** → The virtual-labs submodule should be at `Submodules/virtual-labs`, not `submodules/virtual-labs`

**Authentication failed** → Make sure your SSH key is added to [GitHub SSH keys](https://github.com/settings/keys) or your PAT has `repo` scope

---

## Repository Links

- **Main Repository**: [praxilabs/unity-mcp](https://github.com/praxilabs/unity-mcp)
- **Virtual Labs Submodule**: [praxilabs/virtual-labs](https://github.com/praxilabs/virtual-labs)
- **SSH Key Setup**: [GitHub SSH Documentation](https://docs.github.com/en/authentication/connecting-to-github-with-ssh)
- **Personal Access Token Setup**: [GitHub PAT Documentation](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens)

---

**That's it!** Choose SSH (recommended) or HTTPS with PAT, clone the repository with submodules, and you're ready to go!
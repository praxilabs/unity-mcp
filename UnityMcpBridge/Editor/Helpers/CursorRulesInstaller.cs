using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityMcpBridge.Editor.Helpers
{
    public static class CursorRulesInstaller
    {
        private const string BranchName = "main";
        private const string GitUrl = "https://github.com/praxilabs/unity-mcp.git";
        

        public static void Initialize()
        {
            InstallationManager.OnInstallationCompleted += OnInstallationCompleted;
        }
        
        private static void OnInstallationCompleted()
        {
            InstallCursorMcpRules();
        }
        
        public static bool InstallCursorMcpRules()
        {
            try
            {
                // Get the Unity project directory (where the Assets folder is)
                string unityProjectDir = Application.dataPath;
                string projectDir = Path.GetDirectoryName(unityProjectDir);
                
                // Create .cursor/rules directory structure
                string cursorRulesPath = Path.Combine(projectDir, ".cursor", "rules");
                Directory.CreateDirectory(cursorRulesPath);
                
                // First, check if there's a local CursorRules directory in the parent directory
                string parentDir = Path.GetDirectoryName(projectDir);
                string localCursorRulesPath = Path.Combine(parentDir, "CursorRules");
                
                if (Directory.Exists(localCursorRulesPath))
                {
                    // Use local CursorRules directory
                    string[] sourceFiles = Directory.GetFiles(localCursorRulesPath, "*.mdc", SearchOption.AllDirectories);
                    
                    if (sourceFiles.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Warning", "No .mdc files found in local CursorRules directory.", "OK");
                        return false;
                    }
                    
                    int copiedFiles = 0;
                    foreach (string sourceFile in sourceFiles)
                    {
                        string relativePath = Path.GetRelativePath(localCursorRulesPath, sourceFile);
                        string destinationFile = Path.Combine(cursorRulesPath, Path.GetFileName(sourceFile));
                        
                        File.Copy(sourceFile, destinationFile, true);
                        copiedFiles++;
                    }
                    
                    EditorUtility.DisplayDialog("Success", 
                        $"Successfully installed {copiedFiles} Cursor MCP rules from local directory to:\n\\.cursor\\rules", 
                        "OK");
                    
                    UnityEngine.Debug.Log($"Installed {copiedFiles} Cursor MCP rules from local directory to {cursorRulesPath}");
                    return true;
                }
                
                // Fall back to GitHub if local directory doesn't exist
                // Create a temporary directory for fetching CursorRules from GitHub
                string tempDir = Path.Combine(Path.GetTempPath(), "UnityMCP_CursorRules_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempDir);
                
                try
                {
                    // Initialize git repo in the temp directory
                    CommandRunner.RunCommand("git", "init", workingDirectory: tempDir);
                    
                    // Add remote (handle case where it already exists)
                    try
                    {
                        CommandRunner.RunCommand("git", $"remote add origin {GitUrl}", workingDirectory: tempDir);
                    }
                    catch (Exception)
                    {
                        // Remote might already exist, try to set the URL
                        try
                        {
                            CommandRunner.RunCommand("git", $"remote set-url origin {GitUrl}", workingDirectory: tempDir);
                        }
                        catch (Exception)
                        {
                            // If that fails too, remove and re-add
                            CommandRunner.RunCommand("git", "remote remove origin", workingDirectory: tempDir);
                            CommandRunner.RunCommand("git", $"remote add origin {GitUrl}", workingDirectory: tempDir);
                        }
                    }
                    
                    // Configure sparse checkout
                    CommandRunner.RunCommand("git", "config core.sparseCheckout true", workingDirectory: tempDir);
                    
                    // Set sparse checkout path to only include CursorRules folder
                    string sparseCheckoutPath = Path.Combine(tempDir, ".git", "info", "sparse-checkout");
                    File.WriteAllText(sparseCheckoutPath, "CursorRules/");
                    
                    // Fetch and checkout the branch
                    CommandRunner.RunCommand("git", $"fetch --depth=1 origin {BranchName}", workingDirectory: tempDir);
                    CommandRunner.RunCommand("git", $"checkout {BranchName}", workingDirectory: tempDir);
                    
                    // Path to the fetched CursorRules directory
                    string sourceCursorRulesPath = Path.Combine(tempDir, "CursorRules");
                    
                    if (!Directory.Exists(sourceCursorRulesPath))
                    {
                        EditorUtility.DisplayDialog("Error", "CursorRules directory not found in the repository.", "OK");
                        return false;
                    }
                    
                    // Copy all .mdc files from CursorRules to .cursor/rules
                    string[] sourceFiles = Directory.GetFiles(sourceCursorRulesPath, "*.mdc");
                    
                    if (sourceFiles.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Warning", "No .mdc files found in CursorRules directory.", "OK");
                        return false;
                    }
                    
                    int copiedFiles = 0;
                    foreach (string sourceFile in sourceFiles)
                    {
                        string fileName = Path.GetFileName(sourceFile);
                        string destinationFile = Path.Combine(cursorRulesPath, fileName);
                        
                        File.Copy(sourceFile, destinationFile, true);
                        copiedFiles++;
                    }
                    
                    EditorUtility.DisplayDialog("Success", 
                        $"Successfully installed {copiedFiles} Cursor MCP rules from GitHub to:\n\\.cursor\\rules", 
                        "OK");
                    
                    UnityEngine.Debug.Log($"Installed {copiedFiles} Cursor MCP rules from GitHub to {cursorRulesPath}");
                    return true;
                }
                finally
                {
                    // Clean up temporary directory
                    try
                    {
                        if (Directory.Exists(tempDir))
                        {
                            Directory.Delete(tempDir, true);
                        }
                    }
                    catch (Exception cleanupEx)
                    {
                        UnityEngine.Debug.LogWarning($"Failed to clean up temporary directory: {cleanupEx.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", 
                    $"Failed to install Cursor MCP rules: {e.Message}", 
                    "OK");
                UnityEngine.Debug.LogError($"Failed to install Cursor MCP rules: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }
        
        /// <summary>
        /// Checks if Cursor MCP Rules are already installed in the current Unity project.
        /// </summary>
        /// <returns>True if rules are installed, false otherwise.</returns>
        public static bool AreRulesInstalled()
        {
            try
            {
                string unityProjectDir = Application.dataPath;
                string projectDir = Path.GetDirectoryName(unityProjectDir);
                string cursorRulesPath = Path.Combine(projectDir, ".cursor", "rules");
                
                if (!Directory.Exists(cursorRulesPath))
                {
                    return false;
                }
                
                string[] installedFiles = Directory.GetFiles(cursorRulesPath, "*.mdc");
                return installedFiles.Length > 0;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Gets the path where Cursor MCP Rules are installed.
        /// </summary>
        /// <returns>The installation path, or null if not installed.</returns>
        public static string GetInstallationPath()
        {
            try
            {
                string unityProjectDir = Application.dataPath;
                string projectDir = Path.GetDirectoryName(unityProjectDir);
                string cursorRulesPath = Path.Combine(projectDir, ".cursor", "rules");
                
                return Directory.Exists(cursorRulesPath) ? cursorRulesPath : null;
            }
            catch
            {
                return null;
            }
        }
        

    }
}

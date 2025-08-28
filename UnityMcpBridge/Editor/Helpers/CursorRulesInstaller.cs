using System;
using System.IO;
using System.Threading.Tasks;
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
            InstallationManager.OnInstallationCompleted += static () => _ = InstallCursorMcpRules();
        }
        
        public static async Task<bool> InstallCursorMcpRules()
        {
            try
            {
                // Wait for editor to stop compiling before proceeding
                await WaitForEditorCompilation();
                
                string cursorRulesInstallationPath = GetCursorRulesPath();
                string cursorRulesPath = cursorRulesInstallationPath + "/rules";
                // Delete existing .cursor folder if it exists
                if (Directory.Exists(cursorRulesPath))
                {
                    try
                    {
                        Directory.Delete(cursorRulesPath, true);
                        Debug.Log("Deleted existing .cursor folder for clean installation");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Could not delete existing .cursor folder: {ex.Message}. Installation will continue.");
                    }
                }
                
                Directory.CreateDirectory(cursorRulesInstallationPath);
                
                string sourcePath = GetLocalCursorRulesPath() ?? await FetchFromGitHub();
                
                if (sourcePath != null && Directory.Exists(sourcePath))
                {
                    CopyDirectory(sourcePath, cursorRulesInstallationPath);
                    ShowSuccessMessage(sourcePath.Contains("temp") ? "GitHub" : "local directory");
                    return true;
                }
                
                EditorUtility.DisplayDialog("Error", "CursorRules directory not found.", "OK");
                return false;
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to install Cursor MCP rules: {e.Message}", "OK");
                Debug.LogError($"Failed to install Cursor MCP rules: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }
        
        private static async Task WaitForEditorCompilation()
        {
            while (EditorApplication.isCompiling)
            {
                await Task.Delay(1000);
            }
        }
        
        private static string GetCursorRulesPath()
        {
            string projectDir = Path.GetDirectoryName(Application.dataPath);
            return Path.Combine(projectDir, ".cursor");
        }
        
        private static string GetLocalCursorRulesPath()
        {
            string projectDir = Path.GetDirectoryName(Application.dataPath);
            string parentDir = Path.GetDirectoryName(projectDir);
            string localPath = Path.Combine(parentDir, "CursorRules");
            return Directory.Exists(localPath) ? localPath : null;
        }
        
        private static async Task<string> FetchFromGitHub()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "UnityMCP_CursorRules_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            
            try
            {
                await SetupGitRepository(tempDir);
                return Path.Combine(tempDir, "CursorRules");
            }
            catch
            {
                CleanupTempDirectory(tempDir);
                return null;
            }
        }
        
        private static async Task SetupGitRepository(string tempDir)
        {
            await Task.Run(() => CommandRunner.RunCommand("git", "init", workingDirectory: tempDir));
            
            try
            {
                await Task.Run(() => CommandRunner.RunCommand("git", $"remote add origin {GitUrl}", workingDirectory: tempDir));
            }
            catch
            {
                try
                {
                    await Task.Run(() => CommandRunner.RunCommand("git", $"remote set-url origin {GitUrl}", workingDirectory: tempDir));
                }
                catch
                {
                    await Task.Run(() => CommandRunner.RunCommand("git", "remote remove origin", workingDirectory: tempDir));
                    await Task.Run(() => CommandRunner.RunCommand("git", $"remote add origin {GitUrl}", workingDirectory: tempDir));
                }
            }
            
            await Task.Run(() => CommandRunner.RunCommand("git", "config core.sparseCheckout true", workingDirectory: tempDir));
            File.WriteAllText(Path.Combine(tempDir, ".git", "info", "sparse-checkout"), "CursorRules/");
            await Task.Run(() => CommandRunner.RunCommand("git", $"fetch --depth=1 origin {BranchName}", workingDirectory: tempDir));
            await Task.Run(() => CommandRunner.RunCommand("git", $"checkout {BranchName}", workingDirectory: tempDir));
        }
        
        private static void CleanupTempDirectory(string tempDir)
        {
            try
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to clean up temporary directory: {ex.Message}");
            }
        }
        
        private static void ShowSuccessMessage(string source)
        {
            EditorUtility.DisplayDialog("Success", 
                $"Successfully installed entire CursorRules directory from {source} to:\n\\.cursor\\rules", 
                "OK");
            Debug.Log($"Installed entire CursorRules directory from {source} to {GetCursorRulesPath()}");
        }
        
        public static bool AreRulesInstalled()
        {
            try
            {
                string cursorRulesPath = GetCursorRulesPath();
                return Directory.Exists(cursorRulesPath) && Directory.GetFiles(cursorRulesPath, "*.mdc").Length > 0;
            }
            catch
            {
                return false;
            }
        }
        
        public static string GetInstallationPath()
        {
            try
            {
                string cursorRulesPath = GetCursorRulesPath();
                return Directory.Exists(cursorRulesPath) ? cursorRulesPath : null;
            }
            catch
            {
                return null;
            }
        }
        
        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);
            
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(destinationDir, fileName), true);
            }
            
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string subDirName = Path.GetFileName(subDir);
                CopyDirectory(subDir, Path.Combine(destinationDir, subDirName));
            }
        }
    }
}

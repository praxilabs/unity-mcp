using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityMcpBridge.Editor.Helpers
{
    public static class ServerInstaller
    {
        private const string RootFolder = "UnityMCP";
        private const string ServerFolder = "UnityMcpServer";
        private const string BranchName = "main";
        private const string GitUrl = "https://github.com/praxilabs/unity-mcp.git";
        // raw needs just the branch name, not "refs/heads"
        private const string PyprojectUrl =
            "https://raw.githubusercontent.com/praxilabs/unity-mcp/" + BranchName + "/UnityMcpServer/src/pyproject.toml";

        public static bool Initialize() {
            if (IsServerInstalled(GetSaveLocation())) {
                return true;
            }
            return false;
        }

        public static void EnsureServerInstalled() {
            try {
                string saveLocation = GetSaveLocation();
                Debug.Log($"Server installation path: {saveLocation}");
                
                if (!IsServerInstalled(saveLocation)) {
                    Debug.Log("Server not found, installing...");
                    InstallServer(saveLocation);
                } else {
                    // Check for updates and pull latest changes
                    string installedVersion = GetInstalledVersion();
                    string latestVersion = installedVersion;
                    try { 
                        latestVersion = GetLatestVersion(); 
                    } catch { 
                        Debug.LogWarning("Could not fetch latest version, skipping update check");
                    }
                    
                    if (IsNewerVersion(latestVersion, installedVersion)) {
                        Debug.Log($"New version available ({latestVersion} vs {installedVersion}), updating...");
                        UpdateServer(saveLocation);
                    } else {
                        Debug.Log("Server is up to date");
                    }
                }
            } catch (Exception ex) {
                Debug.LogError($"Failed to ensure server installation: {ex.Message}");
            }
        }

        public static string GetServerPath()
        {
            return Path.Combine(GetSaveLocation(), ServerFolder, "src");
        }

        /// <summary>
        /// Gets the platform-specific save location for the server.
        /// </summary>
        public static string GetSaveLocation()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "AppData",
                    "Local",
                    "Programs",
                    RootFolder
                );
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "bin",
                    RootFolder
                );
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string path = "/usr/local/bin";
                return !Directory.Exists(path) || !IsDirectoryWritable(path)
                    ? Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        "Applications",
                        RootFolder
                    )
                    : Path.Combine(path, RootFolder);
            }
            throw new Exception("Unsupported operating system.");
        }

        private static bool IsDirectoryWritable(string path)
        {
            try
            {
                File.Create(Path.Combine(path, "test.txt")).Dispose();
                File.Delete(Path.Combine(path, "test.txt"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the server is installed at the specified location.
        /// </summary>
        private static bool IsServerInstalled(string location)
        {
            return Directory.Exists(location)
                && File.Exists(Path.Combine(location, ServerFolder, "src", "pyproject.toml"));
        }

        /// <summary>
        /// Installs the server by cloning only the UnityMcpServer folder from the repository and setting up dependencies.
        /// </summary>
        private static void InstallServer(string location)
        {
            // Create the src directory where the server code will reside
            Directory.CreateDirectory(location);

            // Initialize git repo in the src directory
            CommandRunner.RunCommand("git", $"init", workingDirectory: location);

            // Add remote (handle case where it already exists)
            try
            {
                CommandRunner.RunCommand("git", $"remote add origin {GitUrl}", workingDirectory: location);
            }
            catch (Exception)
            {
                // Remote might already exist, try to set the URL
                try
                {
                    CommandRunner.RunCommand("git", $"remote set-url origin {GitUrl}", workingDirectory: location);
                }
                catch (Exception)
                {
                    // If that fails too, remove and re-add
                    CommandRunner.RunCommand("git", "remote remove origin", workingDirectory: location);
                    CommandRunner.RunCommand("git", $"remote add origin {GitUrl}", workingDirectory: location);
                }
            }

            // Configure sparse checkout
            CommandRunner.RunCommand("git", "config core.sparseCheckout true", workingDirectory: location);

            // Set sparse checkout path to only include UnityMcpServer folder
            string sparseCheckoutPath = Path.Combine(location, ".git", "info", "sparse-checkout");
            File.WriteAllText(sparseCheckoutPath, $"{ServerFolder}/");

            // Fetch and checkout the branch
            CommandRunner.RunCommand("git", $"fetch --depth=1 origin {BranchName}", workingDirectory: location);
            CommandRunner.RunCommand("git", $"checkout {BranchName}", workingDirectory: location);

        }

        /// <summary>
        /// Fetches the currently installed version from the local pyproject.toml file.
        /// </summary>
        public static string GetInstalledVersion()
        {
            string pyprojectPath = Path.Combine(
                GetSaveLocation(),
                ServerFolder,
                "src",
                "pyproject.toml"
            );
            return ParseVersionFromPyproject(File.ReadAllText(pyprojectPath));
        }

        /// <summary>
        /// Fetches the latest version from the GitHub pyproject.toml file.
        /// </summary>
        public static string GetLatestVersion()
        {
            using WebClient webClient = new();
            string pyprojectContent = webClient.DownloadString(PyprojectUrl);
            return ParseVersionFromPyproject(pyprojectContent);
        }

        /// <summary>
        /// Updates the server by pulling the latest changes for the UnityMcpServer folder only.
        /// </summary>
        private static void UpdateServer(string location)
        {
            CommandRunner.RunCommand("git", $"pull origin {BranchName}", workingDirectory: location);
        }

        /// <summary>
        /// Parses the version number from pyproject.toml content.
        /// </summary>
        private static string ParseVersionFromPyproject(string content)
        {
            foreach (string line in content.Split('\n'))
            {
                if (line.Trim().StartsWith("version ="))
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        return parts[1].Trim().Trim('"');
                    }
                }
            }
            throw new Exception("Version not found in pyproject.toml");
        }

        /// <summary>
        /// Compares two version strings to determine if the latest is newer.
        /// </summary>
        public static bool IsNewerVersion(string latest, string installed)
        {
            int[] latestParts = latest.Split('.').Select(int.Parse).ToArray();
            int[] installedParts = installed.Split('.').Select(int.Parse).ToArray();
            for (int i = 0; i < Math.Min(latestParts.Length, installedParts.Length); i++)
            {
                if (latestParts[i] > installedParts[i])
                {
                    return true;
                }

                if (latestParts[i] < installedParts[i])
                {
                    return false;
                }
            }
            return latestParts.Length > installedParts.Length;
        }


    }
}

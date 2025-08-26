using UnityEngine;
using UnityEditor;
using UnityMcpBridge.Editor.Helpers;

namespace UnityMcpBridge.Editor.Tools
{
    public class InstallationTool : EditorWindow
    {
        private bool isInstalling = false;
        private Vector2 scrollPosition;
        
        [MenuItem("Window/Unity MCP/Installation Manager")]
        public static void ShowWindow()
        {
            InstallationTool window = GetWindow<InstallationTool>("MCP Bridge Installation");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }
        
        private void OnEnable()
        {
            InstallationManager.OnInstallationStarted += OnInstallationStarted;
            InstallationManager.OnInstallationCompleted += OnInstallationCompleted;
        }
        
        private void OnDisable()
        {
            InstallationManager.OnInstallationStarted -= OnInstallationStarted;
            InstallationManager.OnInstallationCompleted -= OnInstallationCompleted;
        }
        
        private void OnInstallationStarted()
        {
            isInstalling = true;
            Repaint();
        }
        
        private void OnInstallationCompleted()
        {
            isInstalling = false;
            Repaint();
        }
        
        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            EditorGUILayout.Space(10);
            
            // Title
            EditorGUILayout.LabelField("Unity MCP Bridge Installation", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // Status
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Installation Status:", GUILayout.Width(120));
            
            bool isInstalled = InstallationManager.IsServerInstalled;
            string statusText = isInstalled ? "Installed" : "Not Installed";
            Color statusColor = isInstalled ? Color.green : Color.red;
            
            GUI.color = statusColor;
            EditorGUILayout.LabelField(statusText, EditorStyles.boldLabel);
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);
            
            // Installation Button
            EditorGUI.BeginDisabledGroup(isInstalling || isInstalled);
            if (GUILayout.Button("Install Server", GUILayout.Height(30)))
            {
                InstallationManager.StartInstallation();
            }
            EditorGUI.EndDisabledGroup();
            
            // Uninstall Button
            EditorGUI.BeginDisabledGroup(isInstalling || !isInstalled);
            if (GUILayout.Button("Uninstall Server", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Confirm Uninstall", 
                    "Are you sure you want to uninstall the MCP server? This will remove all server files.", 
                    "Uninstall", "Cancel"))
                {
                    InstallationManager.UninstallServer();
                }
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.Space(10);
            
            // Progress indicator
            if (isInstalling)
            {
                EditorGUILayout.LabelField("Installing server...", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Please wait while the server is being installed.");
                EditorGUILayout.Space(5);
                
                // Simple progress bar
                Rect rect = EditorGUILayout.GetControlRect(false, 20);
                EditorGUI.ProgressBar(rect, 0.5f, "Installing...");
            }
            
            EditorGUILayout.Space(10);
            
            // Information
            EditorGUILayout.LabelField("Information:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "The Unity MCP Bridge requires a Python server to be installed on your system. " +
                "This server handles communication between Unity and MCP clients like Claude Desktop or Cursor.\n\n" +
                "After installation, the main MCP Bridge tool will become available in the Tools menu.",
                MessageType.Info);
            
            EditorGUILayout.EndScrollView();
        }
    }
}

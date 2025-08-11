using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace XNodeEditor
{
    public partial class NodeEditorWindow
    {
        public VisualElement uiRoot;
        public event Action OnCloseWindow;

        public void CreateGUI()
        {
            GetUIRoot();
        }

        public void GetUIRoot()
        {
            uiRoot = rootVisualElement;
        }

        private void OnDestroy()
        {
            if(OnCloseWindow != null)
            OnCloseWindow.Invoke();
        }
    }
}
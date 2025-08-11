using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Vuplex.WebView;

public class WebViewDisplay : MonoBehaviour
{
    [SerializeField] private CanvasWebViewPrefab _canvasWebViewPrefab;
    [SerializeField] private string _initialUrl;
    [SerializeField] private bool enableFullScreenOption;

    private async void Start()
    {
        await InitializeWebView();
    }

    private async Task InitializeWebView()
    {
        if(_initialUrl == "") return;
        
        await UpdateWebViewUrl(_initialUrl);
    }

    public async Task UpdateWebViewUrl(string url)
    {
        await _canvasWebViewPrefab.WaitUntilInitialized();

#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLWebView.SetFullscreenEnabled(enableFullScreenOption);
#endif
        _canvasWebViewPrefab.WebView.LoadUrl(url);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Vuplex.WebView;

public abstract class MenuWithWebview : MenusBase, IPointerDownHandler
{
    protected static List<Canvas> _webMenuCanvases = new List<Canvas>();
    protected static List<CanvasWebViewPrefab> _canvasWebViewPrefabList = new List<CanvasWebViewPrefab>();
    private static Canvas _activeWebviewMenu;
    private const int WEBMENU_LAYER = 32767;
    [SerializeField] private CanvasWebViewPrefab _canvasWebViewPrefab;

    protected virtual void OnEnable()
    {
        RegularMenu.OnOpenNewNormalWindow += UpdateActiveWebviewMenu;

        if (!_webMenuCanvases.Contains(MenuCanvas)) _webMenuCanvases.Add(MenuCanvas);
        if (!_canvasWebViewPrefabList.Contains(_canvasWebViewPrefab)) _canvasWebViewPrefabList.Add(_canvasWebViewPrefab);

        Open();
    }

    protected virtual void OnDestroy()
    {
        RegularMenu.OnOpenNewNormalWindow -= UpdateActiveWebviewMenu;
        _webMenuCanvases.Clear();
        _canvasWebViewPrefabList.Clear();
    }

    /// <summary>
    /// Called Automatically On Enabling Object.
    /// Must be called explicitly in case of any canvas manual reactivation action eg: disable canvas then enable
    /// </summary>
    public override void Open()
    {
        CloseOtherMenus();
        UpdateWebviewMenusSortings();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateWebviewMenusSortings();
    }

    private void UpdateWebviewMenusSortings()
    {
        foreach (var item in _webMenuCanvases)
        {
            if (item == MenuCanvas)
            {
                item.sortingOrder = WEBMENU_LAYER;
            }
            else
            {
                item.sortingOrder = 0;
                Close(item);
            }
        }
    }

    private void UpdateActiveWebviewMenu()
    {
        if (_webMenuCanvases.Count > 0)
        {
            foreach (var item in _webMenuCanvases)
            {
                if (item !=null && item == _activeWebviewMenu)
                {
                    item.sortingOrder = WEBMENU_LAYER;
                }
            }

        }
    }

    private void Close(Canvas canvas)
    {
        canvas.enabled = false;
    }

    private void HideWebView(CanvasWebViewPrefab webViewGO)
    {
        webViewGO.Visible = false;
    }

    private void CloseOtherMenus()
    {
        foreach (var item in _webMenuCanvases)
        {
            if (item != MenuCanvas)
            {
                Close(item);
            }
            else
            {
                item.enabled = true;
                _activeWebviewMenu = item;
            }

        }

        foreach (var item in _canvasWebViewPrefabList)
        {
            if (item != _canvasWebViewPrefab)
            {
                HideWebView(item);
            }
            else
            {
                if (item != null)
                    item.Visible = true;
            }

        }

    }

}

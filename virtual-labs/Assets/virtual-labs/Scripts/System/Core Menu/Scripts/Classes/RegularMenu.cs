using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class RegularMenu : MenusBase ,IPointerDownHandler
{
    public static UnityAction OnOpenNewNormalWindow;
    protected virtual void OnEnable()
    {
        if (_normalMenus.Contains(MenuCanvas)) return;

        _normalMenus.Add(MenuCanvas);
        OnOpenNewNormalWindow?.Invoke();

        Open();
    }
  
    public override void Open()
    {
        UpdateMenuState(MenuState.Default);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateMenusLayers();
    }

    private void UpdateMenusLayers()
    {
        if (MenuCanvas == _normalMenus[_normalMenus.Count - 1]) return;

        if (_normalMenus.Contains(MenuCanvas))
        {
            _normalMenus.Remove(MenuCanvas);
            _normalMenus.Add(MenuCanvas);

            for (int i = 0; i < _normalMenus.Count; i++)
            {
                _normalMenus[i].sortingOrder = i;
            }
        }
    }

    private void UpdateMenuLayersOnDisabling()
    {
        for (int i = 0; i < _normalMenus.Count; i++)
        {
            _normalMenus[i].sortingOrder = i;
        }
    }

    protected virtual void OnDisable()
    {
        _normalMenus.Remove(MenuCanvas);
        UpdateMenuLayersOnDisabling();
        OnOpenNewNormalWindow?.Invoke();
    }

}

using UnityEngine;

public class BlinkingToggle : MonoBehaviour
{
    // [SerializeField] private AnimatedToggleSwitch _blinkingToggle;

    private void OnEnable()
    {
        // _blinkingToggle.OnToggleChanged += ToggleHologram;
    }

    private void OnDisable()
    {
        // _blinkingToggle.OnToggleChanged -= ToggleHologram;
    }

    private void ToggleHologram(bool toggle)
    {
        if (toggle)
            ToolsFlashManager.Instance.canFlash = true;
        else
            ToolsFlashManager.Instance.canFlash = false;
    }
}
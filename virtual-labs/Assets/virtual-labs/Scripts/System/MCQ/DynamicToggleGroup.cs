using System.Collections.Generic;
using System.Linq;
using PraxiLabs.MCQ;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class DynamicToggleGroup : MonoBehaviour
{
    public ToggleGroup ToggleGroup {get; private set;}
    private bool _hasSelection = false;
    private List<Toggle> _toggles = new();

    private void Awake()
    {
        ToggleGroup = GetComponent<ToggleGroup>();
        _toggles.AddRange(ToggleGroup.GetComponentsInChildren<Toggle>());
        foreach (var toggle in _toggles)
        {
            toggle.group = ToggleGroup; // In case toggle group wasn't assigned yet // Special case
            toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggle, isOn));
        }

        // Ensure allowSwitchOff is true initially
        ToggleGroup.allowSwitchOff = true;
    }

    private void OnToggleValueChanged(Toggle changedToggle, bool isOn)
    {
        if (isOn)
        {
            // Once a toggle is selected, disable "Allow Switch Off"
            if (!_hasSelection)
            {
                _hasSelection = true;
                ToggleGroup.allowSwitchOff = false;
            }
        }
    }

    public void AddToggle(Toggle toggle)
    {
        toggle.group = ToggleGroup;
        _toggles.Add(toggle);
        toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggle, isOn));
    }
    
    public void Reset()
    {
        // Reset all toggles to off and re-enable "Allow Switch Off"
        foreach (var toggle in _toggles)
        {
            toggle.SetIsOnWithoutNotify(false);
        }

        _hasSelection = false;
        ToggleGroup.allowSwitchOff = true;
    }


    private void OnDestroy()
    {
        // Clean up listeners to avoid memory leaks
        foreach (var toggle in _toggles)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }
}

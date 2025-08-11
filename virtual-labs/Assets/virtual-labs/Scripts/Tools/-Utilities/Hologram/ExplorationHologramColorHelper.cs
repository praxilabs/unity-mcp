using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationHologramColorHelper : Singleton<ExplorationHologramColorHelper>
{
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _explorationColor;

    public void SetDefaultColor()
    {
        List<Hologram> holograms = ExplorationHandler.Instance.GetHintsFlashableObjectHologram();
        if (holograms == null) return;
        foreach (Hologram hologram in holograms)
        {
            hologram.SetRendersColor(_defaultColor);
        }
    }

    public void SetExplorationColor()
    {
        List<Hologram> holograms = ExplorationHandler.Instance.GetHintsFlashableObjectHologram();
        if (holograms == null) return;
        foreach (Hologram hologram in holograms)
        {
            hologram.SetRendersColor(_explorationColor);
        }
    }
}

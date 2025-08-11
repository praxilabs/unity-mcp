using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafetyToolsSpawner : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _safetyToolsContainers = new List<RectTransform>();
    [SerializeField] private List<SpawnToolData> _safetyToolsPrefabs = new List<SpawnToolData>();
    [SerializeField] private float _verticalSpacing = 10f;

    private int _prefabIndex;

    private void Start()
    {
        SpawnTools();
    }

    public void SpawnTools()
    {
        _prefabIndex = 0;

        foreach (RectTransform container in _safetyToolsContainers)
        {
            List<GameObject> toolsInContainer = CollectFittingTools(container);

            CenterAndPositionTools(toolsInContainer, container);

            if (_prefabIndex >= _safetyToolsPrefabs.Count)
                return;
        }

        Debug.LogError("Not enough space to fit all safety tools!");
    }

    private List<GameObject> CollectFittingTools(RectTransform container)
    {
        float containerHeight = container.rect.height;
        float usedHeight = 0f;
        List<GameObject> toolsToFit = new List<GameObject>();

        while (_prefabIndex < _safetyToolsPrefabs.Count)
        {
            GameObject toolGO = InstantiateTool(_safetyToolsPrefabs[_prefabIndex], container);
            RectTransform toolRT = toolGO.GetComponent<RectTransform>();

            float toolHeight = toolRT.rect.height;
            float heightWithSpacing = toolHeight + _verticalSpacing;

            if (usedHeight + heightWithSpacing > containerHeight)
            {
                Destroy(toolGO);
                break;
            }

            usedHeight += heightWithSpacing;
            toolsToFit.Add(toolGO);

            RegisterIfEquippable(toolGO);
            _prefabIndex++;
        }

        return toolsToFit;
    }

    private GameObject InstantiateTool(SpawnToolData toolData, RectTransform container)
    {
        GameObject toolGO = Instantiate(toolData.safetyToolPrefab);
        toolGO.transform.SetParent(container, false);

        SafetyTool safetyTool = toolGO.GetComponent<SafetyTool>();
        safetyTool.isEquippable = toolData.isEquippable;
        safetyTool.equippedImage = toolData.equippedImage;

        LayoutRebuilder.ForceRebuildLayoutImmediate(toolGO.GetComponent<RectTransform>());
        return toolGO;
    }

    private void RegisterIfEquippable(GameObject toolGO)
    {
        SafetyTool tool = toolGO.GetComponent<SafetyTool>();
        if (tool.isEquippable)
        {
            SafetyToolsManager.Instance.equippableSafetyTools.Add(tool);
        }
    }

    private void CenterAndPositionTools(List<GameObject> tools, RectTransform container)
    {
        float totalHeight = CalculateTotalHeight(tools);
        float startY = (container.rect.height - totalHeight) / 2f;
        float currentY = startY;

        foreach (GameObject toolGO in tools)
        {
            RectTransform toolRT = toolGO.GetComponent<RectTransform>();
            float toolHeight = toolRT.rect.height;

            toolRT.anchorMin = toolRT.anchorMax = new Vector2(0, 1);
            toolRT.pivot = new Vector2(0, 1);
            toolRT.anchoredPosition = new Vector2(0, -currentY - _verticalSpacing);

            currentY += toolHeight + _verticalSpacing;
        }
    }

    private float CalculateTotalHeight(List<GameObject> tools)
    {
        float total = 0f;
        foreach (var tool in tools)
        {
            float height = tool.GetComponent<RectTransform>().rect.height;
            total += height + _verticalSpacing;
        }
        return total;
    }
}

[System.Serializable]
public struct SpawnToolData
{
    public GameObject safetyToolPrefab;
    public GameObject equippedImage;
    public bool isEquippable;
}
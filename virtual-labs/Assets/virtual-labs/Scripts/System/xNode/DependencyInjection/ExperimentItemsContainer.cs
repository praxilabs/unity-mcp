using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExperimentItemsContainer : Singleton<ExperimentItemsContainer>
{
    public List<RunTimePrefabEntry> prefabRegisteries = new List<RunTimePrefabEntry>();
    public List<GameObject> experimentItems = new List<GameObject>();

    private void Start()
    {
        _addToDontDestroyOnLoad = false;
    }

    public void RegisterObjects()
    {
        List<RegisterObject> registerObjects = new List<RegisterObject>(FindObjectsOfType<RegisterObject>(true));

        foreach (RegisterObject registerObject in registerObjects)
            registerObject.RegisterInContainer();
    }

    public void Register(RunTimePrefabEntry registry)
    {
        RegisterItems(registry);
    }

    private void RegisterItems(RunTimePrefabEntry registeredItem)
    {
        if (!prefabRegisteries.Contains(registeredItem))
        {
            prefabRegisteries.Add(registeredItem);
            foreach (var child in registeredItem.children)
                experimentItems.Add(child);
        }
    }

    public GameObject Resolve(string parent, string child)
    {
        RunTimePrefabEntry prefabEntry = prefabRegisteries
            .FirstOrDefault(entry => entry.parent.name == parent || entry.parent.name == parent + "(Clone)");

        if (prefabEntry == null)
        {
            Debug.LogWarning($"Parent GameObject '{parent}' not found in prefabRegisteries.");
            return null;
        }

        GameObject foundChild = prefabEntry.children
            .FirstOrDefault(go => go.name == child || go.name == child + "(Clone)");

        if (foundChild == null)
        {
            Debug.LogWarning($"Child GameObject '{child}' not found under parent '{parent}'.");
            return null;
        }

        return foundChild;
    }

    /// <summary>
    /// This is used to find a type that has only one instance in scene or in registry,
    /// if there is 0 or more than one, this will return null.
    /// </summary>
    public T Resolve<T>() where T : Component
    {
        var matchingObjects = experimentItems
        .Where(go => go.GetComponent<T>() != null)
        .ToList();

        if (matchingObjects.Count == 1)
            return matchingObjects[0].GetComponent<T>();

        if (matchingObjects.Count > 1)
            Debug.LogError($"Multiple GameObjects found with component of type '{typeof(T)}'. Resolve<T>() expects exactly one.");
        else
            Debug.LogWarning($"No GameObject found with component of type '{typeof(T)}'.");

        return null;
    }

    public void ClearStagePrefabRegistery(List<GameObject> stagePrefabRegistery, string parent)
    {
        RunTimePrefabEntry prefabEntry = prefabRegisteries
                                        .FirstOrDefault(entry => entry.parent.name == parent || entry.parent.name == parent + "(Clone)");

        if (prefabEntry != null)
            prefabRegisteries.Remove(prefabEntry);

        experimentItems = experimentItems.Except(stagePrefabRegistery).ToList();
    }
}

[Serializable]
public class RunTimePrefabEntry
{
    public GameObject parent;
    public List<GameObject> children = new List<GameObject>();
}
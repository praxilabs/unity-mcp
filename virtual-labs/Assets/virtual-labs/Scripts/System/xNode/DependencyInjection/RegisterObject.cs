using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegisterObject : MonoBehaviour
{
    public enum RegistrationType { Single, WholeHierarchy };

    public ExperimentItemsRegistry registryData;

    [SerializeField] private RegistrationType _registrationType;

    private readonly HashSet<string> _excludedNamespaces = new HashSet<string>
    {
        "UnityEngine.UI",
    };

    private readonly HashSet<Type> _excludedComponentTypes = new HashSet<Type>
    {
        typeof(Transform),
        typeof(RectTransform),
        typeof(Canvas),
        typeof(MeshRenderer),
        typeof(MeshFilter),
        typeof(SpriteRenderer),
        typeof(CanvasRenderer),
        typeof(RegisterObject),
    };

    public List<GameObject> _registeredGameObjects = new List<GameObject>();
   
    private Dictionary<string, int> _objectNameCounts = new Dictionary<string, int>();
    private HashSet<string> _duplicateNames = new HashSet<string>();

    private bool _isRegistered = false;

    private void Awake()
    {
        RegisterInContainer();
    }

    public void ClearStagePrefabRegistery()
    {
        ExperimentItemsContainer.Instance.ClearStagePrefabRegistery(_registeredGameObjects, this.name);
    }

    public void RegisterInContainer()
    {
        if(_isRegistered) return;

        RunTimePrefabEntry registry = new();

        registry.parent = this.gameObject;
        registry.children = _registeredGameObjects;

        ExperimentItemsContainer.Instance.Register(registry);
        _isRegistered = true;
    }

    public void RegisterPreparation()
    {
        foreach (var prefabRegistry in registryData.prefabRegisteries)
        {
            if (prefabRegistry.prefabName == this.name)
            {
                prefabRegistry.prefabChildren.Clear();
                _registeredGameObjects.Clear();

                // Count object name occurrences
                _objectNameCounts.Clear();
                _duplicateNames.Clear();
                CountObjectNames(this.gameObject);

                StartRegistration(prefabRegistry, this.gameObject);

                // Custom sorting: Keep this.gameObject on top and sort the rest alphabetically
                prefabRegistry.prefabChildren = prefabRegistry.prefabChildren
                    .OrderBy(entry => entry.childName == this.gameObject.name ? 0 : 1)
                    .ThenBy(entry => entry.childName)
                    .ToList();

                return;
            }
        }

        var newEntry = new PrefabEntry
        {
            prefabName = this.name,
            prefabChildren = new List<ChildEntry>()
        };

        // Count object name occurrences
        _objectNameCounts.Clear();
        _duplicateNames.Clear();
        CountObjectNames(this.gameObject);

        StartRegistration(newEntry, this.gameObject);

        // Custom sorting: Keep parent on top and sort the rest alphabetically
        newEntry.prefabChildren = newEntry.prefabChildren
            .OrderBy(entry => entry.childName == this.gameObject.name ? 0 : 1)
            .ThenBy(entry => entry.childName)
            .ToList();

        registryData.prefabRegisteries.Add(newEntry);
    }

    private void CountObjectNames(GameObject rootObject)
    {
        foreach (Transform child in rootObject.GetComponentsInChildren<Transform>(true))
        {
            string objectName = child.gameObject.name;

            if (!_objectNameCounts.ContainsKey(objectName))
            {
                _objectNameCounts[objectName] = 0;
            }
            _objectNameCounts[objectName]++;

            if (_objectNameCounts[objectName] > 1)
            {
                _duplicateNames.Add(objectName);
            }
        }
    }

    private void StartRegistration(PrefabEntry prefabEntry, GameObject currentObject)
    {
        if(_registrationType == RegistrationType.Single)
            SingleEntry(prefabEntry, currentObject);
        else if(_registrationType == RegistrationType.WholeHierarchy)
            MultipleEntries(prefabEntry, currentObject);
    }

    private void SingleEntry(PrefabEntry prefabEntry, GameObject currentObject)
    {

        List<Component> components = currentObject.GetComponents<Component>().ToList();

        var entry = new ChildEntry
        {
            childName = currentObject.name,
            childComponents = components.Select(c => c.GetType().FullName).ToList()
        };

        prefabEntry.prefabChildren.Add(entry);
        _registeredGameObjects.Add(currentObject);
    }

    private void MultipleEntries(PrefabEntry prefabEntry, GameObject currentObject)
    {
        bool canSkip = false;

        if (_duplicateNames.Contains(currentObject.name))
            canSkip = true;

        if (!canSkip)
        {
            var components = currentObject.GetComponents<Component>()
                .Where(c => !(c is Transform)) // Exclude Transform component
                .ToList();

            var meaningfulComponents = components
                .Where(c => !_excludedComponentTypes.Contains(c.GetType())) // Exclude specific types
                .Where(c => !IsInExcludedNamespace(c.GetType())) // Exclude based on namespace
                .ToList();

            if (meaningfulComponents.Count > 0)
            {
                var entry = new ChildEntry
                {
                    childName = currentObject.name,
                    childComponents = components.Select(c => c.GetType().FullName).ToList()
                };
                prefabEntry.prefabChildren.Add(entry);
                _registeredGameObjects.Add(currentObject);
            }
        }

        // Recursively process all children, including disabled GameObjects
        foreach (Transform child in currentObject.transform)
        {
            MultipleEntries(prefabEntry, child.gameObject);
        }
    }

    private bool IsInExcludedNamespace(Type type)
    {
        return _excludedNamespaces.Contains(type.Namespace);
    }
}

[System.Serializable]
public class RegistryItem
{
    public string prefabName;
    public string childName;
}
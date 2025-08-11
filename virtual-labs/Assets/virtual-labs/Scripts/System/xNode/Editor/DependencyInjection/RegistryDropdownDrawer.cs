using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RegistryDropdownAttribute))]
public class RegistryDropdownDrawer : PropertyDrawer
{
    private Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

    private ExperimentItemsRegistry _registryData;
    private SerializedProperty _prefabProperty;
    private SerializedProperty _childProperty;

    private string _selectedPrefab;
    private float _dropdownWidth;
    private Rect _prefabDropdownPosition;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the RegistryDropdown attribute
        RegistryDropdownAttribute attribute = (RegistryDropdownAttribute)this.attribute;

        if (_prefabProperty == null)
            _prefabProperty = property.FindPropertyRelative("prefabName");
        if (_childProperty == null)
            _childProperty = property.FindPropertyRelative("childName");

        if (_prefabProperty == null || _childProperty == null)
        {
            EditorGUI.LabelField(position, label.text, "Invalid property structure");
            return;
        }

        StepNode node = property.serializedObject.targetObject as StepNode;

        if (node == null) return;

        var graph = node.graph as StepsGraph;
        if (graph == null || graph.registryData == null)
        {
            EditorGUI.LabelField(position, label.text, "Graph or registryData not set");
            return;
        }

        _registryData = graph.registryData;

        if (_registryData.prefabRegisteries == null || _registryData.prefabRegisteries.Count == 0)
        {
            EditorGUI.LabelField(position, label.text, "No prefab registries available");
            return;
        }

        _dropdownWidth = position.width;

        DisplayPrefabDropDown(position);
        DisplayChildrenDropDown(position, _childProperty, attribute);
    }

    private void DisplayPrefabDropDown(Rect position)
    {
        _prefabDropdownPosition = new Rect(position.x, position.y, _dropdownWidth, EditorGUIUtility.singleLineHeight);

        // Prefab Dropdown
        List<string> prefabNames = _registryData.prefabRegisteries.Select(entry => entry.prefabName).ToList();

        if (string.IsNullOrEmpty(_prefabProperty.stringValue) || !prefabNames.Contains(_prefabProperty.stringValue))
        {
            _prefabProperty.stringValue = prefabNames.FirstOrDefault(); // Assign first valid value
        }

        int prefabIndex = prefabNames.IndexOf(_prefabProperty.stringValue);
        prefabIndex = Mathf.Max(0, prefabIndex);

        EditorGUI.BeginChangeCheck();
        prefabIndex = EditorGUI.Popup(_prefabDropdownPosition, "Objects parent", prefabIndex, prefabNames.ToArray());
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_prefabProperty.serializedObject.targetObject, "Change Prefab Selection");
            _prefabProperty.stringValue = prefabNames[prefabIndex];
            _prefabProperty.serializedObject.ApplyModifiedProperties();  // Apply change
        }
    }


    private void DisplayChildrenDropDown(Rect position, SerializedProperty property, RegistryDropdownAttribute attribute)
    {
        Rect ChildrenDropdownPosition = new Rect(position.x, _prefabDropdownPosition.y + EditorGUIUtility.singleLineHeight + 2, _dropdownWidth, EditorGUIUtility.singleLineHeight);
        Type filterType = attribute.ComponentType;

        PrefabEntry selectedPrefabEntry = _registryData.prefabRegisteries.FirstOrDefault(entry => entry.prefabName == _prefabProperty.stringValue);
        if (selectedPrefabEntry == null || selectedPrefabEntry.prefabChildren == null || selectedPrefabEntry.prefabChildren.Count == 0)
        {
            EditorGUI.LabelField(ChildrenDropdownPosition, property.displayName, "No children available");
            return;
        }

        List<string> childrenNames = new List<string> { "(none)" };
        childrenNames.AddRange(
            filterType == typeof(GameObject)
                ? selectedPrefabEntry.prefabChildren.Select(entry => entry.childName)
                : selectedPrefabEntry.prefabChildren
                    .Where(entry => entry.childComponents.Any(componentTypeName => filterType.IsAssignableFrom(GetType(componentTypeName))))
                    .Select(entry => entry.childName)
        );

        // Ensure valid selection
        if (string.IsNullOrEmpty(_childProperty.stringValue) || !childrenNames.Contains(_childProperty.stringValue))
        {
            _childProperty.stringValue = "(none)"; // Default to "(none)" instead of resetting
        }

        int childIndex = childrenNames.IndexOf(_childProperty.stringValue);
        childIndex = Mathf.Max(0, childIndex);

        EditorGUI.BeginChangeCheck();
        childIndex = EditorGUI.Popup(ChildrenDropdownPosition, attribute.DisplayName, childIndex, childrenNames.ToArray());
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_childProperty.serializedObject.targetObject, "Change Child Selection");
            _childProperty.stringValue = childrenNames[childIndex];
            _childProperty.serializedObject.ApplyModifiedProperties();
        }
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight + 2) * 2 + EditorGUIUtility.singleLineHeight;
    }

    private Type GetType(string typeName)
    {
        if (_typeCache.TryGetValue(typeName, out Type type))
        {
            return type;
        }

        type = Type.GetType(typeName, throwOnError: false);
        if (type == null)
        {
            type = AppDomain.CurrentDomain.GetAssemblies()
                             .Select(assembly => assembly.GetType(typeName, throwOnError: false))
                             .FirstOrDefault(t => t != null);
        }

        _typeCache[typeName] = type;
        return type;
    }
}
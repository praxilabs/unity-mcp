using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

public class FunctionCallBaseEditor : StepNodeEditor
{
    protected FunctionCallBase _functionCall;
    protected ExperimentItemsRegistry _registryData;

    protected MethodInfo _selectedFunction;
    protected bool _functionSelectionChanged;

    private GUIStyle _titleStyle;
    protected GUIStyle TitleStyle =>
                _titleStyle ??= SetTitleStyle();

    public override void OnBodyGUI(XNode.Node currentNode)
    {
        if ((target as XNode.Node).groupCollapsed) return;

        _functionCall = currentNode as FunctionCallBase;
        StepsGraph graph = _functionCall.graph as StepsGraph;
        _registryData = graph.registryData;

        using (var so = new EditorGUI.ChangeCheckScope())
        {
            serializedObject.Update();

            ValidateCalledObjectSelection();
            DrawNodeFields(currentNode);

            if (_functionCall.nodeCollapsed)
            {
                DisplayDropDownMenus();
                DrawPorts(_functionCall);
            }
            DisplayNodeFieldsButton(currentNode);

            if (so.changed)
                serializedObject.ApplyModifiedProperties();
        }
    }

    protected void ValidateCalledObjectSelection()
    {
        if (_functionCall.prefabName == _functionCall.previousCalledObjectName) return;

        _functionCall.ResetSelection();
    }

    // Display the components dropdown menu
    // Display the methods dropdown menu if a component is selected
    protected void DisplayDropDownMenus()
    {
        if (!string.IsNullOrEmpty(_functionCall.childName))
            DisplayComponentsDropdown();

        if (!string.IsNullOrEmpty(_functionCall.selectedComponent))
            DisplayMethodsDropdown();

        if (!string.IsNullOrEmpty(_functionCall.selectedFunction))
            DisplayParameters();

        if (_functionSelectionChanged)
            _functionSelectionChanged = false;
    }

    protected void DisplayComponentsDropdown()
    {
        if (_functionCall.prefabName == null)
        {
            EditorGUILayout.HelpBox("No GameObject selected.", UnityEditor.MessageType.Warning);
            return;
        }

        var prefab = _registryData.prefabRegisteries
            .FirstOrDefault(p => p.prefabName == _functionCall.prefabName);

        if (prefab == null) return;

        var child = prefab.prefabChildren
            .FirstOrDefault(c => c.childName == _functionCall.childName);

        if (child == null) return;

        string[] componentNames = child.childComponents.ToArray();

        int selectedIndex = Array.FindIndex(componentNames, name => name == _functionCall.selectedComponent);

        EditorGUILayout.BeginVertical(DarkBoxStyle);
        EditorGUILayout.LabelField("Components", TitleStyle);

        int newSelectedIndex = EditorGUILayout.Popup(selectedIndex, componentNames);
        EditorGUILayout.EndVertical();

        if (newSelectedIndex != selectedIndex)
        {
            _functionCall.selectedComponent = newSelectedIndex >= 0 ? componentNames[newSelectedIndex] : null;
            _functionCall.selectedFunction = null;
            _functionCall.parameterValues = null;
        }
    }
    protected virtual void DisplayMethodsDropdown()
    {
        if (string.IsNullOrEmpty(_functionCall.selectedComponent))
        {
            EditorGUILayout.HelpBox("No component selected.", UnityEditor.MessageType.Warning);
            return;
        }

        Type componentType = FindTypeByName(_functionCall.selectedComponent);

        MethodInfo[] methods = GetMethodInfos(componentType);
        string[] methodNames = GetMethodNames(componentType);

        string selectedFunctionName = _functionCall.selectedFunction;
        // Find index of the currently selected method
        int selectedIndex = Array.FindIndex(methodNames, name =>
        {
            if (_functionCall.selectedFunction == null || !name.Contains(_functionCall.selectedFunction))
            {
                return false;
            }
            else
            {
                return true;
            }
        });

        // Display the dropdown
        EditorGUILayout.BeginVertical(DarkBoxStyle);
        EditorGUILayout.LabelField("Methods", TitleStyle);
        int newSelectedIndex = EditorGUILayout.Popup(selectedIndex, methodNames);
        EditorGUILayout.EndVertical();

        if (newSelectedIndex > -1)
            _selectedFunction = methods[newSelectedIndex];

        // Handle selection change
        if (newSelectedIndex != selectedIndex)
        {
            _functionSelectionChanged = true;

            _functionCall.selectedFunction = newSelectedIndex >= 0 ? _selectedFunction.Name : null;

            // Update parameters if a method is selected
            if (!string.IsNullOrEmpty(_functionCall.selectedFunction))
            {
                _selectedFunction = methods[newSelectedIndex];
                UpdateMethodParameters(_selectedFunction);
            }
        }
    }

    private Type FindTypeByName(string typeName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType(typeName))
            .FirstOrDefault(t => t != null);
    }

    private void UpdateMethodParameters(MethodInfo methodInfo)
    {
        ParameterInfo[] parameters = methodInfo.GetParameters();
        _functionCall.parameterValues = new SerializableParameter[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            Type paramType = parameters[i].ParameterType;
            InitializeParameterValues(paramType, i);
        }
    }

    // Initialize default values for each parameter
    protected void InitializeParameterValues(Type paramType, int parameterIndex)
    {
        SerializableParameter newParameter = new SerializableParameter();

        if (paramType == typeof(int))
            newParameter.SetValue(0);
        else if (paramType == typeof(float))
            newParameter.SetValue(0f);
        else if (paramType == typeof(string))
            newParameter.SetValue(string.Empty);
        else if (paramType == typeof(bool))
            newParameter.SetValue(false);
        else if (paramType == typeof(Vector3))
            newParameter.SetValue(Vector3.zero);
        else if (paramType.IsEnum)
        {
            Enum firstEnumValue = (Enum)Enum.GetValues(paramType).GetValue(0);
            newParameter.SetEnumValue(firstEnumValue);
        }
        else if (typeof(UnityEngine.Object).IsAssignableFrom(paramType))
            newParameter.SetValue(null);

        _functionCall.parameterValues[parameterIndex] = newParameter;
    }

    protected virtual string[] GetMethodNames(Type componentType)
    {
        return componentType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(m => m.ReturnType != typeof(System.Collections.IEnumerator))
            .Where(m => !m.IsSpecialName) // Exclude property methods (getters/setters)
            .Select(m => $"{m.Name} ({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))})")
            .ToArray();
    }


    protected virtual MethodInfo[] GetMethodInfos(Type componentType)
    {
        return componentType
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(m => m.ReturnType != typeof(System.Collections.IEnumerator))
                        .Where(m => !m.IsSpecialName)
                        .ToArray();
    }

    protected MethodInfo GetMethodInfo(MonoBehaviour component, string methodName)
    {
        return component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
    }

    protected virtual void DisplayParameters()
    {
        if (_selectedFunction == null)
            return;

        ParameterInfo[] parameters = _selectedFunction.GetParameters();

        // Initialize the parameterValues array if it's null or doesn't match the parameter length
        if (_functionCall.parameterValues == null || _functionCall.parameterValues.Length != parameters.Length)
        {
            _functionCall.parameterValues = new SerializableParameter[parameters.Length];
        }

        if (parameters.Length <= 0) return;

        EditorGUILayout.BeginVertical(DarkBoxStyle); // 🟦 Boxed UI for better grouping
        EditorGUILayout.LabelField("Function Parameters", TitleStyle); // 🔹 Bold title

        for (int i = 0; i < parameters.Length; i++)
        {
            if (_functionCall.parameterValues[i] == null)
            {
                _functionCall.parameterValues[i] = new SerializableParameter();
            }

            Type paramType = parameters[i].ParameterType;

            if (parameters[i].ParameterType.IsEnum)
                _functionCall.parameterValues[i].typeName = parameters[i].ParameterType.BaseType.ToString();
            else
                _functionCall.parameterValues[i].typeName = paramType.Name;

            object value = _functionCall.parameterValues[i].GetValue();

            // Draw fields based on parameter type
            EditorGUILayout.BeginHorizontal(); // 🔹 Better spacing inside box

            if (paramType == typeof(int))
                _functionCall.parameterValues[i].SetValue(EditorGUILayout.IntField(parameters[i].Name, (int)value));
            else if (paramType == typeof(float))
                _functionCall.parameterValues[i].SetValue(EditorGUILayout.FloatField(parameters[i].Name, (float)value));
            else if (paramType == typeof(string))
                _functionCall.parameterValues[i].SetValue(EditorGUILayout.TextField(parameters[i].Name, (string)value));
            else if (paramType == typeof(bool))
                _functionCall.parameterValues[i].SetValue(EditorGUILayout.Toggle(parameters[i].Name, (bool)value));
            else if (paramType == typeof(Vector3))
                _functionCall.parameterValues[i].SetValue(EditorGUILayout.Vector3Field(parameters[i].Name, (Vector3)value));
            else if (paramType.IsEnum)
                _functionCall.parameterValues[i].SetValue(EditorGUILayout.EnumPopup(parameters[i].Name, (Enum)value));
            else if (typeof(UnityEngine.Object).IsAssignableFrom(paramType))
                _functionCall.parameterValues[i].SetValue(EditorGUILayout.ObjectField(parameters[i].Name, value as UnityEngine.Object, paramType, true));
            else
                EditorGUILayout.LabelField(parameters[i].Name + ": Unsupported parameter type " + paramType.Name);

            EditorGUILayout.EndHorizontal(); // Close horizontal layout
        }

        EditorGUILayout.EndVertical(); // Close box layout
    }


    private GUIStyle SetTitleStyle()
    {
        return new GUIStyle(EditorStyles.label)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            normal = { textColor = StyleHelperxNode.ConvertHexColor("#FFB433") }
        };
    }
}
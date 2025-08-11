using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

[CustomNodeEditor(typeof(FunctionCallCoroutineGlobalStep))]
public class FunctionCallCoroutineGlobalEditor : FunctionCallCoroutineEditor
{
    protected override string[] GetMethodNames(Type componentType)
    {
        // Use reflection to get all public instance methods of the component's type
        return componentType
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(m => m.ReturnType == typeof(System.Collections.IEnumerator))
                        .Where(m => !m.IsSpecialName) // Exclude property methods (getters/setters)
                        .Select(m => m.Name)
                        .ToArray();
    }

    protected override MethodInfo[] GetMethodInfos(Type componentType)
    {
        return componentType
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(m => m.ReturnType == typeof(System.Collections.IEnumerator))
                        .Where(m => !m.IsSpecialName)
                        .ToArray();
    }
}
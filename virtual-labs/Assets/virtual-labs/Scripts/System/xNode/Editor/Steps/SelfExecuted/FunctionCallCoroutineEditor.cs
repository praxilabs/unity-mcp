using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

[CustomNodeEditor(typeof(FunctionCallCoroutineStep))]
public class FunctionCallCoroutineEditor : FunctionCallBaseEditor
{
    protected override string[] GetMethodNames(Type componentType)
    {
        // Use reflection to get all public instance methods of the component's type
        return componentType
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(m => m.ReturnType == typeof(System.Collections.IEnumerator))
                        .Where(m => !m.IsSpecialName) // Exclude property methods (getters/setters)
                        .Select(m => $"{m.Name} ({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))})")
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
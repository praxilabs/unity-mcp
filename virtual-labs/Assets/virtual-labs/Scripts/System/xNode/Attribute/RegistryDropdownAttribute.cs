using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class RegistryDropdownAttribute : PropertyAttribute
{
    public Type ComponentType { get; }
    public string DisplayName { get; set; }

    public RegistryDropdownAttribute(Type componentType, string displayName = "")
    {
        ComponentType = componentType;
        DisplayName = displayName;
    }
}
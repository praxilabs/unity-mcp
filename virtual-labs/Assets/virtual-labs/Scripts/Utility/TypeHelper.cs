using System.Collections.Generic;
using System;
using UnityEngine;

public static class TypeHelper
{
    private static readonly Dictionary<Type, string> FriendlyTypeNames = new Dictionary<Type, string>
    {
        { typeof(bool), "bool" },
        { typeof(int), "int" },
        { typeof(float), "float" },
        { typeof(double), "double" },
        { typeof(string), "string" },
        { typeof(Vector3), "Vector3" },
    };

    public static string GetFriendlyTypeName(Type type)
    {
        if (FriendlyTypeNames.TryGetValue(type, out string friendlyName))
            return friendlyName;

        // Default to the type's name if not found in the dictionary
        return type.Name;
    }
}

public static class TypeExtensions
{
    public static string GetFriendlyName(this Type type)
    {
        return TypeHelper.GetFriendlyTypeName(type);
    }
}
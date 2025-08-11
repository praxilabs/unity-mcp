using System;
using UnityEngine;

/// <summary>
/// Add this attribute to a field in Variable Node to make it a global variable
/// Note: Only works in Variable Node
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class GlobalVariableAttribute : PropertyAttribute { }
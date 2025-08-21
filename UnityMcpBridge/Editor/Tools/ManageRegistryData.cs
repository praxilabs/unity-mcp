using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using XNode;

public static class ManageRegistryData
{
    public static object HandleListParents(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();

        if (string.IsNullOrEmpty(graphPath))
        {
            return new
            {
                success = false,
                                    error = "Missing required argument: graphPath"
            };
        }

        try
        {
            // Load the graph asset as base type to avoid compile-time dependency
            var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            // Get the registry data from the graph using SerializedObject
            var registryData = GetRegistryDataFromGraph(graph);
            if (registryData == null)
            {
                return new
                {
                    success = false,
                    error = "Could not access registry data from graph (is it a StepsGraph?)"
                };
            }

            // Get prefab registries using reflection
            var prefabRegistries = GetPrefabRegistries(registryData);
            if (prefabRegistries == null)
            {
                return new
                {
                    success = false,
                    error = "No registry data found in the graph"
                };
            }

            // Extract parent names (prefab names) using reflection
            var parents = new List<string>();
            var enumerable = prefabRegistries as System.Collections.IEnumerable;
            if (enumerable != null)
            {
                foreach (var entry in enumerable)
                {
                    var prefabNameField = entry.GetType().GetField("prefabName");
                    if (prefabNameField != null)
                    {
                        var prefabName = prefabNameField.GetValue(entry) as string;
                        if (!string.IsNullOrEmpty(prefabName))
                        {
                            parents.Add(prefabName);
                        }
                    }
                }
            }

            parents.Sort();
            var parentsArray = parents.ToArray();

            return new
            {
                success = true,
                message = $"Found {parentsArray.Length} parent objects in registry",
                graphPath = graphPath,
                parents = parentsArray,
                count = parentsArray.Length,
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = $"Failed to list parents: {ex.Message}"
            };
        }
    }

    public static object HandleListAll(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();

        if (string.IsNullOrEmpty(graphPath))
        {
            return new
            {
                success = false,
                                    error = "Missing required argument: graphPath"
            };
        }

        try
        {
            // Load the graph asset as base type to avoid compile-time dependency
            var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            // Get the registry data from the graph using SerializedObject
            var registryData = GetRegistryDataFromGraph(graph);
            if (registryData == null)
            {
                return new
                {
                    success = false,
                    error = "Could not access registry data from graph (is it a StepsGraph?)"
                };
            }

            // Get prefab registries using reflection
            var prefabRegistries = GetPrefabRegistries(registryData);
            if (prefabRegistries == null)
            {
                return new
                {
                    success = false,
                    error = "No registry data found in the graph"
                };
            }

            // Flatten the list: create a list of all parent-child combinations
            var allItems = new List<object>();
            var enumerable = prefabRegistries as System.Collections.IEnumerable;
            
            if (enumerable != null)
            {
                foreach (var prefabEntry in enumerable)
                {
                    var prefabNameField = prefabEntry.GetType().GetField("prefabName");
                    var prefabChildrenField = prefabEntry.GetType().GetField("prefabChildren");
                    
                    if (prefabNameField != null)
                    {
                        var prefabName = prefabNameField.GetValue(prefabEntry) as string;
                        if (string.IsNullOrEmpty(prefabName)) continue;

                        // Add the parent itself
                        allItems.Add(new
                        {
                            parent = prefabName,
                            child = prefabName,
                            isParent = true,
                            components = new string[0] // Parent has no specific components
                        });

                        // Add all children
                        if (prefabChildrenField != null)
                        {
                            var children = prefabChildrenField.GetValue(prefabEntry) as System.Collections.IEnumerable;
                            if (children != null)
                            {
                                foreach (var childEntry in children)
                                {
                                    var childNameField = childEntry.GetType().GetField("childName");
                                    var childComponentsField = childEntry.GetType().GetField("childComponents");
                                    
                                    if (childNameField != null)
                                    {
                                        var childName = childNameField.GetValue(childEntry) as string;
                                        if (string.IsNullOrEmpty(childName)) continue;

                                        var components = new List<string>();
                                        if (childComponentsField != null)
                                        {
                                            var childComponents = childComponentsField.GetValue(childEntry) as System.Collections.IEnumerable;
                                            if (childComponents != null)
                                            {
                                                foreach (var component in childComponents)
                                                {
                                                    if (component != null)
                                                        components.Add(component.ToString());
                                                }
                                            }
                                        }

                                        allItems.Add(new
                                        {
                                            parent = prefabName,
                                            child = childName,
                                            isParent = false,
                                            components = components.ToArray()
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new
            {
                success = true,
                message = $"Found {allItems.Count} total items in registry (flattened)",
                graphPath = graphPath,
                items = allItems.ToArray(),
                count = allItems.Count,
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = $"Failed to list all items: {ex.Message}"
            };
        }
    }

    public static object HandleListChildren(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();
        string parentName = args?["parentName"]?.ToString();

        if (string.IsNullOrEmpty(graphPath))
        {
            return new
            {
                success = false,
                                    error = "Missing required argument: graphPath"
            };
        }

        if (string.IsNullOrEmpty(parentName))
        {
            return new
            {
                success = false,
                                    error = "Missing required argument: parentName"
            };
        }

        try
        {
            // Load the graph asset as base type to avoid compile-time dependency
            var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            // Get the registry data from the graph using SerializedObject
            var registryData = GetRegistryDataFromGraph(graph);
            if (registryData == null)
            {
                return new
                {
                    success = false,
                    error = "Could not access registry data from graph (is it a StepsGraph?)"
                };
            }

            // Get prefab registries using reflection
            var prefabRegistries = GetPrefabRegistries(registryData);
            if (prefabRegistries == null)
            {
                return new
                {
                    success = false,
                    error = "No registry data found in the graph"
                };
            }

            // Find the specific parent
            object parentEntry = null;
            var enumerable = prefabRegistries as System.Collections.IEnumerable;
            if (enumerable != null)
            {
                foreach (var entry in enumerable)
                {
                    var prefabNameField = entry.GetType().GetField("prefabName");
                    if (prefabNameField != null)
                    {
                        var prefabName = prefabNameField.GetValue(entry) as string;
                        if (string.Equals(prefabName, parentName, StringComparison.OrdinalIgnoreCase))
                        {
                            parentEntry = entry;
                            break;
                        }
                    }
                }
            }

            if (parentEntry == null)
            {
                return new
                {
                    success = false,
                    error = $"Parent '{parentName}' not found in registry"
                };
            }

            // Extract children
            var children = new List<object>();
            var prefabChildrenField = parentEntry.GetType().GetField("prefabChildren");
            if (prefabChildrenField != null)
            {
                var childrenEnumerable = prefabChildrenField.GetValue(parentEntry) as System.Collections.IEnumerable;
                if (childrenEnumerable != null)
                {
                    foreach (var childEntry in childrenEnumerable)
                    {
                        var childNameField = childEntry.GetType().GetField("childName");
                        var childComponentsField = childEntry.GetType().GetField("childComponents");
                        
                        if (childNameField != null)
                        {
                            var childName = childNameField.GetValue(childEntry) as string;
                            if (string.IsNullOrEmpty(childName)) continue;

                            var components = new List<string>();
                            if (childComponentsField != null)
                            {
                                var childComponents = childComponentsField.GetValue(childEntry) as System.Collections.IEnumerable;
                                if (childComponents != null)
                                {
                                    foreach (var component in childComponents)
                                    {
                                        if (component != null)
                                            components.Add(component.ToString());
                                    }
                                }
                            }

                            children.Add(new
                            {
                                childName = childName,
                                components = components.ToArray()
                            });
                        }
                    }
                }
            }

            // Sort children by name
            children = children.OrderBy(c => 
            {
                var childNameProperty = c.GetType().GetProperty("childName");
                return childNameProperty?.GetValue(c) as string ?? "";
            }).ToList();

            return new
            {
                success = true,
                message = $"Found {children.Count} children for parent '{parentName}'",
                graphPath = graphPath,
                parentName = parentName,
                children = children.ToArray(),
                count = children.Count,
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = $"Failed to list children: {ex.Message}"
            };
        }
    }

    public static object HandleGetChildComponents(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();
        string parentName = args?["parentName"]?.ToString();
        string childName = args?["childName"]?.ToString();

        if (string.IsNullOrEmpty(graphPath))
        {
            return new
            {
                success = false,
                error = "Missing required argument: graphPath"
            };
        }

        if (string.IsNullOrEmpty(parentName))
        {
            return new
            {
                success = false,
                error = "Missing required argument: parentName"
            };
        }

        if (string.IsNullOrEmpty(childName))
        {
            return new
            {
                success = false,
                error = "Missing required argument: childName"
            };
        }

        try
        {
            // Load the graph asset as base type to avoid compile-time dependency
            var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            // Get the registry data from the graph using SerializedObject
            var registryData = GetRegistryDataFromGraph(graph);
            if (registryData == null)
            {
                return new
                {
                    success = false,
                    error = "Could not access registry data from graph (is it a StepsGraph?)"
                };
            }

            // Get prefab registries using reflection
            var prefabRegistries = GetPrefabRegistries(registryData);
            if (prefabRegistries == null)
            {
                return new
                {
                    success = false,
                    error = "No registry data found in the graph"
                };
            }

            // Find the specific parent
            object parentEntry = null;
            var enumerable = prefabRegistries as System.Collections.IEnumerable;
            if (enumerable != null)
            {
                foreach (var entry in enumerable)
                {
                    var prefabNameField = entry.GetType().GetField("prefabName");
                    if (prefabNameField != null)
                    {
                        var prefabName = prefabNameField.GetValue(entry) as string;
                        if (string.Equals(prefabName, parentName, StringComparison.OrdinalIgnoreCase))
                        {
                            parentEntry = entry;
                            break;
                        }
                    }
                }
            }

            if (parentEntry == null)
            {
                return new
                {
                    success = false,
                    error = $"Parent '{parentName}' not found in registry"
                };
            }

            // Find the specific child and get its components
            var components = new List<string>();
            var prefabChildrenField = parentEntry.GetType().GetField("prefabChildren");
            if (prefabChildrenField != null)
            {
                var childrenEnumerable = prefabChildrenField.GetValue(parentEntry) as System.Collections.IEnumerable;
                if (childrenEnumerable != null)
                {
                    foreach (var childEntry in childrenEnumerable)
                    {
                        var childNameField = childEntry.GetType().GetField("childName");
                        if (childNameField != null)
                        {
                            var currentChildName = childNameField.GetValue(childEntry) as string;
                            if (string.Equals(currentChildName, childName, StringComparison.OrdinalIgnoreCase))
                            {
                                // Found the child, now get its components
                                var childComponentsField = childEntry.GetType().GetField("childComponents");
                                if (childComponentsField != null)
                                {
                                    var childComponents = childComponentsField.GetValue(childEntry) as System.Collections.IEnumerable;
                                    if (childComponents != null)
                                    {
                                        foreach (var component in childComponents)
                                        {
                                            if (component != null)
                                                components.Add(component.ToString());
                                        }
                                    }
                                }
                                break; // Found the child, no need to continue
                            }
                        }
                    }
                }
            }

            if (components.Count == 0)
            {
                return new
                {
                    success = false,
                    error = $"Child '{childName}' not found in parent '{parentName}' or has no components"
                };
            }

            return new
            {
                success = true,
                message = $"Found {components.Count} components for child '{childName}' in parent '{parentName}'",
                graphPath = graphPath,
                parentName = parentName,
                childName = childName,
                components = components.ToArray(),
                count = components.Count,
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = $"Failed to get child components: {ex.Message}"
            };
        }
    }

    public static object HandleGetComponentMethods(JObject args)
    {
        string componentTypeName = args?["componentTypeName"]?.ToString();

        if (string.IsNullOrEmpty(componentTypeName))
        {
            return new
            {
                success = false,
                error = "Missing required argument: componentTypeName"
            };
        }

        try
        {
            // Get the component type using reflection
            var componentType = System.Type.GetType(componentTypeName);
            if (componentType == null)
            {
                // Try with UnityEngine namespace
                componentType = System.Type.GetType($"UnityEngine.{componentTypeName}");
            }
            if (componentType == null)
            {
                // Try with Praxilabs namespace
                componentType = System.Type.GetType($"Praxilabs.{componentTypeName}");
            }
            if (componentType == null)
            {
                // Try with Praxilabs.Input namespace
                componentType = System.Type.GetType($"Praxilabs.Input.{componentTypeName}");
            }

            if (componentType == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not find component type: {componentTypeName}"
                };
            }

            // Get all public methods
            var methods = componentType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
            
            var methodList = new List<object>();
            foreach (var method in methods)
            {
                // Skip Unity's built-in methods and properties
                if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_") || 
                    method.Name == "ToString" || method.Name == "GetType" || 
                    method.Name == "Equals" || method.Name == "GetHashCode")
                {
                    continue;
                }

                // Get parameter information
                var parameters = method.GetParameters();
                var parameterList = new List<object>();
                
                foreach (var param in parameters)
                {
                    parameterList.Add(new
                    {
                        name = param.Name,
                        type = param.ParameterType.Name,
                        isOptional = param.IsOptional,
                        defaultValue = param.HasDefaultValue ? param.DefaultValue?.ToString() : null
                    });
                }

                methodList.Add(new
                {
                    name = method.Name,
                    returnType = method.ReturnType.Name,
                    parameters = parameterList.ToArray(),
                    parameterCount = parameters.Length
                });
            }

            // Sort methods by name
            methodList = methodList.OrderBy(m => 
            {
                var nameProperty = m.GetType().GetProperty("name");
                return nameProperty?.GetValue(m) as string ?? "";
            }).ToList();

            return new
            {
                success = true,
                message = $"Found {methodList.Count} public methods for component '{componentTypeName}'",
                componentTypeName = componentTypeName,
                methods = methodList.ToArray(),
                count = methodList.Count,
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = $"Failed to get component methods: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Helper method to get registry data from a graph using SerializedObject to access dataRegistries
    /// </summary>
    private static object GetRegistryDataFromGraph(NodeGraph graph)
    {
        try
        {
            // Use SerializedObject to access the dataRegistries field directly
            SerializedObject so = new SerializedObject(graph);
            SerializedProperty dataRegistriesProperty = so.FindProperty("dataRegistries");
            
            if (dataRegistriesProperty == null || dataRegistriesProperty.arraySize == 0)
            {
                return null;
            }
            
            // Get the first registry from the dataRegistries array
            SerializedProperty firstRegistry = dataRegistriesProperty.GetArrayElementAtIndex(0);
            if (firstRegistry.objectReferenceValue == null)
            {
                return null;
            }
            
            return firstRegistry.objectReferenceValue;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to get registry data from graph: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Helper method to get prefab registries from registry data using reflection
    /// </summary>
    private static object GetPrefabRegistries(object registryData)
    {
        if (registryData == null) 
        {
            return null;
        }
        
        var prefabRegisteriesField = registryData.GetType().GetField("prefabRegisteries");
        if (prefabRegisteriesField == null) 
        {
            return null;
        }
        
        return prefabRegisteriesField.GetValue(registryData);
    }
}

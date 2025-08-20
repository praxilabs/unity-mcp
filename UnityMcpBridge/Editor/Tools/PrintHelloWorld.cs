

using Newtonsoft.Json.Linq;
using UnityEngine;

public static class PrintHelloWorld
    {
    public static object HandleCommand(JObject args)
    {
        GameObject go = new GameObject("HelloWorldObject");
        
        go.AddComponent<HelloWorldComponent>();
        
        return new
        {
            success = true,
            message = "Hello, World! printed to console",
            timestamp = System.DateTime.Now.ToString()
        };
    }
    }

internal class HelloWorldComponent : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Hello, World!");
    }
}
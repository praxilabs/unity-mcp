using Newtonsoft.Json.Linq;
using UnityEngine;

public static class Print
{
    public static object HandleCommand(JObject args)
    {
        // Extract value from args, defaulting to "Hello, World!" if not provided
        string valueToPrint = args["value"]?.ToString() ?? "Hello, World!";
        
        Debug.Log($"Print: {valueToPrint}");
        
        return new
        {
            success = true,
            message = $"Value '{valueToPrint}' printed to console",
            timestamp = System.DateTime.Now.ToString()
        };
    }
}
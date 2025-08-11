using System;
using System.Reflection;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static bool _addToDontDestroyOnLoad = true;
    private static readonly object _lock = new object();
    private static bool _applicationQuitting = false;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_applicationQuitting) return null;

            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_addToDontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }

    protected virtual void Initialize() { }

    static bool HasSerializedFields()
    {
        var t = typeof(T);
        var fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in fields)
        {
            var hasSerializeField = Attribute.IsDefined(property, typeof(SerializeField));
            if (hasSerializeField)
            {
                return true;
            }
        }
        return false;
    }

    private void OnApplicationQuit()
    {
        _applicationQuitting = true;
    }
}

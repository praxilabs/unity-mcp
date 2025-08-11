using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnGameEvent", menuName = "Events/Game Event")]
public class GameEventScriptableObject : ScriptableObject
{
    [SerializeField] private List<GameEventListener> listeners = new List<GameEventListener>();

    public void AddListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void RemoveListener(GameEventListener listener)
    {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }

    public void RaiseEvent(object data)
    {
        List<GameEventListener> listenersCopy = new List<GameEventListener>(listeners);

        foreach (var listener in listenersCopy)
        {
            // Check if the listener is still in the original list
            if (listeners.Contains(listener))
            {
                listener.OnEventRaised(data);
            }
        }
    }
}

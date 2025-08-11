using System.Collections.Generic;
using UnityEngine;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] private GameEventScriptableObject gameEvent;
    [SerializeField] private GameEvent response;

    private void OnEnable()
    {
        gameEvent.AddListener(this);
    }

    private void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }

    public void OnEventRaised(object data)
    {
        response?.Invoke(data);
    }
}

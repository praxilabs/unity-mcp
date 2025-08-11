using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Table
{
    public class EventsRegistry : MonoBehaviour
    {
        #region static members
        private static Dictionary<string, UnityEvent> _registry = new Dictionary<string, UnityEvent>();

        public static void Invoke(string EventName)
        {
            Debug.Log($"Attemting invoke eventName {EventName}");

            if (!_registry.ContainsKey(EventName))
            {
                Debug.LogError($"EventsRegistry has no event with name {EventName}");
                return;
            }
            Debug.Log($"Invoking eventName {EventName}");
            _registry[EventName]?.Invoke();
        }
        #endregion

        private void Start()
        {
            if (_registry.Count > 0)
                _registry.Clear();

            for (int i = 0; i < EventsList.Count; i++)
                _registry.Add(EventsList[i].EventName, EventsList[i].Event);
        }

        [field: SerializeField] public List<EventEntry> EventsList { get; set; }

        [System.Serializable]
        public class EventEntry
        {
            [SerializeField] private string _eventName;
            [SerializeField] private UnityEvent _event;

            public string EventName { get => _eventName; set => _eventName = value; }
            public UnityEngine.Events.UnityEvent Event { get => _event; set => _event = value; }
            public void Invoke() => Event.Invoke();
            public void AddListener(UnityEngine.Events.UnityAction call) => Event.AddListener(call);
        }
    }
}
using System;
/// This class acts as a central hub for events to make event subscription/unsubscription and invoking all occurs from the same place to be able to better track/make/call events
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
[RequireComponent(typeof(EventsDebugger))]
#endif
[Serializable]
public class EventsManager : Singleton<EventsManager>
{
    private Dictionary<ZeroParamEventKey, ZeroParamTypedEvent> _zeroParamEventDictionary = new Dictionary<ZeroParamEventKey, ZeroParamTypedEvent>();
    private Dictionary<OneParamEventKey, OneParamTypedEvent> _oneParamtypedEventDictionary = new Dictionary<OneParamEventKey, OneParamTypedEvent>();
    private Dictionary<TwoParamEventKey, TwoParamTypedEvent> _twoParamtypedEventDictionary = new Dictionary<TwoParamEventKey, TwoParamTypedEvent>();
    private Dictionary<ThreeParamEventKey, ThreeParamTypedEvent> _threeParamtypedEventDictionary = new Dictionary<ThreeParamEventKey, ThreeParamTypedEvent>();
    private Dictionary<FourParamEventKey, FourParamTypedEvent> _fourParamtypedEventDictionary = new Dictionary<FourParamEventKey, FourParamTypedEvent>();
    private Dictionary<MultiParamEventKey, MultiParamTypedEvent> _multiParamtypedEventDictionary = new Dictionary<MultiParamEventKey, MultiParamTypedEvent>();

    #region zero Parameter Event
    public void AddListener(ZeroParamEventKey eventKey, UnityAction listener)
    {
        ZeroParamTypedEvent uEvent = new ZeroParamTypedEvent();

        if (_zeroParamEventDictionary.ContainsKey(eventKey))
            uEvent = _zeroParamEventDictionary[eventKey];
        else
            _zeroParamEventDictionary.Add(eventKey, uEvent);

#if UNITY_EDITOR
        EventsDebugger.Instance.AddActionOnInspector(eventKey, uEvent, listener);
#else
        uEvent.AddListener(listener);
#endif
    }

    public void RemoveListener(ZeroParamEventKey eventKey, UnityAction listener)
    {
        if (_zeroParamEventDictionary.ContainsKey(eventKey))
        {
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveActionOnInspector(_zeroParamEventDictionary[eventKey], listener);
#else
            _zeroParamEventDictionary[eventKey].RemoveListener(listener);
#endif
        }
        else
            Debug.LogError("There is no event with this name");
    }

    public void Invoke(ZeroParamEventKey eventKey)
    {
        if (_zeroParamEventDictionary.ContainsKey(eventKey))
            _zeroParamEventDictionary[eventKey].Invoke();
        else
            Debug.LogError("There is no event with this name");
#if UNITY_EDITOR
        EventsDebugger.Instance.triggeredEventsNames.Insert(0, eventKey.ToString());
#endif
    }

    public void RemoveAllListeners(ZeroParamEventKey eventKey)
    {
        if (_zeroParamEventDictionary.ContainsKey(eventKey))
        {
            _zeroParamEventDictionary[eventKey].RemoveAllListeners();
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveAllActionsOnInspector(eventKey, _zeroParamEventDictionary[eventKey]);
#endif
            _zeroParamEventDictionary.Remove(eventKey);
        }
        else
            Debug.LogError("There is no event with this name");
    }
    #endregion

    #region one Parameter Event
    public void AddListener(OneParamEventKey eventKey, UnityAction<object> listener)
    {
        OneParamTypedEvent uEvent = new OneParamTypedEvent();

        if (_oneParamtypedEventDictionary.ContainsKey(eventKey))
            uEvent = _oneParamtypedEventDictionary[eventKey];

        else
            _oneParamtypedEventDictionary.Add(eventKey, uEvent);
#if UNITY_EDITOR
        EventsDebugger.Instance.AddActionOnInspector(eventKey, uEvent, listener);
#else
         uEvent.AddListener(listener);
#endif
    }
    public void RemoveListener(OneParamEventKey eventKey, UnityAction<object> listener)
    {
        if (_oneParamtypedEventDictionary.ContainsKey(eventKey))
        {
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveActionOnInspector(_oneParamtypedEventDictionary[eventKey], listener);
#else
            _oneParamtypedEventDictionary[eventKey].RemoveListener(listener);
#endif
        }
        else
            Debug.LogError("There is no event with this name");

    }

    public void Invoke(OneParamEventKey eventKey, object type)
    {
        if (_oneParamtypedEventDictionary.ContainsKey(eventKey))
            _oneParamtypedEventDictionary[eventKey].Invoke(type);
        else
            Debug.LogError("There is no event with this name");
#if UNITY_EDITOR
        EventsDebugger.Instance.triggeredEventsNames.Insert(0, eventKey.ToString());
#endif
    }
    public void RemoveAllListeners(OneParamEventKey eventKey)
    {
        if (_oneParamtypedEventDictionary.ContainsKey(eventKey))
        {
            _oneParamtypedEventDictionary[eventKey].RemoveAllListeners();
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveAllActionsOnInspector(eventKey, _oneParamtypedEventDictionary[eventKey]);
#endif
            _oneParamtypedEventDictionary.Remove(eventKey);
        }
        else
            Debug.LogError("There is no event with this name");
    }
    #endregion

    #region two Parameter Event
    public void AddListener(TwoParamEventKey eventKey, UnityAction<object, object> listener)
    {
        TwoParamTypedEvent uEvent = new TwoParamTypedEvent();

        if (_twoParamtypedEventDictionary.ContainsKey(eventKey))
            uEvent = _twoParamtypedEventDictionary[eventKey];

        else
            _twoParamtypedEventDictionary.Add(eventKey, uEvent);
#if UNITY_EDITOR
        EventsDebugger.Instance.AddActionOnInspector(eventKey, uEvent, listener);
#else
         uEvent.AddListener(listener);
#endif
    }
    public void RemoveListener(TwoParamEventKey eventKey, UnityAction<object, object> listener)
    {
        if (_twoParamtypedEventDictionary.ContainsKey(eventKey))
        {
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveActionOnInspector(_twoParamtypedEventDictionary[eventKey], listener);
#else
            _twoParamtypedEventDictionary[eventKey].RemoveListener(listener);
#endif
        }
        else
            Debug.LogError("There is no event with this name");

    }

    public void Invoke(TwoParamEventKey eventKey, object type, object type2)
    {
        if (_twoParamtypedEventDictionary.ContainsKey(eventKey))
            _twoParamtypedEventDictionary[eventKey].Invoke(type, type2);
        else
            Debug.LogError("There is no event with this name");
#if UNITY_EDITOR
        EventsDebugger.Instance.triggeredEventsNames.Insert(0, eventKey.ToString());
#endif
    }
    public void RemoveAllListeners(TwoParamEventKey eventKey)
    {
        if (_twoParamtypedEventDictionary.ContainsKey(eventKey))
        {
            _twoParamtypedEventDictionary[eventKey].RemoveAllListeners();
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveAllActionsOnInspector(eventKey, _twoParamtypedEventDictionary[eventKey]);
#endif
            _twoParamtypedEventDictionary.Remove(eventKey);
        }
        else
            Debug.LogError("There is no event with this name");
    }
    #endregion

    #region three Parameter Event
    public void AddListener(ThreeParamEventKey eventKey, UnityAction<object, object, object> listener)
    {
        ThreeParamTypedEvent uEvent = new ThreeParamTypedEvent();

        if (_threeParamtypedEventDictionary.ContainsKey(eventKey))
            uEvent = _threeParamtypedEventDictionary[eventKey];

        else
            _threeParamtypedEventDictionary.Add(eventKey, uEvent);
#if UNITY_EDITOR
        EventsDebugger.Instance.AddActionOnInspector(eventKey, uEvent, listener);
#else
         uEvent.AddListener(listener);
#endif
    }
    public void RemoveListener(ThreeParamEventKey eventKey, UnityAction<object, object, object> listener)
    {
        if (_threeParamtypedEventDictionary.ContainsKey(eventKey))
        {
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveActionOnInspector(_threeParamtypedEventDictionary[eventKey], listener);
#else
            _threeParamtypedEventDictionary[eventKey].RemoveListener(listener);
#endif
        }
        else
            Debug.LogError("There is no event with this name");

    }

    public void Invoke(ThreeParamEventKey eventKey, object type, object type2, object type3)
    {
        if (_threeParamtypedEventDictionary.ContainsKey(eventKey))
            _threeParamtypedEventDictionary[eventKey].Invoke(type, type2, type3);
        else
            Debug.LogError("There is no event with this name");
#if UNITY_EDITOR
        EventsDebugger.Instance.triggeredEventsNames.Insert(0, eventKey.ToString());
#endif
    }
    public void RemoveAllListeners(ThreeParamEventKey eventKey)
    {
        if (_threeParamtypedEventDictionary.ContainsKey(eventKey))
        {
            _threeParamtypedEventDictionary[eventKey].RemoveAllListeners();
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveAllActionsOnInspector(eventKey, _threeParamtypedEventDictionary[eventKey]);
#endif
            _threeParamtypedEventDictionary.Remove(eventKey);
        }
        else
            Debug.LogError("There is no event with this name");
    }
    #endregion

    #region four Parameter Event
    public void AddListener(FourParamEventKey eventKey, UnityAction<object, object, object, object> listener)
    {
        FourParamTypedEvent uEvent = new FourParamTypedEvent();

        if (_fourParamtypedEventDictionary.ContainsKey(eventKey))
            uEvent = _fourParamtypedEventDictionary[eventKey];

        else
            _fourParamtypedEventDictionary.Add(eventKey, uEvent);
#if UNITY_EDITOR
        EventsDebugger.Instance.AddActionOnInspector(eventKey, uEvent, listener);
#else
         uEvent.AddListener(listener);
#endif
    }
    public void RemoveListener(FourParamEventKey eventKey, UnityAction<object, object, object, object> listener)
    {
        if (_fourParamtypedEventDictionary.ContainsKey(eventKey))
        {
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveActionOnInspector(_fourParamtypedEventDictionary[eventKey], listener);
#else
            _fourParamtypedEventDictionary[eventKey].RemoveListener(listener);
#endif
        }
        else
            Debug.LogError("There is no event with this name");

    }

    public void Invoke(FourParamEventKey eventKey, object type, object type2, object type3, object type4)
    {
        if (_fourParamtypedEventDictionary.ContainsKey(eventKey))
            _fourParamtypedEventDictionary[eventKey].Invoke(type, type2, type3, type4);
        else
            Debug.LogError("There is no event with this name");
#if UNITY_EDITOR
        EventsDebugger.Instance.triggeredEventsNames.Insert(0, eventKey.ToString());
#endif
    }
    public void RemoveAllListeners(FourParamEventKey eventKey)
    {
        if (_fourParamtypedEventDictionary.ContainsKey(eventKey))
        {
            _fourParamtypedEventDictionary[eventKey].RemoveAllListeners();
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveAllActionsOnInspector(eventKey, _fourParamtypedEventDictionary[eventKey]);
#endif
            _fourParamtypedEventDictionary.Remove(eventKey);
        }
        else
            Debug.LogError("There is no event with this name");
    }
    #endregion

    #region multi Parameter Event
    public void AddListener(MultiParamEventKey eventKey, UnityAction<List<object>> listener)
    {
        MultiParamTypedEvent uEvent = new MultiParamTypedEvent();

        if (_multiParamtypedEventDictionary.ContainsKey(eventKey))
            uEvent = _multiParamtypedEventDictionary[eventKey];

        else
            _multiParamtypedEventDictionary.Add(eventKey, uEvent);
#if UNITY_EDITOR
        EventsDebugger.Instance.AddActionOnInspector(eventKey, uEvent, listener);
#else
         uEvent.AddListener(listener);
#endif
    }
    public void RemoveListener(MultiParamEventKey eventKey, UnityAction<List<object>> listener)
    {
        if (_multiParamtypedEventDictionary.ContainsKey(eventKey))
        {
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveActionOnInspector(_multiParamtypedEventDictionary[eventKey], listener);
#else
            _multiParamtypedEventDictionary[eventKey].RemoveListener(listener);
#endif
        }
        else
            Debug.LogError("There is no event with this name");

    }

    public void Invoke(MultiParamEventKey eventKey, List<object> listOfTypes)
    {
        if (_multiParamtypedEventDictionary.ContainsKey(eventKey))
            _multiParamtypedEventDictionary[eventKey].Invoke(listOfTypes);
        else
            Debug.LogError("There is no event with this name");
#if UNITY_EDITOR
        EventsDebugger.Instance.triggeredEventsNames.Insert(0, eventKey.ToString());
#endif
    }
    public void Invoke(MultiParamEventKey eventKey, params object[] listOfTypes)
    {
        List<object> listOfTypesList = listOfTypes.ToList();
        if (_multiParamtypedEventDictionary.ContainsKey(eventKey))
            _multiParamtypedEventDictionary[eventKey].Invoke(listOfTypesList);
        else
            Debug.LogError("There is no event with this name");
#if UNITY_EDITOR
        EventsDebugger.Instance.triggeredEventsNames.Insert(0, eventKey.ToString());
#endif
    }
    public void RemoveAllListeners(MultiParamEventKey eventKey)
    {
        if (_multiParamtypedEventDictionary.ContainsKey(eventKey))
        {
            _multiParamtypedEventDictionary[eventKey].RemoveAllListeners();
#if UNITY_EDITOR
            EventsDebugger.Instance.RemoveAllActionsOnInspector(eventKey, _multiParamtypedEventDictionary[eventKey]);
#endif
            _multiParamtypedEventDictionary.Remove(eventKey);
        }
        else
            Debug.LogError("There is no event with this name");
    }
    #endregion
}
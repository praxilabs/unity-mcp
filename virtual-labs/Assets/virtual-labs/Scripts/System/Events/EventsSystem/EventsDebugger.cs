#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
/// <summary>
/// This class is for showing events and events data on inspector 
/// </summary>
public class EventsDebugger : Singleton<EventsDebugger>
{
    [Header("Active Events")]
    [SerializeField]
    private List<ZeroParamEventDebugStruct> _zeroParamEventList;
    [SerializeField]
    private List<OneParamTypedEventDebugStruct> _oneParamEventList;
    [SerializeField]
    private List<TwoParamTypedEventDebugStruct> _twoParamEventList;
    [SerializeField]
    private List<ThreeParamTypedEventDebugStruct> _threeParamEventList;
    [SerializeField]
    private List<FourParamTypedEventDebugStruct> _fourParamEventList;
    [SerializeField]
    private List<MultiParamTypedEventDebugStruct> _MultiParamEventList;

    [Space]
    [Header("Triggered Events")]
    public List<string> triggeredEventsNames = new List<string>();

    #region zero Parameter Event
    public void AddActionOnInspector(ZeroParamEventKey eventKey, ZeroParamTypedEvent uEvent, UnityAction listner)
    {
        UnityAction unityAction = listner;
        if (!(listner.Target is MonoBehaviour))
        {
            GameObject go = new GameObject(listner.Target.GetType().Name + "_" + listner.GetMethodInfo().Name);
            go.transform.SetParent(this.transform);
            unityAction = go.AddComponent<NonMonoEventDebuggerZeroParam>().MethodCall(listner);
        }
        ZeroParamEventDebugStruct eventDictionary = new ZeroParamEventDebugStruct(eventKey.ToString(), uEvent);
        _zeroParamEventList.Remove(eventDictionary);
        _zeroParamEventList.Add(eventDictionary);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(uEvent, unityAction);
    }
    public void RemoveActionOnInspector(ZeroParamTypedEvent uEvent, UnityAction listner)
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(uEvent, listner);
    }
    public void RemoveAllActionsOnInspector(ZeroParamEventKey eventKey, ZeroParamTypedEvent uEvent)
    {
        ZeroParamEventDebugStruct eventDictionary = new ZeroParamEventDebugStruct(eventKey.ToString(), uEvent);
        _zeroParamEventList.Remove(eventDictionary);
    }
    #endregion

    #region one Parameter Event
    public void AddActionOnInspector(OneParamEventKey eventKey, OneParamTypedEvent uEvent, UnityAction<object> listner)
    {
        UnityAction<object> unityAction = listner;
        if (!(listner.Target is MonoBehaviour))
        {
            GameObject go = new GameObject(listner.Target.GetType().Name + "_" + listner.GetMethodInfo().Name);
            go.transform.SetParent(this.transform);
            unityAction = go.AddComponent<NonMonoEventDebuggerOneParam>().MethodCall(listner);
        }
        OneParamTypedEventDebugStruct eventDictionary = new OneParamTypedEventDebugStruct(eventKey.ToString(), uEvent);
        _oneParamEventList.Remove(eventDictionary);
        _oneParamEventList.Add(eventDictionary);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(uEvent, unityAction);
    }
    public void RemoveActionOnInspector(OneParamTypedEvent uEvent, UnityAction<object> listner)
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(uEvent, listner);
    }

    public void RemoveAllActionsOnInspector(OneParamEventKey eventKey, OneParamTypedEvent uEvent)
    {
        OneParamTypedEventDebugStruct eventDictionary = new OneParamTypedEventDebugStruct(eventKey.ToString(), uEvent);
        _oneParamEventList.Remove(eventDictionary);
    }
    #endregion

    #region two Parameter Event
    public void AddActionOnInspector(TwoParamEventKey eventKey, TwoParamTypedEvent uEvent, UnityAction<object, object> listner)
    {
        UnityAction<object, object> unityAction = listner;
        if (!(listner.Target is MonoBehaviour))
        {
            GameObject go = new GameObject(listner.Target.GetType().Name + "_" + listner.GetMethodInfo().Name);
            go.transform.SetParent(this.transform);
            unityAction = go.AddComponent<NonMonoEventDebuggerTwoParam>().MethodCall(listner);
        }
        TwoParamTypedEventDebugStruct eventDictionary = new TwoParamTypedEventDebugStruct(eventKey.ToString(), uEvent);
        _twoParamEventList.Remove(eventDictionary);
        _twoParamEventList.Add(eventDictionary);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(uEvent, unityAction);
    }
    public void RemoveActionOnInspector(TwoParamTypedEvent uEvent, UnityAction<object, object> listner)
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(uEvent, listner);
    }

    public void RemoveAllActionsOnInspector(TwoParamEventKey eventKey, TwoParamTypedEvent uEvent)
    {
        TwoParamTypedEventDebugStruct eventDictionary = new TwoParamTypedEventDebugStruct(eventKey.ToString(), uEvent);
        _twoParamEventList.Remove(eventDictionary);
    }
    #endregion

    #region three Parameter Event
    public void AddActionOnInspector(ThreeParamEventKey eventKey, ThreeParamTypedEvent uEvent, UnityAction<object, object, object> listner)
    {
        UnityAction<object, object, object> unityAction = listner;
        if (!(listner.Target is MonoBehaviour))
        {
            GameObject go = new GameObject(listner.Target.GetType().Name + "_" + listner.GetMethodInfo().Name);
            go.transform.SetParent(this.transform);
            unityAction = go.AddComponent<NonMonoEventDebuggerThreeParam>().MethodCall(listner);
        }
        ThreeParamTypedEventDebugStruct eventDictionary = new ThreeParamTypedEventDebugStruct(eventKey.ToString(), uEvent);
        _threeParamEventList.Remove(eventDictionary);
        _threeParamEventList.Add(eventDictionary);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(uEvent, unityAction);
    }
    public void RemoveActionOnInspector(ThreeParamTypedEvent uEvent, UnityAction<object, object, object> listner)
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(uEvent, listner);
    }

    public void RemoveAllActionsOnInspector(ThreeParamEventKey eventKey, ThreeParamTypedEvent uEvent)
    {
        ThreeParamTypedEventDebugStruct eventDictionary = new ThreeParamTypedEventDebugStruct(eventKey.ToString(), uEvent);
        _threeParamEventList.Remove(eventDictionary);
    }
    #endregion

    #region four Parameter Event
    public void AddActionOnInspector(FourParamEventKey eventKey, FourParamTypedEvent uEvent, UnityAction<object, object, object, object> listner)
    {
        UnityAction<object, object, object, object> unityAction = listner;
        if (!(listner.Target is MonoBehaviour))
        {
            GameObject go = new GameObject(listner.Target.GetType().Name + "_" + listner.GetMethodInfo().Name);
            go.transform.SetParent(this.transform);
            unityAction = go.AddComponent<NonMonoEventDebuggerFourParam>().MethodCall(listner);
        }
        FourParamTypedEventDebugStruct eventDictionary = new FourParamTypedEventDebugStruct(eventKey.ToString(), uEvent);
        _fourParamEventList.Remove(eventDictionary);
        _fourParamEventList.Add(eventDictionary);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(uEvent, unityAction);
    }
    public void RemoveActionOnInspector(FourParamTypedEvent uEvent, UnityAction<object, object, object, object> listner)
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(uEvent, listner);
    }

    public void RemoveAllActionsOnInspector(FourParamEventKey eventKey, FourParamTypedEvent uEvent)
    {
        FourParamTypedEventDebugStruct eventDictionary = new FourParamTypedEventDebugStruct(eventKey.ToString(), uEvent);
        _fourParamEventList.Remove(eventDictionary);
    }
    #endregion

    #region Multi Parameter Event
    public void AddActionOnInspector(MultiParamEventKey eventKey, MultiParamTypedEvent uEvent, UnityAction<List<object>> listner)
    {
        UnityAction<List<object>> unityAction = listner;
        if (!(listner.Target is MonoBehaviour))
        {
            GameObject go = new GameObject(listner.Target.GetType().Name + "_" + listner.GetMethodInfo().Name);
            go.transform.SetParent(this.transform);
            unityAction = go.AddComponent<NonMonoEventDebuggerMultiParam>().MethodCall(listner);
        }
        MultiParamTypedEventDebugStruct eventDictionary = new MultiParamTypedEventDebugStruct(eventKey.ToString(), uEvent);
        _MultiParamEventList.Remove(eventDictionary);
        _MultiParamEventList.Add(eventDictionary);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(uEvent, unityAction);
    }
    public void RemoveActionOnInspector(MultiParamTypedEvent uEvent, UnityAction<List<object>> listner)
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(uEvent, listner);
    }

    public void RemoveAllActionsOnInspector(MultiParamEventKey eventKey, MultiParamTypedEvent uEvent)
    {
        MultiParamTypedEventDebugStruct eventDictionary = new MultiParamTypedEventDebugStruct(eventKey.ToString(), uEvent);
        _MultiParamEventList.Remove(eventDictionary);
    }
    #endregion

}

#endif

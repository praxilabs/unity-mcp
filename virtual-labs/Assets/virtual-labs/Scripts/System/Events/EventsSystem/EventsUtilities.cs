using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

# region Events Classes
[Serializable]
public class ZeroParamTypedEvent : UnityEvent { }
[Serializable]
public class OneParamTypedEvent : UnityEvent<object> { }
[Serializable]
public class TwoParamTypedEvent : UnityEvent<object, object> { }
[Serializable]
public class ThreeParamTypedEvent : UnityEvent<object, object, object> { }
[Serializable]
public class FourParamTypedEvent : UnityEvent<object, object, object, object> { }
[Serializable]
public class MultiParamTypedEvent : UnityEvent<List<object>> { }
#endregion

# region Events Keys For Events Dictionaries
[Serializable]
public struct ZeroParamEventKey
{
    private string _eventName;
    public ZeroParamEventKey(string eventName)
    {
        _eventName = eventName;
    }
    public override string ToString()
    {
        return _eventName;
    }
}
[Serializable]
public struct OneParamEventKey
{
    private string _eventName;
    public OneParamEventKey(string eventName)
    {
        _eventName = eventName;
    }
    public override string ToString()
    {
        return _eventName;
    }
}
[Serializable]
public struct TwoParamEventKey
{
    private string _eventName;
    public TwoParamEventKey(string eventName)
    {
        _eventName = eventName;
    }
    public override string ToString()
    {
        return _eventName;
    }
}
[Serializable]
public struct ThreeParamEventKey
{
    private string _eventName;
    public ThreeParamEventKey(string eventName)
    {
        _eventName = eventName;
    }
    public override string ToString()
    {
        return _eventName;
    }
}
[Serializable]
public struct FourParamEventKey
{
    private string _eventName;
    public FourParamEventKey(string eventName)
    {
        _eventName = eventName;
    }
    public override string ToString()
    {
        return _eventName;
    }
}
[Serializable]
public struct MultiParamEventKey
{
    private string _eventName;
    public MultiParamEventKey(string eventName)
    {
        _eventName = eventName;
    }
    public override string ToString()
    {
        return _eventName;
    }
}
#endregion

#region Events Debugging Structs For Inspector
[Serializable]
public struct ZeroParamEventDebugStruct : IEqualityComparer<ZeroParamEventDebugStruct>
{
    public string eventName;
    public ZeroParamTypedEvent myEvent;
    public ZeroParamEventDebugStruct(string evName, ZeroParamTypedEvent ev)
    {
        eventName = evName;
        myEvent = ev;
    }
    public bool Equals(ZeroParamEventDebugStruct x, ZeroParamEventDebugStruct y)
    {
        if (x.eventName == y.eventName && x.myEvent == y.myEvent)
            return true;

        return false;
    }
    public int GetHashCode(ZeroParamEventDebugStruct obj)
    {
        return obj.eventName.GetHashCode();
    }
}

[Serializable]
public struct OneParamTypedEventDebugStruct : IEqualityComparer<OneParamTypedEventDebugStruct>
{
    public string eventName;
    public OneParamTypedEvent myEvent;
    public OneParamTypedEventDebugStruct(string evName, OneParamTypedEvent ev)
    {
        eventName = evName;
        myEvent = ev;
    }
    public bool Equals(OneParamTypedEventDebugStruct x, OneParamTypedEventDebugStruct y)
    {
        if (x.eventName == y.eventName && x.myEvent == y.myEvent)
            return true;

        return false;
    }
    public int GetHashCode(OneParamTypedEventDebugStruct obj)
    {
        return obj.eventName.GetHashCode();
    }
}

[Serializable]
public struct TwoParamTypedEventDebugStruct
{
    public string eventName;
    public TwoParamTypedEvent myEvent;
    public TwoParamTypedEventDebugStruct(string evName, TwoParamTypedEvent ev)
    {
        eventName = evName;
        myEvent = ev;
    }
    public bool Equals(TwoParamTypedEventDebugStruct x, TwoParamTypedEventDebugStruct y)
    {
        if (x.eventName == y.eventName && x.myEvent == y.myEvent)
            return true;

        return false;
    }
    public int GetHashCode(TwoParamTypedEventDebugStruct obj)
    {
        return obj.eventName.GetHashCode();
    }
}

[Serializable]
public struct ThreeParamTypedEventDebugStruct
{
    public string eventName;
    public ThreeParamTypedEvent myEvent;

    public ThreeParamTypedEventDebugStruct(string evName, ThreeParamTypedEvent ev)
    {
        eventName = evName;
        myEvent = ev;
    }
    public bool Equals(ThreeParamTypedEventDebugStruct x, ThreeParamTypedEventDebugStruct y)
    {
        if (x.eventName == y.eventName && x.myEvent == y.myEvent)
            return true;

        return false;
    }
    public int GetHashCode(ThreeParamTypedEventDebugStruct obj)
    {
        return obj.eventName.GetHashCode();
    }
}
[Serializable]
public struct FourParamTypedEventDebugStruct
{
    public string eventName;
    public FourParamTypedEvent myEvent;

    public FourParamTypedEventDebugStruct(string evName, FourParamTypedEvent ev)
    {
        eventName = evName;
        myEvent = ev;
    }
    public bool Equals(FourParamTypedEventDebugStruct x, FourParamTypedEventDebugStruct y)
    {
        if (x.eventName == y.eventName && x.myEvent == y.myEvent)
            return true;

        return false;
    }
    public int GetHashCode(FourParamTypedEventDebugStruct obj)
    {
        return obj.eventName.GetHashCode();
    }
}

[Serializable]
public struct MultiParamTypedEventDebugStruct
{
    public string eventName;
    public MultiParamTypedEvent myEvent;

    public MultiParamTypedEventDebugStruct(string evName, MultiParamTypedEvent ev)
    {
        eventName = evName;
        myEvent = ev;
    }
    public bool Equals(MultiParamTypedEventDebugStruct x, MultiParamTypedEventDebugStruct y)
    {
        if (x.eventName == y.eventName && x.myEvent == y.myEvent)
            return true;

        return false;
    }
    public int GetHashCode(MultiParamTypedEventDebugStruct obj)
    {
        return obj.eventName.GetHashCode();
    }
}
#endregion

#region Events Creator Scriptable Object Utilities
[Serializable]
public class EventContainer
{

    [HideInInspector] public string typeName;
    [HideInInspector] public int eventsCount;

    [HideInInspector]
    public EventKeyType eventType;
    public List<EventName> eventList;

    public EventContainer(EventKeyType eventTypeConstructor)
    {
        typeName = eventTypeConstructor.ToString();
        eventType = eventTypeConstructor;
        eventList = new List<EventName>();
        eventsCount = eventList.Count;
    }

    public void UpdateEventCount()
    {
        eventsCount = eventList.Count;
        typeName = eventType.ToString() + $"    [{eventsCount}]";
    }
}

[Serializable]
public enum EventKeyType
{
    ZeroParam,
    OneParam,
    TwoParam,
    ThreeParam,
    FourParam,
    MultiParam,
}

[Serializable]
public struct EventName
{
    public string eventName;
    [TextArea(2, 2)] public string description;
}

#endregion
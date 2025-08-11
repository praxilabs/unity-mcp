using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using System;

public class NonMonoEventDebuggerZeroParam : MonoBehaviour
{
    private UnityAction uAction;
    public UnityAction MethodCall(UnityAction action)
    {
        uAction = action;
        return new UnityAction(MethodInvoke);
    }
    public void MethodInvoke()
    {
        uAction.Invoke();
    }
}
public class NonMonoEventDebuggerOneParam : MonoBehaviour
{
    private UnityAction<object> uAction;
    public UnityAction<object> MethodCall(UnityAction<object> action)
    {
        uAction = action;
        return new UnityAction<object>(MethodInvoke);
    }
    public void MethodInvoke(object obj)
    {
        uAction.Invoke(obj);
    }
}
public class NonMonoEventDebuggerTwoParam : MonoBehaviour
{
    private UnityAction<object, object> uAction;
    public UnityAction<object, object> MethodCall(UnityAction<object, object> action)
    {
        uAction = action;
        return new UnityAction<object, object>(MethodInvoke);
    }
    public void MethodInvoke(object obj, object obj2)
    {
        uAction.Invoke(obj, obj2);
    }
}
public class NonMonoEventDebuggerThreeParam : MonoBehaviour
{
    private UnityAction<object, object, object> uAction;
    public UnityAction<object, object, object> MethodCall(UnityAction<object, object, object> action)
    {
        uAction = action;
        return new UnityAction<object, object, object>(MethodInvoke);
    }
    public void MethodInvoke(object obj, object obj2, object obj3)
    {
        uAction.Invoke(obj, obj2, obj3);
    }
}
public class NonMonoEventDebuggerFourParam : MonoBehaviour
{
    private UnityAction<object, object, object, object> uAction;
    public UnityAction<object, object, object, object> MethodCall(UnityAction<object, object, object, object> action)
    {
        uAction = action;
        return new UnityAction<object, object, object, object>(MethodInvoke);
    }
    public void MethodInvoke(object obj, object obj2, object obj3, object obj4)
    {
        uAction.Invoke(obj, obj2, obj3, obj4);
    }
}
public class NonMonoEventDebuggerMultiParam : MonoBehaviour
{
    private UnityAction<List<object>> uAction;
    public UnityAction<List<object>> MethodCall(UnityAction<List<object>> action)
    {
        uAction = action;
        return new UnityAction<List<object>>(MethodInvoke);
    }
    public void MethodInvoke(List<object> objs)
    {
        uAction.Invoke(objs);
    }
}
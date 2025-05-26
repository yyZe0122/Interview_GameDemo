using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用于里氏替换原则 装载 子类的父类
/// </summary>
public abstract class EventInfoBase { }

/// <summary>
/// 用来包裹 对应观察者 委托函数的 类
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventInfo<T> : EventInfoBase
{
    //真正观察者 对应的函数信息 记录在其中
    public UnityAction<T> action;

    public EventInfo(UnityAction<T> actions)
    {
        action += actions;
    }
}

/// <summary>
/// 主要用来记录无参无返回值的委托
/// </summary>
public class EventInfo : EventInfoBase
{
    //真正观察者 对应的函数信息 记录在其中
    public UnityAction action;

    public EventInfo(UnityAction actions)
    {
        action += actions;
    }
}

/// <summary>
/// 事件中心模块
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    //用于记录对应事件 关联的 对应逻辑
    private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();
    
    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName">事件名字</param>
    //触发(分发)事件 方法
    public void EventTrigger<T>(E_EventType eventName,T info)
    {
        //存在有委托的人才处理逻辑
        if (eventDic.ContainsKey(eventName))
        {
            //去执行对应逻辑
            (eventDic[eventName] as EventInfo<T>).action?.Invoke(info);
        }
    }

    /// <summary>
    /// 触发事件 无参数
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(E_EventType eventName)
    {
        //存在有委托的人才处理逻辑
        if (eventDic.ContainsKey(eventName))
        {
            //去执行对应逻辑
            (eventDic[eventName] as EventInfo).action?.Invoke();
        }
    }
    
    /// <summary>
    /// 添加事件监听者
    /// </summary>
    /// <param name="eventName">事件名字</param>
    /// <param name="func">监听调用函数</param>
    //添加事件监听者
    public void AddEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        //如果已经存在关心事件的委托记录 直接添加即可
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).action += func;
        }
        else
        {
            eventDic.Add(eventName,new EventInfo<T>(func));
        }
    }
    
    /// <summary>
    /// 添加事件监听者 无参数
    /// </summary>
    /// <param name="eventName">事件名字</param>
    /// <param name="func">监听调用函数</param>
    //添加事件监听者
    public void AddEventListener(E_EventType eventName, UnityAction func)
    {
        //如果已经存在关心事件的委托记录 直接添加即可
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).action += func;
        }
        else
        {
            eventDic.Add(eventName,new EventInfo(func));
        }
    }

    /// <summary>
    /// 移除事件监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func">监听调用函数</param>
    public void RemoveEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).action -= func;
        }
        
    }
    
    /// <summary>
    /// 移除事件监听者 无参数
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func">监听调用函数</param>
    public void RemoveEventListener(E_EventType eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).action -= func;
        }
        
    }

    /// <summary>
    /// 清空所有事件的监听
    /// </summary>
    public void ClearEvent()
    {
        eventDic.Clear();
    }

    /// <summary>
    /// 清除指定事件的所有监听
    /// </summary>
    /// <param name="eventName"></param>
    public void ClearEvent(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            eventDic.Remove(eventName);
        }
    }
    
    private EventCenter() {}
}

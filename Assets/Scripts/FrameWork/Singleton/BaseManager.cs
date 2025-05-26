using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 单例模式基类 方便实现单例模式的类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseManager<T> where T:class//,new()
{
    private static T instance;

    //保护的 静态的 只读的Object对象
    protected static readonly object lockObject = new object(); 

    /// <summary>
    /// 属性的方式(推荐)
    /// </summary>
    public static T Instance
    {
        get
        {
            //判断是否为空 防止性能问题
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        //instance = new T();
                        Type type = typeof(T);
                        //用反射得到无参私有的构造函数 用来实例化对象
                        //需要在继承的子类中申明私有的无参构造函数
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                            null, Type.EmptyTypes, null);
                        if (info != null)
                        {
                            instance = info.Invoke(null) as T;
                        }
                        else
                        {
                            Debug.LogError("没有得到对应的无参构造函数");
                        }
                    }
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// 方法的方式(不推荐)
    /// </summary>
    /// <returns></returns>
    //public static T GetInstance()
    //{
    //     if (instance == null)
    //     {
    //         instance = new T();
    //     }
    //     return instance;
    // }
}

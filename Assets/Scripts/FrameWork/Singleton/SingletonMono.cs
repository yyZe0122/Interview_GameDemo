using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 继承Mono的单例模式基类 是挂载式
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour  where T: MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// 可以被重写
    /// </summary>
    protected virtual void Awake()
    {
        //判断之前存在单例模式对象
        if (instance != null)
        {
            //移除自己的脚本
            //解决唯一性
            Destroy(this);
        }
        instance = this as T;
        //过场景不移除
        DontDestroyOnLoad(this.gameObject);
    }
}

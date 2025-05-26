using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动挂载式的单例模式的基类 推荐
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //动态创建 动态挂载
                //在场景上创建空物体
                GameObject gameObject = new GameObject();
                //得到T脚本名 为对象名 这样在编辑器脚本中可以明确
                gameObject.name = typeof(T).ToString();
                //动态挂载对应的单例模式脚本
                instance = gameObject.AddComponent<T>();
                //过场景不移除对象
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
    }
    
}

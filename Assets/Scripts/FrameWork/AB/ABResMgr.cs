using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ABResMgr : BaseManager<ABResMgr>
{
    //是不是在编辑器模式下调试
    //true通过EditorResMgr加载
    //false通过ABMgr加载
    private bool isDebug = true;
    
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isAsync = false)
        where T : Object
    {
#if UNITY_EDITOR
        //如果是调试状态
        if (isDebug)
        {
            //定义一个AB包资源的管理模式 对应文件夹名 就是包名(abName)
            T res = EditorResMgr.Instance.LoadEditorRes<T>($"{abName}/{resName}");
            callBack?.Invoke(res as T);
        }
        //如果不是调试状态
        else
        {
            ABMgr.Instance.LoadResAsync<T>(abName, resName, callBack, isAsync);
        }
#else
        //游戏发布使用的
        ABMgr.Instance.LoadResAsync<T>(abName, resName, callBack, isAsync);
#endif
    }
    
    
    private ABResMgr() {}
}

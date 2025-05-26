using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换管理器 主要用于切换游戏场景
/// </summary>
public class SceneMgr : BaseManager<SceneMgr>
{
    private SceneMgr() {}
    
    /// <summary>
    /// 同步切换场景方法
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="callBack"></param>
    public void LoadScene(string name, UnityAction callBack = null)
    {
        //切换场景
        SceneManager.LoadScene(name);
        //调用回调函数
        callBack?.Invoke();
    }
    
    //异步切换场景方法
    public void LoadSceneAsync(string name, UnityAction callBack = null)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsync(name, callBack));
    }
    
    public IEnumerator ReallyLoadSceneAsync(string name, UnityAction callBack = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //不停的在协同程序中 每帧检查是否加载结束
        while (!ao.isDone)
        {
            //可以利用事件中心发送出去
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange,ao.progress);
            yield return 0;
        }
        //避免最后一帧直接结束 没有同步发送出去
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange,1);
        
        //异步加载结束 返回回调函数
        callBack?.Invoke();
        callBack = null;
    }
}

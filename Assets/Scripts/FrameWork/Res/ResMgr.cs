using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// 资源信息基类
/// 主要用于里氏替换 父类容器装子类对象
/// 设置抽象类 不实例化
/// </summary>
public abstract class ResInfoBase
{
    //引用计数
    public int refCount = 0;

}

/// <summary>
/// 资源信息对象
/// 主要用于存储资源信息 异步加载委托信息 异步加载协程信息
/// </summary>
/// <typeparam name="T">资源类型</typeparam>
public class ResInfo<T> : ResInfoBase
{
    //记录资源
    public T asset;
    
    //记录委托 
    //主要用于异步加载结束后 传递资源到外部的委托
    public UnityAction<T> callBack;
    
    //记录协同程序 方便单独实现停止
    //主要用于存储 异步加载启动时开启的协同程序
    public Coroutine coroutine;

    //引用计数为0时 是否真正移除
    public bool isDel;
    
    public void AddRefCount()
    {
        refCount++;
    }
    
    public void RemoveRefCount()
    {
        refCount--;
        if (refCount < 0)
        {
            Debug.LogError("引用计数小于0了 请检查引用和卸载是否配对");
        }
    }
}

/// <summary>
/// Resources资源加载模块管理器
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //用于存储加载过的资源 或者加载中的资源 的容器
    //string资源路径 ResInfoBase值为自定义 将资源,委托等等都封装 方便解决问题
    public Dictionary<string, ResInfoBase> resDic = new Dictionary<string, ResInfoBase>();
    
    private ResMgr() { }

    /// <summary>
    /// 同步加载Resources资源的方法
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Load<T>(string path) where T: Object
    {
        //资源的唯一ID是 "路径名_资源类型" 拼接而成
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> info = new ResInfo<T>();
        //字典中不存在资源
        if (!resDic.ContainsKey(resName))
        {
            //直接同步加载 并且添加资源到字典中
            info.asset = Resources.Load<T>(path);
            //添加引用计数
            info.AddRefCount();
            //并且记录资源信息到字典中
            resDic.Add(resName,info);
            //传出资源
            return info.asset;
        }
        else
        {
            //取出字典中的资源记录
            info = resDic[resName] as ResInfo<T>;
            //添加引用计数
            info.AddRefCount();
            //如果资源为空 那么还在异步加载
            if (info.asset == null)
            {
                //停止异步加载
                MonoMgr.Instance.StopCoroutine(info.coroutine);
                //直接同步加载
                //直接同步加载 并且添加资源到字典中
                info.asset = Resources.Load<T>(path);
                //停止异步加载后 还应该把等待着的委托去执行
                info.callBack?.Invoke(info.asset);
                //回调结束 异步加载也停了 所以清除无用的引用
                info.callBack = null;
                info.coroutine = null;
                
                return info.asset;
            }
            else
            {
                //清除无用的引用
                info.callBack = null;
                info.coroutine = null;
                
                return info.asset;
            }
        }
        
    }
    
    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <param name="path">Resources资源路径</param>
    /// <param name="callBack">加载结束后的回调函数 资源加载结束后会调用</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void LoadAsync<T>(string path,UnityAction<T> callBack) where T: Object
    {
        //资源的唯一ID是 "路径名_资源类型" 拼接而成
        string resName = path + "_" + typeof(T).Name;
        //声明资源信息对象
        ResInfo<T> info;
        //如果res字典中没有存储对应的资源信息
        if (!resDic.ContainsKey(resName))
        {
            info = new ResInfo<T>();
            //将资源记录添加到字典中(资源还没有加载成功)
            //添加引用计数
            info.AddRefCount();
            resDic.Add(resName,info);
            //记录传入的委托函数 等加载完成再使用
            info.callBack += callBack;
            //开启协同程序 异步加载资源 并且记录协同程序(以后可能停止)
            info.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path, callBack));
        }
        //如果存在资源
        else
        {
            //从字典中取出资源信息
            info = resDic[resName] as ResInfo<T>;
            //添加引用计数
            info.AddRefCount();
            //如果资源还没加载完毕
            //如果资源为null 那么代表没有加载完毕 还在加载
            if (info.asset == null)
            {
                //再添加委托 在资源加载完毕后 全部传回去
                info.callBack += callBack;
            }
            //如果资源加载完毕
            else
            {
                //资源已经加载 所以调用委托直接传资源
                callBack?.Invoke(info.asset);
            }
        }
    }

    private IEnumerator ReallyLoadAsync<T>(string path, UnityAction<T> callBack) where T : Object
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync<GameObject>(path);
        //等待资源加载结束后才会继续执行 后面的代码
        yield return rq;
        //资源加载结束
        //执行将资源传到外部的委托函数去进行使用
        
        //资源的唯一ID是 "路径名_资源类型" 拼接而成
        string resName = path + "_" + typeof(T).Name;
        if (resDic.ContainsKey(resName))
        {
            //as是因为上面new用的父类
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //取出资源信息 并且 记录加载完成的资源
            resInfo.asset = rq.asset as T;

            //判断是否需要待删除
            //引用计数为0 才删除
            if (resInfo.refCount == 0)
            {
                //删除
                UnLoadAsset<T>(path,resInfo.isDel,null,false);
            }
            else
            {
                //将加载完成的资源传出去
                resInfo.callBack?.Invoke(resInfo.asset);

                //当加载完毕后 引用就可以清空
                //避免引用占用 潜在的内存泄漏
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }
    
    /// <summary>
    /// 异步加载资源的方法 通过type的方式
    /// </summary>
    /// <param name="path">Resources资源路径</param>
    /// <param name="callBack">加载结束后的回调函数 资源加载结束后会调用</param>
    [Obsolete("建议使用泛型加载方式 如果要type加载 那么不能和泛型混合使用加载同类型同名资源")]
    public void LoadAsync(string path,Type type,UnityAction<Object> callBack)
    {
        //资源的唯一ID是 "路径名_资源类型" 拼接而成
        string resName = path + "_" + type.Name;
        //声明资源信息对象
        ResInfo<Object> info;
        //如果res字典中没有存储对应的资源信息
        if (!resDic.ContainsKey(resName))
        {
            info = new ResInfo<Object>();
            //将资源记录添加到字典中(资源还没有加载成功)
            //添加引用计数
            info.AddRefCount();
            resDic.Add(resName,info);
            //记录传入的委托函数 等加载完成再使用
            info.callBack += callBack;
            //开启协同程序 异步加载资源 并且记录协同程序(以后可能停止)
            info.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<Object>(path, callBack));
        }
        //如果存在资源
        else
        {
            //从字典中取出资源信息
            info = resDic[resName] as ResInfo<Object>;
            //添加引用计数
            info.AddRefCount();
            //如果资源还没加载完毕
            //如果资源为null 那么代表没有加载完毕 还在加载
            if (info.asset == null)
            {
                //再添加委托 在资源加载完毕后 全部传回去
                info.callBack += callBack;
            }
            //如果资源加载完毕
            else
            {
                //资源已经加载 所以调用委托直接传资源
                callBack?.Invoke(info.asset);
            }
        }
    }

    private IEnumerator ReallyLoadAsync(string path,Type type)
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync(path, type);
        //等待资源加载结束后才会继续执行 后面的代码
        yield return rq;
        //资源加载结束
        //执行将资源传到外部的委托函数去进行使用
        
        //资源的唯一ID是 "路径名_资源类型" 拼接而成
        string resName = path + "_" + type.Name;
        if (resDic.ContainsKey(resName))
        {
            //as是因为上面new用的父类
            ResInfo<Object> resInfo = resDic[resName] as ResInfo<Object>;
            //取出资源信息 并且 记录加载完成的资源
            resInfo.asset = rq.asset;
            
            //判断是否需要待删除
            //引用计数为0 才删除
            if (resInfo.refCount == 0)
            {
                //删除
                UnLoadAsset(path,type,resInfo.isDel,null,false);
            }
            else
            {
                //将加载完成的资源传出去
                resInfo.callBack?.Invoke(resInfo.asset);

                //当加载完毕后 引用就可以清空
                //避免引用占用 潜在的内存泄漏
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }

    /// <summary>
    /// 指定卸载的资源
    /// 通过T泛型
    /// </summary>
    /// <param name="path">对应的资源路径</param>
    /// <param name="callBack">卸载的是异步加载的委托</param>
    public void UnLoadAsset<T>(string path,bool isDel = false,UnityAction<T> callBack = null,bool isRemove = true)
    {
        //资源的唯一ID是 "路径名_资源类型" 拼接而成
        string resName = path + "_" + typeof(T).Name;
        //判断是否存在对应资源
        if (resDic.ContainsKey(resName))
        {
            ResInfo<T> info = resDic[resName] as ResInfo<T>;
            //减少引用计数
            if (isRemove)
            {
                info.RemoveRefCount();
            }
            //记录引用计数为0时 是否马上移除标签
            info.isDel = isDel;
            //不为空 资源已经加载完毕
            if (info.asset != null && info.refCount == 0 && info.isDel)
            {
                //从字典移除
                resDic.Remove(resName);
                //通过API卸载资源
                Resources.UnloadAsset(info.asset as Object);
            }
            //资源正在异步加载中 只移除回调函数
            else if (info.asset == null)
            {
                //改变标识 等待删除
                //info.isDel = true;
                /*
                 //可能存在卸载不了
                MonoMgr.Instance.StopCoroutine(info.coroutine);
                resDic.Remove(resName);
                */
                
                //当异步加载不想使用时 应该删除回调记录 而不是卸载资源
                if (callBack != null)
                {
                    info.callBack -= callBack;
                }
            }
        }
    }
    
    /// <summary>
    /// 使用Type的方式移除Resources资源
    /// </summary>
    /// <param name="path">传入资源路径</param>
    /// <param name="type">传入资源类型</param>
    public void UnLoadAsset(string path,Type type,bool isDel = false,UnityAction<Object> callBack = null,bool isRemove = true)
    {
        //资源的唯一ID是 "路径名_资源类型" 拼接而成
        string resName = path + "_" + type.Name;
        //判断是否存在对应资源
        if (resDic.ContainsKey(resName))
        {
            ResInfo<Object> info = resDic[resName] as ResInfo<Object>;
            //减少引用计数
            if (isRemove)
            {
                info.RemoveRefCount();
            }
            //记录引用计数为0时 是否马上移除标签
            info.isDel = isDel;
            //不为空 资源已经加载完毕
            if (info.asset != null && info.refCount == 0 && info.isDel)
            {
                //从字典移除
                resDic.Remove(resName);
                //通过API卸载资源
                Resources.UnloadAsset(info.asset);
            }
            //资源正在异步加载中
            else if (info.asset == null)
            {
                //改变标识 等待删除
                //info.isDel = true;
                
                //当异步加载不想使用时 应该删除回调记录 而不是卸载资源
                if (callBack != null)
                {
                    info.callBack -= callBack;
                }
            }
        }
    }
    

    /// <summary>
    /// 异步卸载没有使用的 Resources相关的资源
    /// </summary>
    /// <param name="callBack">回调函数</param>
    public void UnLoadUnsedAssets(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnLoadUnsedAssets(callBack));
    }

    private IEnumerator ReallyUnLoadUnsedAssets(UnityAction callBack)
    {
        //在真正移除不使用的资源之前
        //应该把自己记录的引用计数为0 并且没有被移除记录的资源 全部移除掉
        List<string> list = new List<string>();
        foreach (string path in resDic.Keys)
        {
            if (resDic[path].refCount == 0)
            {
                list.Add(path);
            }
        }
        foreach (string path in list)
        {
            resDic.Remove(path);
        }
        
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        //卸载完毕后 通知外部
        callBack();
    }
    
    /// <summary>
    /// 获取当前某个资源的引用计数
    /// </summary>
    /// <param name="path">传入的地址</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns></returns>
    public int GetRefCoun<T>(string path)
    {
        //资源的唯一ID是 "路径名_资源类型" 拼接而成
        string resName = path + "_" + typeof(T).Name;
        if (resDic.ContainsKey(resName))
        {
            return (resDic[resName] as ResInfo<T>).refCount;
        }
        return 0;
    }

    /// <summary>
    /// 清空字典
    /// </summary>
    /// <param name="callBack">清空字典完成后调用的委托函数</param>
    public void ClearDic(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyClearDic(callBack));
    }
    
    private IEnumerator ReallyClearDic(UnityAction callBack)
    {
        resDic.Clear();
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        //卸载完毕后 通知外部
        callBack();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//知识点
//字典
//协程
//AB包相关API
//委托
//lambda表达式
//单例模式基类——>观看Unity小框架视频 进行学习
public class ABMgr : SingleAutoMono<ABMgr>
{
    //主包
    private AssetBundle mainAB = null;
    //主包依赖获取配置文件
    private AssetBundleManifest manifest = null;

    //选择存储 AB包的容器
    //AB包不能够重复加载 否则会报错
    //字典知识 用来存储 AB包对象
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 获取AB包加载路径
    /// </summary>
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    /// <summary>
    /// 主包名 根据平台不同 报名不同
    /// </summary>
    private string MainName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    /// <summary>
    /// 加载主包 和 配置文件
    /// 因为加载所有包是 都得判断 通过它才能得到依赖信息
    /// 所以写一个方法
    /// </summary>
    private void LoadMainAB()
    {
        if( mainAB == null )
        {
            mainAB = AssetBundle.LoadFromFile( PathUrl + MainName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    /// <summary>
    /// 加载指定包的依赖包
    /// </summary>
    /// <param name="abName"></param>
    private void LoadDependencies(string abName)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i], ab);
            }
        }
    }
    
    /// <summary>
    /// 泛型异步加载资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    /// <param name="isAsync">是否进行同步加载 需要的话传入true</param>
    /// <typeparam name="T"></typeparam>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isAsync = false) where T:Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack,isAsync));
    }
    //正儿八经的 协程函数
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isAsync = false) where T : Object
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //还没有加载AB包
            if (!abDic.ContainsKey(strs[i]))
            {
                //同步加载
                if (isAsync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                //异步加载
                else
                {
                    //使用null在字典中占位 代表正在异步加载
                    abDic.Add(strs[i],null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    //异步加载结束后 替换null 代表加载结束
                    abDic[strs[i]] = req.assetBundle;
                }
            }
            //就证明字典中已经记录了一个AB包信息
            else
            {
                //字典中对应的数据如果中null 那么证明还在加载
                while (abDic[strs[i]] == null)
                {
                    //只要发现在加载中 那么就等待一帧
                    yield return 0;
                    //一帧后进行判断
                }
            }
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            //同步加载
            if (isAsync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            //异步加载
            else
            {
                //使用null在字典中占位 代表正在异步加载
                abDic.Add(abName,null);
            
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                //异步加载结束后 替换null 代表加载结束
                abDic[abName] = req.assetBundle;
            }
        }
        //就证明字典中已经记录了目标AB包信息
        else
        {
            //字典中对应的数据如果中null 那么证明还在加载
            while (abDic[abName] == null)
            {
                //只要发现在加载中 那么就等待一帧
                yield return 0;
                //一帧后进行判断
            }
        }
        //判断是同步加载还是异步加载
        if (isAsync)
        {
            //同步加载AB包中的资源
            T res = abDic[abName].LoadAsset<T>(resName);
            callBack?.Invoke(res);
        }
        else
        {
            //异步加载包中资源
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abq;
            
            callBack?.Invoke((abq.asset as T));
        }
    }

    /// <summary>
    /// Type异步加载资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack, bool isAsync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack, isAsync));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack, bool isAsync = false)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //还没有加载AB包
            if (!abDic.ContainsKey(strs[i]))
            {
                //同步加载
                if (isAsync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                //异步加载
                else
                {
                    //使用null在字典中占位 代表正在异步加载
                    abDic.Add(strs[i],null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    //异步加载结束后 替换null 代表加载结束
                    abDic[strs[i]] = req.assetBundle;
                }
            }
            //就证明字典中已经记录了一个AB包信息
            else
            {
                //字典中对应的数据如果中null 那么证明还在加载
                while (abDic[strs[i]] == null)
                {
                    //只要发现在加载中 那么就等待一帧
                    yield return 0;
                    //一帧后进行判断
                }
            }
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            //同步加载
            if (isAsync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            //异步加载
            else
            {
                //使用null在字典中占位 代表正在异步加载
                abDic.Add(abName,null);
            
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                //异步加载结束后 替换null 代表加载结束
                abDic[abName] = req.assetBundle;
            }
        }
        //就证明字典中已经记录了目标AB包信息
        else
        {
            //字典中对应的数据如果中null 那么证明还在加载
            while (abDic[abName] == null)
            {
                //只要发现在加载中 那么就等待一帧
                yield return 0;
                //一帧后进行判断
            }
        }

        if (isAsync)
        {
            //同步加载包中资源
            Object res = abDic[abName].LoadAsset(resName, type);
            callBack?.Invoke(res);
        }
        else
        {
            //异步加载包中资源
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName, type);
            yield return abq;
            callBack?.Invoke(abq.asset);
        }
    }

    /// <summary>
    /// 名字 异步加载 指定资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack, bool isAsync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callBack, isAsync));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack, bool isAsync = false)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //还没有加载AB包
            if (!abDic.ContainsKey(strs[i]))
            {
                //同步加载
                if (isAsync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                //异步加载
                else
                {
                    //使用null在字典中占位 代表正在异步加载
                    abDic.Add(strs[i],null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    //异步加载结束后 替换null 代表加载结束
                    abDic[strs[i]] = req.assetBundle;
                }
            }
            //就证明字典中已经记录了一个AB包信息
            else
            {
                //字典中对应的数据如果中null 那么证明还在加载
                while (abDic[strs[i]] == null)
                {
                    //只要发现在加载中 那么就等待一帧
                    yield return 0;
                    //一帧后进行判断
                }
            }
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            //同步加载
            if (isAsync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            //异步加载
            else
            {
                //使用null在字典中占位 代表正在异步加载
                abDic.Add(abName,null);
            
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                //异步加载结束后 替换null 代表加载结束
                abDic[abName] = req.assetBundle;
            }
        }
        //就证明字典中已经记录了目标AB包信息
        else
        {
            //字典中对应的数据如果中null 那么证明还在加载
            while (abDic[abName] == null)
            {
                //只要发现在加载中 那么就等待一帧
                yield return 0;
                //一帧后进行判断
            }
        }

        if (isAsync)
        {
            //同步加载包中资源
            Object res = abDic[abName].LoadAsset(resName);
            callBack?.Invoke(res);
        }
        else
        {
            //异步加载包中资源
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName);
            yield return abq;
            callBack?.Invoke(abq.asset);
        }

    }

    /// <summary>
    /// 卸载AB包的方法
    /// </summary>
    /// <param name="name">AB包名</param>
    /// <param name="callBack">卸载是否成功</param>
    public void UnLoadAB(string name,UnityAction<bool> callBack)
    {
        if( abDic.ContainsKey(name) )
        {
            if (abDic[name] == null)
            {
                callBack(false);
                return;
            }
            abDic[name].Unload(false);
            abDic.Remove(name);
            callBack(true);
        }
    }

    //清空AB包的方法
    public void ClearAB(UnityAction<bool> callBack)
    {
        //停止所有协同程序
        StopAllCoroutines();
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        //卸载主包
        mainAB = null;
    }
}

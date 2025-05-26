using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 层级枚举
/// </summary>
public enum E_UILayer
{
    /// <summary>
    /// 底层
    /// </summary>
    Bottom,
    /// <summary>
    /// 中层
    /// </summary>
    Middle,
    /// <summary>
    /// 高层
    /// </summary>
    Top,
    /// <summary>
    /// 系统层(最高层)
    /// </summary>
    System,
}

/// <summary>
/// 管理所有UI面板的管理器
/// 注意:
/// 面板预设体要和面板类名字一致
/// </summary>
public class UIMagr : BaseManager<UIMagr>
{
    /// <summary>
    /// 主要用于里氏替换原则 在字典中用 用父类容器装载子类
    /// </summary>
    private abstract class BasePanelInfo {}
    
    /// <summary>
    /// 用于存储面板信息 和加载完成的回调函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class PanelInfo<T> : BasePanelInfo where T: BasePanel
    {
        public T panel = null;
        public UnityAction<T> callBack = null;
        //是否因为销毁而隐藏
        public bool isHide;
        
        //构造函数 方便传入回调函数
        public PanelInfo(UnityAction<T> callBack)
        {
            this.callBack += callBack;
        }
    }
    
    //UI摄像机
    private Camera uiCamera;
    //Canvas
    public Canvas uiCanvas;
    //EventSystem
    private EventSystem uiEventSystem;

    //层级父对象
    private Transform bottomLayer;
    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    /// <summary>
    /// 用于存储所有的面板对象
    /// </summary>
    private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>();
    
    private UIMagr()
    {
        //动态创建唯一的Canvas和EventSystem,摄像机
        uiCamera = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/UICamera").GetComponent<Camera>());
        //UI摄像机过场景不移除 专门用来渲染UI面板
        GameObject.DontDestroyOnLoad(uiCamera.gameObject);
        
        //动态创建Canvas对象
        uiCanvas = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
        //设置使用的UI摄像机
        uiCanvas.worldCamera = uiCamera;
        //Canvas过场景不移除
        GameObject.DontDestroyOnLoad(uiCanvas.gameObject);
        
        //找到层级父对象
        bottomLayer = uiCanvas.transform.Find("Bottom");
        middleLayer = uiCanvas.transform.Find("Middle");
        topLayer = uiCanvas.transform.Find("Top");
        systemLayer = uiCanvas.transform.Find("System");
        
        //动态创建EventSystem对象
        uiEventSystem = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
        //Canvas过场景不移除
        GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
    }

    /// <summary>
    /// 获取对应层级的父对象
    /// </summary>
    /// <param name="layer">层级枚举值(父对象名)</param>
    /// <returns></returns>
    public Transform GetLayerTransform(E_UILayer layer)
    {
        switch (layer)
        {
            case E_UILayer.Bottom:
                return bottomLayer;
            case E_UILayer.Middle:
                return middleLayer;
            case E_UILayer.Top:
                return topLayer;
            case E_UILayer.System:
                return systemLayer;
            default:
                return null;
        }
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="layer">面板显示层级</param>
    /// <param name="callBack">由于可能异步加载 所以通过委托回调的形式</param>
    /// <param name="isSyns">是否采用同步加载 默认为异步加载</param>
    /// <typeparam name="T">面板类型 要和预设体名字一致</typeparam>
    public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle, UnityAction<T> callBack = null,
        bool isSyns = false) where T: BasePanel
    {
        //获取面板名
        string panelName = typeof(T).Name;
        //存在面板
        if (panelDic.ContainsKey(panelName))
        {
            //取出字典中 占位数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //正在异步加载
            if (panelInfo.panel == null)
            {
                //如果之前显示了又隐藏 又想显示 那么就设置为false
                panelInfo.isHide = false;
                //应该等待加载完毕
                //只需要记录回调函数 加载完后调用
                if (callBack != null)
                {
                    panelInfo.callBack += callBack;
                }
            }
            //已经异步加载结束
            else
            {
                //如果是失活状态 直接激活面板即可
                if (!panelInfo.panel.gameObject.activeSelf)
                {
                    panelInfo.panel.gameObject.SetActive(true);
                }
                //如果要显示面板 那么执行一次默认的面板的显示逻辑
                panelInfo.panel.ShowMe();
                //如果存在回调函数 那么返回出去
                callBack?.Invoke(panelInfo.panel);
            }
            //存在面板就不需要执行后面的不存在步骤
            return;
        }
        //不存在面板 先存入字典当中 占位置 之后如果显示 才能得到信息判断
        panelDic.Add(panelName,new PanelInfo<T>(callBack));
        
        //不存在面板 那么就加载面板
        ABResMgr.Instance.LoadResAsync<GameObject>("ui",panelName, (res) =>
        {
            //取出字典中 占位数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            
            //表示异步加载结束前 就想隐藏该面板
            if (panelInfo.isHide)
            {
                panelDic.Remove(panelName);
                return;
            }
            
            //层级的处理
            Transform fatherTransform = GetLayerTransform(layer);
            //当层级为null代表传错的时候 默认传到中层
            if (fatherTransform == null)
            {
                fatherTransform = middleLayer;
            }
            //创建面板预设体 创建到对应的父对象下 并且保持缩放不变
            GameObject panelObj = GameObject.Instantiate(res, fatherTransform, false);

            //获取对应UI组件
            T panel = panelObj.GetComponent<T>();
            //执行默认的面板的显示逻辑
            panel.ShowMe();
            //传出去使用
            panelInfo.callBack?.Invoke(panel);
            //回调执行完 将其置空 防止内存泄漏
            panelInfo.callBack = null;
            //存储Panel
            panelInfo.panel = panel;

        },isSyns);
    }


    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="isDestory">是销毁还是失活 true销毁 默认false失活</param>
    /// <typeparam name="T">面板类型 要和预设体名字一致</typeparam>
    public void HidePanel<T>(bool isDestory = false) where T: BasePanel
    {
        //获取面板名
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            //取出字典中 占位数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //如果存在 但是正在加载中
            if (panelInfo.panel == null)
            {
                //修改隐藏表示 表示这个面板即将要隐藏
                panelInfo.isHide = true;
                //既然要隐藏了 回调函数不会使用 直接null
                panelInfo.callBack = null;
            }
            //已经加载结束
            else
            {
                panelInfo.panel.HideMe();
                //如果销毁 那么从字典中移除
                if (isDestory)
                {
                    //销毁面板
                    GameObject.Destroy(panelInfo.panel.gameObject);
                    //从字典中移除
                    panelDic.Remove(panelName);
                }
                //如果失活 那么不用从字典中移除
                else
                {
                    //失活
                    panelInfo.panel.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    /// <typeparam name="T">面板类型 要和预设体名字一致</typeparam>
    public T GetPanel<T>(UnityAction<T> callBack) where T: BasePanel
    {
        //获取面板名
        string panelName = typeof(T).Name;
        //如果字典中存在 返回面板
        if (panelDic.ContainsKey(panelName))
        {
            //取出字典中 占位数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //正在加载中
            if (panelInfo.panel == null)
            {
                //加载中等待加载结束 再传输回调函数
                panelInfo.callBack += callBack;
                return  panelDic[panelName] as T;
            }
            //加载结束 并且没有隐藏
            else if(!panelInfo.isHide)
            {
                callBack?.Invoke(panelInfo.panel);
                return  panelDic[panelName] as T;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 为控件添加自定义事件
    /// </summary>
    /// <param name="control">对应的控件</param>
    /// <param name="type">控件的类型</param>
    /// <param name="callBack">响应的函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type,
        UnityAction<BaseEventData> callBack)
    {
        //这种逻辑保证 控件上只有一个Eventtigger
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        //
        if (trigger == null)
        {
            trigger = control.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);
        
        trigger.triggers.Add(entry);
    }
}

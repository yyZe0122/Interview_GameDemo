using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器管理器
/// 主要用于开启 停止 重置等操作来管理计时器
/// </summary>
public class TimerMgr : BaseManager<TimerMgr>
{
    /// <summary>
    /// 用于记录当前将要创建的唯一ID
    /// </summary>
    private int TIME_KEY = 0;
    
    /// <summary>
    /// 用于从初管理素有计时器的字典容器(受到Time.timescale影响)
    /// </summary>
    private Dictionary<int, TimerItem> timerDic = new Dictionary<int, TimerItem>();
    
    /// <summary>
    /// 用于从初管理素有计时器的字典容器(不受Time.timescale影响)
    /// </summary>
    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>();
    
    /// <summary>
    /// 计时器管理器中的唯一计时用的协同程序的 间隔时间
    /// </summary>
    private const float intervalTime = 0.1f;
    
    //为了避免内存的浪费 每次While都会生成
    //直接将其声明为成员变量
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime);
    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTime);

    /// <summary>
    /// 字典中等待移除的数据列表
    /// </summary>
    private List<TimerItem> delList = new List<TimerItem>();
    
    /// <summary>
    /// 计时器协同程序
    /// </summary>
    private Coroutine timer;

    private Coroutine realTimer;//不受Time.timescale影响
    
    private TimerMgr()
    {
        //默认计时器就是开启的
        Start();
    }
    
    /// <summary>
    /// 开启计时器管理器的方法
    /// </summary>
    public void Start()
    {
        timer = MonoMgr.Instance.StartCoroutine(StartTiming(false, timerDic));
        realTimer = MonoMgr.Instance.StartCoroutine(StartTiming(true, realTimerDic));
    }
    
    /// <summary>
    /// 关闭计时器管理器的方法
    /// </summary>
    public void Stop()
    {
        MonoMgr.Instance.StopCoroutine(timer);
        MonoMgr.Instance.StopCoroutine(realTimer);
    }
    
    IEnumerator StartTiming(bool isRealTime, Dictionary<int, TimerItem> timeDic)
    {
        while (true)
        {
            //100毫秒进行一次计时
            //判断受不受到Time.timescale影响
            if (isRealTime)
            {
                yield return waitForSecondsRealtime;
            }
            else
            {
                yield return waitForSeconds;
            }
            
            //遍历所有的计时器 进行数据更新
            foreach (TimerItem item in timerDic.Values)
            {
                if (!item.isRuning)
                {
                    continue;
                }
                //判断是否有间隔执行的需求
                if (item.callBack != null)
                {
                    //减去100毫秒
                    item.intervalTime -= (int)(intervalTime * 1000);
                    //满足一次间隔时间执行
                    if (item.intervalTime <= 0)
                    {
                        //间隔一定时间执行一次回调
                        item.callBack.Invoke();
                        //重置间隔时间
                        item.intervalTime = item.maxIntervalTime;
                    }
                }
                //总的时间更新
                item.allTime -= (int)(intervalTime * 1000);
                //计时结束
                if (item.allTime <= 0)
                {
                    item.overCallBack.Invoke();
                    delList.Add(item);
                }
            }
            
            //移除字典等待移除中的数据
            for (int i = 0; i < delList.Count; i++)
            {
                //从字典中移除
                timerDic.Remove(delList[i].keyID);
                //放入缓存池中
                PoolMgr.Instance.PushObj(delList[i]);
            }
            //移除结束后清空列表
            delList.Clear();
        }
    }
    
    /// <summary>
    /// 创建单个计时器
    /// </summary>
    /// <param name="isRealTime">true是不受到Time.timescale影响</param>
    /// <param name="allTime">总时间(毫秒)</param>
    /// <param name="overCallBack">总时间计时结束后的委托回调</param>
    /// <param name="intervalTime">计时中间隔时间(毫秒)</param>
    /// <param name="callBack">计时中间隔时间计时的委托回调</param>
    /// <returns>返回唯一ID用于外部控制对应的计时器</returns>
    public int CreateTimer(int allTime, UnityAction overCallBack, int intervalTime = 0,
        UnityAction callBack = null, bool isRealTime = true)
    {
        //构建唯一ID
        int keyID = TIME_KEY++;
        //从缓存池取出
        TimerItem timerItem = PoolMgr.Instance.GetObjcet<TimerItem>();
        //初始化数据
        timerItem.InitInfo(keyID, allTime, overCallBack, intervalTime, callBack);
        //记录到字典
        if (isRealTime)
        {
            realTimerDic.Add(keyID,timerItem);
        }
        else
        {
            timerDic.Add(keyID,timerItem);
        }
        return keyID;
    }
    
    /// <summary>
    /// 移除单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void RemoveTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            //移除对应ID计时器 放入缓存池
            PoolMgr.Instance.PushObj(timerDic[keyID]);
            //从字典中移除
            timerDic.Remove(keyID);
            Debug.LogError("ID为" + keyID + "的计时器被移除");
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            //移除对应ID计时器 放入缓存池
            PoolMgr.Instance.PushObj(realTimerDic[keyID]);
            //从字典中移除
            realTimerDic.Remove(keyID);
            Debug.LogError("ID为" + keyID + "的计时器被移除");
        }
        else
        {
            Debug.LogError("ID为" + keyID + "的计时器不存在或已经被移除");
        }
    }
    
    /// <summary>
    /// 重置单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void ResetTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].ResetTimer();
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].ResetTimer();
        }
    }
    
    /// <summary>
    /// 开始单个计时器
    /// 主要是暂停后开始
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void StartTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRuning = true;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRuning = true;
        }
    }
    
    /// <summary>
    /// 暂停单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void PauseTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRuning = false;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRuning = false;
        }
    }
    
}

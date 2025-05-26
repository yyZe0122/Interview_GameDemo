using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器对象
/// 存储计时器相关数据
/// </summary>
public class TimerItem : IPoolClass
{
    /// <summary>
    /// 计时器唯一ID
    /// </summary>
    public int keyID;
    /// <summary>
    /// 计时器结束后的委托回调
    /// </summary>
    public UnityAction overCallBack;
    /// <summary>
    /// 计时器计时中间隔一定时间的委托回调
    /// </summary>
    public UnityAction callBack;

    /// <summary>
    ///表示计时器总的时间 毫秒
    /// </summary>
    public int allTime;
    
    /// <summary>
    /// 记录开始计时的时间 的总时间 用于时间重置 毫秒
    /// </summary>
    public int maxAllTime;

    /// <summary>
    /// 间隔执行回调的时间 毫秒
    /// </summary>
    public int intervalTime;
    /// <summary>
    /// 记录一开始的间隔时间 毫秒
    /// </summary>
    public int maxIntervalTime;
    /// <summary>
    /// 计时器是否在进行计时
    /// </summary>
    public bool isRuning;
    
    /// <summary>
    /// 初始化计时器数据
    /// </summary>
    /// <param name="timeID">唯一ID</param>
    /// <param name="allTime">总时间</param>
    /// <param name="overCallBack">总时间计时结束后的委托回调</param>
    /// <param name="intervalTime">计时中间隔时间</param>
    /// <param name="callBack">计时中间隔时间计时的委托回调</param>
    public void InitInfo(int keyID, int allTime, UnityAction overCallBack = null, int intervalTime = 0,
        UnityAction callBack = null)
    {
        this.keyID = keyID;
        this.allTime = this.maxAllTime = allTime;
        this.overCallBack = overCallBack;
        this.intervalTime = this.maxIntervalTime = intervalTime;
        this.callBack = callBack;
        this.isRuning = true;
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        this.allTime = this.maxAllTime;
        this.intervalTime = this.maxIntervalTime;
        this.isRuning = true;
    }
    
    public void ResetInfo()
    {
        //加入缓存池时 清空数据
        overCallBack = null;
        callBack = null;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏管理器 单例模式
/// </summary>
public class GameMgr : SingleAutoMono<GameMgr>
{
    private Transform heroPos;
    private Transform bPos;
    private HeroInfo heroInfo;
    /// <summary>
    /// 玩家对象 每帧更新
    /// </summary>
    public GameObject playerObj;
    
    private GameMgr()
    {
        
    }
    
    /// <summary>
    /// 切换场景 创建玩家对象
    /// </summary>
    public void InstantiatePlayerObj()
    {
        heroInfo = GameDataMgr.Instance.heroData;
        heroPos = GameObject.Find("HeroPos").transform;
        //生成玩家对象 并且将主摄像机的目标传过去
        ABResMgr.Instance.LoadResAsync<GameObject>("hero",heroInfo.heroID.ToString(), (obj) =>
        {
            GameObject player = Instantiate(obj, heroPos.position,heroPos.rotation);
            player.AddComponent<PlayerMove>();
            GameDataMgr.Instance.player = player;
            Camera.main.GetComponent<CameraMove>().SetTarget(player.transform);
        });
    }
}

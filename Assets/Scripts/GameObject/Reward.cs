using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reward : MonoBehaviour
{
    private PlayerData playerData;

    private void Start()
    {
        playerData = GameDataMgr.Instance.playerData;
    }

    /// <summary>
    /// 当玩家碰到物品时 触发
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //获得此次掉落的物品信息
            ItemData itemData = RewardTools.Instance.RandomItem();
            //向玩家的背包中添加物品
            playerData.ItemDataList.Add(itemData);
            print("添加的物品名字" + playerData.ItemDataList.Last().itemInfo.name);
            //销毁自己
            Destroy(gameObject);
        }
    }
}

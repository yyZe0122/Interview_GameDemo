using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家的数据
/// </summary>
public class PlayerData
{
    public int Lev = 1;
    public int money;
    public int nowHeroID;
    //当前武器
    public ItemData NowItemData;
    public bool isWeapon = false;
    /// <summary>
    /// 背包的武器和物品的具体数值 ItemData保存物体的属性
    /// </summary>
    public List<ItemData> ItemDataList = new List<ItemData>();
}

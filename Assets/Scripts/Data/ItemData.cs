using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包或者商店的数据 不需要保存
/// </summary>
public class ItemData
{
    /// <summary>
    /// 物品数量
    /// </summary>
    public int itemNum = 1;
    /// <summary>
    /// 物品信息
    /// </summary>
    public ItemInfo itemInfo;
    /// <summary>
    /// 附魔信息
    /// </summary>
    public AddInfo addData = new AddInfo();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 奖励掉落的工具类
/// 用来计算奖励掉落的概率
/// 用来计算附魔的属性概率
/// </summary>
public class RewardTools : BaseManager<RewardTools>
{
    private List<AddInfo> addInfos;
    private List<ItemInfo> itemInfos;
    private HeroAttInfo heroAttInfo;
    //需要掉落的物品id
    private ItemData itemData;
    //需要掉落的物品的附魔信息
    private AddInfo addData;

    private GameObject rewardObj;
    
    /// <summary>
    /// 随机掉落装备的属性
    /// </summary>
    /// <returns>返回一个装备的完整信息 可以拾取后添加到背包中</returns>
    public ItemData RandomItem()
    {
        //初始化数据
        itemData = new ItemData();
        //根据随机数来获取装备
        itemData.itemInfo = itemInfos[Random.Range(0, 9)];
        itemData.addData = RandomAdd();
        return itemData;
    }

    /// <summary>
    /// 随机附魔的属性
    /// </summary>
    public AddInfo RandomAdd()
    {
        addData = addInfos[Random.Range(0, 11)];
        addData.attNow = Random.Range(addData.attMin, addData.attMax);
        return addData;
    }
    
    /// <summary>
    /// 掉落装备游戏对象的方法
    /// </summary>
    public GameObject ItemCount()
    {
        return rewardObj;
    }
    
    /// <summary>
    /// 当前装备的属性转换的方法
    /// </summary>
    /// <param name="addInfo">需要添加到玩家上的附魔属性数据</param>
    public HeroAttInfo AttCount(AddInfo addInfo)
    {
        switch (addInfo.addAtt)
        {
            case "STR":
                CalculateAttTools.Instance.CalculateAtt(addInfo.attNow, 0, 0);
                break;
            case "DEX":
                CalculateAttTools.Instance.CalculateAtt(0, addInfo.attNow, 0);
                break;
            case "INT":
                CalculateAttTools.Instance.CalculateAtt(0, 0, addInfo.attNow);
                break;
        }
        return heroAttInfo;
    }
    
    private RewardTools()
    {
        //从Json文件中读取基础附魔数据
        addInfos = GameDataMgr.Instance.listaddInfo;
        itemInfos = GameDataMgr.Instance.listItemInfo;
        ABResMgr.Instance.LoadResAsync<GameObject>("reward","1", (obj) =>
        {
            rewardObj = obj;
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 背包物品的详细面板
/// </summary>
public class TipsPanel : BasePanel
{
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtTips;
    public TextMeshProUGUI txtAtt;
    
    private PlayerData playerData;
    private ItemData itemData;
    public override void ShowMe()
    {
        
    }

    public override void HideMe()
    {
        
    }
    
    /// <summary>
    /// 更新介绍的文本
    /// </summary>
    /// <param name="itemID"></param>
    public void ChangeTips(int itemID)
    {
        //获取玩家数据
        playerData = GameDataMgr.Instance.playerData;
        if (itemID == -1)
        {
            //判断是不是装备栏
            itemData = playerData.NowItemData;
        }
        else
        {
            itemData = playerData.ItemDataList[itemID];
        }
        //更新
        txtName.text = itemData.itemInfo.name;
        txtTips.text = itemData.itemInfo.tips;
        //更新附魔信息
        if (itemData.addData != null)
        {
            if (itemData.itemInfo.id == 10 || itemData.itemInfo.id == 11)
            {
                txtAtt.text = "?,药水要附什么魔";
                return;
            }
            txtAtt.text = itemData.addData.name + "增加的数值为:" + itemData.addData.attNow;
        }
        else
        {
            txtAtt.text = "无附魔";
        }
    }
    
}

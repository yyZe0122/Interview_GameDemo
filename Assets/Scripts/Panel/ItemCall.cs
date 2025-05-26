using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCall : BasePanel
{
    private GameObject obj;
    public GameObject iconObj;
    private PlayerData playerData = new PlayerData();
    public TextMeshProUGUI txtNum;
    public Image imageIcon;
    public int NowItemCallID;
    //当前是否有装备
    public bool isHaveWeapon = false;
    
    public override void ShowMe()
    {
        
    }

    public override void HideMe()
    {
        
    }

    private void Start()
    {
        //监听自定义事件
        //鼠标进入
        UIMagr.AddCustomEventListener(GetControl<Image>("imageIcon"), EventTriggerType.PointerEnter, (data) =>
        {
            EventCenter.Instance.EventTrigger<ItemCall>(E_EventType.E_Bag_PointerEnter, this);
        });
        //鼠标移出
        UIMagr.AddCustomEventListener(GetControl<Image>("imageIcon"), EventTriggerType.PointerExit, (data) =>
        {
            EventCenter.Instance.EventTrigger<ItemCall>(E_EventType.E_Bag_PointerExit, this);
        });
        //拖拽部分
        UIMagr.AddCustomEventListener(GetControl<Image>("imageIcon"), EventTriggerType.BeginDrag, (data) =>
        {
            EventCenter.Instance.EventTrigger<ItemCall>(E_EventType.E_Bag_BeginDrag, this);
        });
        UIMagr.AddCustomEventListener(GetControl<Image>("imageIcon"), EventTriggerType.Drag, (data) =>
        {
            EventCenter.Instance.EventTrigger<BaseEventData>(E_EventType.E_Bag_Drag, data);
        });
        UIMagr.AddCustomEventListener(GetControl<Image>("imageIcon"), EventTriggerType.EndDrag, (data) =>
        {
            EventCenter.Instance.EventTrigger<ItemCall>(E_EventType.E_Bag_EndDrag, this);
        });
    }

    /// <summary>
    /// 更新格子的数量和图片
    /// </summary>
    /// <param name="itemID"></param>
    public void ChangeItemCall(int itemID)
    {
        playerData = GameDataMgr.Instance.playerData;
        //更新数量
        txtNum.text = playerData.ItemDataList[itemID].itemNum.ToString();
        //更新图片
        ABResMgr.Instance.LoadResAsync<Sprite>("itemicon",playerData.ItemDataList[itemID].itemInfo.icon.ToString(), (image) =>
        { 
            NowItemCallID = itemID; 
            imageIcon.sprite = image;
        });
    }
    
}

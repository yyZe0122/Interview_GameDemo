using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StorePanel : BasePanel
{
    //格子相关
    public Transform contont;
    private PlayerData playerData;
    private ItemCall itemCall;
    private List<ItemCall> itemCallList = new List<ItemCall>();
    private List<ItemInfo> itemInfoList = new List<ItemInfo>();
    //是否存在药水
    private bool isHave = false;
    
    public override void ShowMe()
    {
        //先获取玩家数据
        playerData = GameDataMgr.Instance.playerData;
        itemInfoList = GameDataMgr.Instance.listItemInfo;
        //更新格子
        ChangeItemCall();
    }

    public override void HideMe()
    {
        
    }

    /// <summary>
    /// 更新背包格子
    /// </summary>
    public void ChangeItemCall()
    {
        //删除之前的格子
        if (itemCallList != null)
        {
            for (int i = 0; i < itemCallList.Count; i++)
            {
                Destroy(itemCallList[i].gameObject);
            }
            //清空
            itemCallList.Clear();
        }
        print("背包格子数量" + playerData.ItemDataList.Count);
        //动态创建创建新的格子
        for (int i = 0; i < playerData.ItemDataList.Count; i++)
        {
            ABResMgr.Instance.LoadResAsync<GameObject>("ui", "ItemCall", (item) =>
            {
                //将格子添加到列表中
                ItemCall itemCall = Instantiate(item.GetComponent<ItemCall>(), contont);
                //初始化当前格子上的ItemCall组件
                itemCall.ChangeItemCall(i);
                //存进List
                itemCallList.Add(itemCall);
            });
        }
    }

    /// <summary>
    /// 购买道具的逻辑
    /// </summary>
    /// <param name="id"></param>
    private void BuyForBag(int id)
    {
        for (int i = 0; i < playerData.ItemDataList.Count; i++)
        {
            if (playerData.ItemDataList[i].itemInfo.id == id)
            {
                playerData.ItemDataList[i].itemNum += 1;
                isHave = true;
            }
        }
        //如果没有就添加药水
        if (!isHave)
        { 
            ItemData noHaveItemData = new ItemData();
            noHaveItemData.itemInfo = itemInfoList[id - 1];
            noHaveItemData.itemNum = 1;
            noHaveItemData.addData = new AddInfo();
            playerData.ItemDataList.Add(noHaveItemData);
        }
        isHave = false;
        ChangeItemCall();
    }

    /// <summary>
    /// 出售装备的逻辑
    /// </summary>
    public void SaleForBag()
    {
        /*//LinQ查询
        var q = from ItemData nowItemData in playerData.ItemDataList
            where nowItemData = 
            select s;
        ChangeItemCall();*/
    }
    
    protected override void ClickButton(string buttonName)
    {
        switch (buttonName)
        {
            case "btnStoreExit":
                UIMagr.Instance.HidePanel<StorePanel>();
                break;
            case "btnBuy1":
                BuyForBag(10);
                break;
            case "btnBuy2":
                BuyForBag(11);
                break;
            case "btnSale":
                SaleForBag();
                break;
            
        }
    }
}

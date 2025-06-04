using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RolePanel : BasePanel
{
    //属性相关
    private TextMeshProUGUI txtSTR;
    private TextMeshProUGUI txtDEX;
    private TextMeshProUGUI txtINT;
    private TextMeshProUGUI txtHp;
    private TextMeshProUGUI txtDef;
    private TextMeshProUGUI txtMp;
    private TextMeshProUGUI txtMeleeAtk;
    private TextMeshProUGUI txtRemoteAtk;
    private TextMeshProUGUI txtMagicAtk;
    
    private HeroInfo heroData;
    private HeroAttInfo heroAttInfo = new HeroAttInfo();
    
    //背包格子相关
    private ItemData nowCallItemData;
    public Transform contont;
    private List<ItemCall> itemCallList = new List<ItemCall>();
    private PlayerData playerData;
    private ItemCall itemCall;
    
    public override void ShowMe()
    {
        txtSTR = GetControl<TextMeshProUGUI>("txtSTR");
        txtDEX = GetControl<TextMeshProUGUI>("txtDEX");
        txtINT = GetControl<TextMeshProUGUI>("txtINT");
        txtHp = GetControl<TextMeshProUGUI>("txtHp");
        txtDef = GetControl<TextMeshProUGUI>("txtDef");
        txtMp = GetControl<TextMeshProUGUI>("txtMp");
        txtMeleeAtk = GetControl<TextMeshProUGUI>("txtMeleeAtk");
        txtRemoteAtk = GetControl<TextMeshProUGUI>("txtRemoteAtk");
        txtMagicAtk = GetControl<TextMeshProUGUI>("txtMagicAtk");
        
        //先获取玩家数据
        playerData = GameDataMgr.Instance.playerData;
        //更新角色属性
        ChangeAttPanel();
        //更新背包格子
        ChangeItemCall();
    }

    public override void HideMe()
    {
        
    }

    /// <summary>
    /// 更新角色面板上的属性
    /// </summary>
    public void ChangeAttPanel()
    {
        //读取英雄属性
        GameDataMgr.Instance.LoadHeroData();
        heroData = GameDataMgr.Instance.heroData;
        //判断玩家是否装备武器
        if (!playerData.isWeapon)
        {
            heroAttInfo = CalculateAttTools.Instance.CalculateAtt(heroData.STR, heroData.DEX, heroData.INT);
            Debug.Log("玩家没有装备武器");
        }
        else
        {
            //判断玩家装备的武器附魔数据
            switch (playerData.NowItemData.addData.addAtt)
            {
                case "STR":
                    heroAttInfo = CalculateAttTools.Instance.CalculateAtt(heroData.STR + playerData.NowItemData.addData.attNow, heroData.DEX, heroData.INT);
                    break;
                case "DEX":
                    heroAttInfo = CalculateAttTools.Instance.CalculateAtt(heroData.STR, heroData.DEX + playerData.NowItemData.addData.attNow, heroData.INT);
                    break;
                case "INT":
                    heroAttInfo = CalculateAttTools.Instance.CalculateAtt(heroData.STR, heroData.DEX, heroData.INT + playerData.NowItemData.addData.attNow);
                    break;
            }
        }
        txtSTR.text = heroData.STR.ToString();
        txtDEX.text = heroData.DEX.ToString();
        txtINT.text = heroData.INT.ToString();
        txtHp.text = heroAttInfo.hp.ToString();
        txtDef.text = heroAttInfo.def.ToString();
        txtMp.text = heroAttInfo.mp.ToString();
        txtMeleeAtk.text = heroAttInfo.meleeAtk.ToString();
        txtRemoteAtk.text = heroAttInfo.remoteAtk.ToString();
        txtMagicAtk.text = heroAttInfo.magicAtk.ToString();
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
    

    protected override void ClickButton(string buttonName)
    {
        switch (buttonName)
        {
            case "btnRoleExit":
                UIMagr.Instance.HidePanel<RolePanel>();
                break;
        }
    }
}

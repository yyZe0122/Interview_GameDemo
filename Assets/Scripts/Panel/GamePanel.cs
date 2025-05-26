using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    private TextMeshProUGUI txtLev;
    private TextMeshProUGUI txtMoney;
    private TextMeshProUGUI txtETigger;
    private Image imageHp;
    private Image imageMp;
    private TextMeshProUGUI txtHp;
    private TextMeshProUGUI txtMp;
    private PlayerData playerData;
    //血量蓝量面板
    private HeroInfo heroData;
    private int maxHp;
    private int maxMp;
    private int nowHp;
    private int nowMp;
    
    public override void ShowMe()
    {
        //初始化一些对象
        AtkOrHit.Instance.Init();
        BagMgr.Instance.Init();
        playerData = GameDataMgr.Instance.playerData;
        heroData = GameDataMgr.Instance.heroData;
        maxHp = heroData.STR * 2;
        maxMp = heroData.INT * 2;
        nowHp = maxHp;
        nowMp = maxMp;
        //获取UI组件
        txtLev = GetControl<TextMeshProUGUI>("txtLev");
        txtMoney = GetControl<TextMeshProUGUI>("txtMoney");
        txtETigger = GetControl<TextMeshProUGUI>("txtETigger");
        imageHp = GetControl<Image>("imageHp");
        imageMp = GetControl<Image>("imageMp");
        
        txtHp = GetControl<TextMeshProUGUI>("txtHp");
        txtMp = GetControl<TextMeshProUGUI>("txtMp");
        //更新等级和金钱的面板显示数据
        txtLev.text = playerData.Lev.ToString();
        txtMoney.text = playerData.money.ToString();
        ChangeHpMp(0,0);
        //监听怪物死亡事件
        EventCenter.Instance.AddEventListener(E_EventType.E_Monster_Dead, () =>
        {
            playerData.money += 10;
            txtMoney.text = playerData.money.ToString();
        });
        //监听玩家受伤事件
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_Player_Hit, (hp) =>
        {
            ChangeHpMp(hp,0);
        });
    }
    
    public override void HideMe()
    {

    } 

    /// <summary>
    /// 玩家接触触发器时候显示的文字
    /// </summary>
    public void ETiggerPlayerText(string txt)
    {
        txtETigger.text = txt;
    }

    /// <summary>
    /// 玩家更新血量和魔法值
    /// </summary>
    private void ChangeHpMp(int hitHp,int hitMp)
    {
        nowHp -= hitHp;
        nowMp -= hitMp;
        //更新血量和魔法值的UI
        imageHp.fillAmount = nowHp / maxHp;
        imageMp.fillAmount = maxMp / nowMp;
        txtHp.text = nowHp + "/" + maxHp;
        txtMp.text = nowMp + "/" + maxMp;
    }
    
    protected override void ClickButton(string buttonName)
    {
        switch (buttonName)
        {
            case "btnExitEntrance":
                UIMagr.Instance.ShowPanel<ReturnMainPanel>();
                EventCenter.Instance.ClearEvent();
                break;
            case "btnSet":
                UIMagr.Instance.ShowPanel<SetPanel>();
                Time.timeScale = 0;
                break;
            case "btnRole":
                UIMagr.Instance.ShowPanel<RolePanel>();
                break;
        }
    }
    
}

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

    private TextMeshProUGUI txtSkill1;
    private TextMeshProUGUI txtSkill2;
    private TextMeshProUGUI txtSkill3;
    private TextMeshProUGUI txtSkill4;
    private Image ImageSkill1;
    private Image ImageSkill2;
    private Image ImageSkill3;
    private Image ImageSkill4;
    private Image ImageSkill1CD;
    private Image ImageSkill2CD;
    private Image ImageSkill3CD;
    private Image ImageSkill4CD;
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
        
        txtSkill1 = GetControl<TextMeshProUGUI>("txtSkill1");
        txtSkill2 = GetControl<TextMeshProUGUI>("txtSkill2");
        txtSkill3 = GetControl<TextMeshProUGUI>("txtSkill3");
        txtSkill4 = GetControl<TextMeshProUGUI>("txtSkill4");
        ImageSkill1 = GetControl<Image>("ImageSkill1");
        ImageSkill2 = GetControl<Image>("ImageSkill2");
        ImageSkill3 = GetControl<Image>("ImageSkill3");
        ImageSkill4 = GetControl<Image>("ImageSkill4");
        ImageSkill1CD = GetControl<Image>("ImageSkill1CD");
        ImageSkill2CD = GetControl<Image>("ImageSkill2CD");
        ImageSkill3CD = GetControl<Image>("ImageSkill3CD");
        ImageSkill4CD = GetControl<Image>("ImageSkill4CD");
        //更新等级和金钱的面板显示数据
        txtLev.text = playerData.Lev.ToString();
        txtMoney.text = playerData.money.ToString();
        ChangeHpMp();
        SkillIcon(heroData.heroID);
        //监听装备更新事件
        EventCenter.Instance.AddEventListener(E_EventType.E_Player_ChangeWeapon, () =>
        {
            GameDataMgr.Instance.LoadHeroData();
            heroData = GameDataMgr.Instance.heroData;
            maxHp = heroData.STR * 2;
            Debug.Log(maxHp);
            maxMp = heroData.INT * 2;
            Debug.Log(maxMp);
            ChangeHpMp();
        });
        //监听怪物死亡事件
        EventCenter.Instance.AddEventListener(E_EventType.E_Monster_Dead, () =>
        {
            playerData.money += 10;
            txtMoney.text = playerData.money.ToString();
        });
        //监听玩家受伤事件
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_Player_Hit, (hp) =>
        {
            ChangeHpMp(hp);
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
    public void ChangeHpMp(int hitHp = 0,int hitMp = 0)
    {
        nowHp -= hitHp;
        nowMp -= hitMp;
        //更新血量和魔法值的UI
        imageHp.fillAmount = nowHp / maxHp;
        imageMp.fillAmount = maxMp / nowMp;
        txtHp.text = nowHp + "/" + maxHp;
        txtMp.text = nowMp + "/" + maxMp;
    }

    public void SkillIcon(int heroId)
    {
        Debug.Log("读取技能图标");
        //技能图片资源的地址
        ABResMgr.Instance.LoadResAsync<Sprite>("skillicon", heroId + "1", (image) =>
        {
            ImageSkill1.sprite = image;
            Debug.Log("读取技能图标" + heroId + "1");
        });
        ABResMgr.Instance.LoadResAsync<Sprite>("skillicon", heroId + "2", (image) =>
        {
            ImageSkill2.sprite = image;
            Debug.Log("读取技能图标" + heroId + "2");
        });
        ABResMgr.Instance.LoadResAsync<Sprite>("skillicon", heroId + "3", (image) =>
        {
            ImageSkill3.sprite = image;
            Debug.Log("读取技能图标" + heroId + "3");
        });
        ABResMgr.Instance.LoadResAsync<Sprite>("skillicon", heroId + "4", (image) =>
        {
            ImageSkill4.sprite = image;
            Debug.Log("读取技能图标" + heroId + "4");
        });
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

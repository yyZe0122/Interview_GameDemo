using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// 专门管理游戏数据的类
/// </summary>
public class GameDataMgr : BaseManager<GameDataMgr>
{
    //音乐音效数据
    public MusicData musicData;
    //初始的英雄属性
    public List<HeroInfo> heroInfo;
    //英雄数据
    public HeroInfo heroData;
    //记录选择的角色ID 用于游戏中使用
    public int nowHeroInfoID;
    //初始未改过按键的键盘按键数据
    public KeyInfo keyInfo;
    
    //初始化基础装备属性
    public List<ItemInfo> listItemInfo;
    //初始化附魔属性
    public List<AddInfo> listaddInfo;
    //现在的附魔数据
    public List<AddInfo> addDataList = new List<AddInfo>();
    //现在的物品数据
    public List<ItemData> itemDataList = new List<ItemData>();
    //现在的玩家数据
    public PlayerData playerData;
    //玩家的上一个武器数据
    public ItemData lastItemData = new ItemData();
    //当前生成的玩家对象
    public GameObject player;
    
    
    /// <summary>
    /// 构造函数
    /// 初始化数据
    /// </summary>
    private GameDataMgr()
    {
        //初始化音乐音效数据
        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData");
        //初始化初始的英雄数据
        heroInfo = JsonMgr.Instance.LoadData<List<HeroInfo>>("HeroInfo");
        //初始化基础装备属性
        listItemInfo = JsonMgr.Instance.LoadData<List<ItemInfo>>("ItemInfo");
        //初始化附魔数据
        listaddInfo = JsonMgr.Instance.LoadData<List<AddInfo>>("AddInfo");
        Debug.Log(Application.persistentDataPath);
        //读取玩家数据
        playerData = JsonMgr.Instance.LoadData<PlayerData>("PlayerData");
        
        //测试
        /*for (int i = 0; i < listItemInfo.Count; i++)
        {
            ItemData item = new ItemData();
            item.addData = new AddInfo();
            item.itemInfo = listItemInfo[i];
            playerData.ItemDataList.Add(item);
        }
        for (int i = 0; i < playerData.ItemDataList.Count; i++)
        {
            Debug.Log(playerData.ItemDataList[i].itemInfo.name);
        }*/
    }
    

    /// <summary>
    /// 存储音乐音效数据
    /// </summary>
    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicData,"MusicData");
    }

    /// <summary>
    /// 新的游戏初始化玩家英雄数据
    /// </summary>
    public void LoadFirstHeroData()
    {
        //新的游戏初始化玩家选择的英雄数据
        heroData = heroInfo[--nowHeroInfoID];
        SaveHeroData();
        playerData.nowHeroID = nowHeroInfoID;
    }

    /// <summary>
    /// 读取已经游玩的玩家英雄数据
    /// </summary>
    public void LoadHeroData()
    {
        //初始化玩家选择的英雄数据
        heroData = JsonMgr.Instance.LoadData<HeroInfo>("HeroData");
    }
    
    /// <summary>
    /// 更新已经游玩的玩家英雄数据
    /// </summary>
    public void ChangeHeroData(bool isNowHaveWeapon)
    {
        if (isNowHaveWeapon)
        {
            //减去上一个的装备数据
            switch (lastItemData.addData.addAtt)
            {
                case "STR":
                    heroData.STR -= lastItemData.addData.attNow;
                    break;
                case "DEX":
                    heroData.DEX -= lastItemData.addData.attNow;
                    break;
                case "INT":
                    heroData.INT -= lastItemData.addData.attNow;
                    break;
            }
        }
        //更新玩家选择的英雄数据
        switch (playerData.NowItemData.addData.addAtt)
        {
            case "STR":
                heroData.STR += playerData.NowItemData.addData.attNow;
                break;
            case "DEX":
                heroData.DEX += playerData.NowItemData.addData.attNow;
                break;
            case "INT":
                heroData.INT += playerData.NowItemData.addData.attNow;
                break;
        }
        SaveHeroData();
    }
    
    /// <summary>
    /// 存储玩家游玩过的英雄数据
    /// </summary>
    public void SaveHeroData()
    {
        JsonMgr.Instance.SaveData(heroData,"HeroData");
    }

    /// <summary>
    /// 读取初始的键盘输入
    /// </summary>
    public void LoadFirstKeyCode()
    {
        keyInfo = JsonMgr.Instance.LoadData<KeyInfo>("KeyInfo");
        SaveKeyCode();
    }
    
    /// <summary>
    /// 读取改键后的键盘输入
    /// </summary>
    public void LoadKeyCode()
    {
        keyInfo = JsonMgr.Instance.LoadData<KeyInfo>("KeyData");
    }
    
    /// <summary>
    /// 保存改键后的键盘
    /// </summary>
    public void SaveKeyCode()
    {
        JsonMgr.Instance.SaveData(keyInfo,"KeyData");
    }
    
    /// <summary>
    /// 保存玩家数据
    /// </summary>
    public void SavePlayerData()
    {
        JsonMgr.Instance.SaveData(playerData,"PlayerData");
    }
    
    /// <summary>
    /// 重置玩家数据
    /// </summary>
    public void FirstPlayerData()
    {
        playerData.Lev = 1;
        playerData.money = 0;
        playerData.isWeapon = false;
        playerData.NowItemData = new ItemData();
        playerData.ItemDataList.Clear();
        SavePlayerData();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseHeroPanel : BasePanel
{
    private Transform heroPos;
    private int heroID = 1;
    private GameObject obj;
    private TextMeshProUGUI txtHeroName;
    private List<HeroInfo> heroInfo;
    public override void ShowMe()
    {
        heroInfo = GameDataMgr.Instance.heroInfo;
        heroPos = GameObject.Find("HeroPos").transform;
        txtHeroName = GetControl<TextMeshProUGUI>("txtHeroName");
        txtHeroName.name = heroInfo[0].heroName;
        ShowHero(heroID);
    }

    public override void HideMe()
    {
        
    }

    public void ShowHero(int heroID)
    {
        //关闭输入检测 防止选择英雄按键盘时候乱跑
        InputMgr.Instance.StartOrEndInputMgr(false);
        if (heroID < 1 )
        {
            heroID = 4;
        }
        if (heroID > 4)
        {
            heroID = 1;
        }
        this.heroID = heroID;
        ABResMgr.Instance.LoadResAsync<GameObject>("hero", heroID.ToString(), (T) =>
        {
            if (obj != null)
            {
                Destroy(obj);
                obj = null;
            }
            obj = Instantiate(T, heroPos);
            txtHeroName.text = heroInfo[--heroID].heroName;
        });
    }

    protected override void ClickButton(string buttonName)
    {
        switch (buttonName)
        {
            case "btnChooseExit":
                UIMagr.Instance.HidePanel<ChooseHeroPanel>();
                //要让摄像机返回主菜单位置
                Camera.main.GetComponent<BeginCamera>().FirstOrLast(true);
                //显示开始面板
                UIMagr.Instance.ShowPanel<BeginPanel>();
                break;
            
            case "btnLeft":
                ShowHero(--heroID);
                break;
            
            case "btnRight":
                ShowHero(++heroID);
                break;
            
            case "btnChooseHero":
                UIMagr.Instance.HidePanel<ChooseHeroPanel>();
                //传出当前选择的英雄id
                GameDataMgr.Instance.nowHeroInfoID = heroID;
                //重置游戏数据
                LoadFirstPlayerData();
                //进入游戏场景
                SceneMgr.Instance.LoadSceneAsync("EntranceScene", () =>
                {
                    GameMgr.Instance.InstantiatePlayerObj();
                });
                break;
        }
    }

    /// <summary>
    /// 读取新的游戏数据
    /// </summary>
    private void LoadFirstPlayerData()
    {
        //读取新的英雄基础数据
        GameDataMgr.Instance.LoadFirstHeroData();
        //判断是不是第一次打开游戏
        if (!PlayerPrefs.HasKey("FirstLaunch")) 
        {
            // 首次启动逻辑（如展示新手引导）
            PlayerPrefs.SetInt("FirstLaunch", 1);
            PlayerPrefs.Save();  // 立即保存，防止异常退出导致未写入
            GameDataMgr.Instance.LoadFirstKeyCode();
        }
        //读取按键绑定
        GameDataMgr.Instance.LoadKeyCode();
        //读取新的游戏数据
        GameDataMgr.Instance.FirstPlayerData();
    }
}

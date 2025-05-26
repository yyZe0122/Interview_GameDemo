using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public override void ShowMe()
    {
        
    }

    public override void HideMe()
    {
        
    }
    
    protected override void ClickButton(string buttonName)
    {
        //通过控件名字来处理事件
        switch (buttonName)
        {
            case "btnNewPlay":
                UIMagr.Instance.ShowPanel<AreYouSureNewGamePanel>();
                break;
            case "btnPlay":
                UIMagr.Instance.HidePanel<BeginPanel>();
                //读取存档
                LoadPlayerData();
                //进入场景
                SceneMgr.Instance.LoadSceneAsync("EntranceScene", () =>
                {
                    GameMgr.Instance.InstantiatePlayerObj();
                });
                break;
            case "btnSet": 
                UIMagr.Instance.ShowPanel<SetPanel>();
                break;
            case "btnQuit": 
                Application.Quit();
                print("退出游戏");
                break;
        }
    }

    /// <summary>
    /// 读取玩家数据
    /// </summary>
    private void LoadPlayerData()
    {
        //读取存档
        GameDataMgr.Instance.LoadHeroData();
        GameDataMgr.Instance.LoadKeyCode();
    }
}

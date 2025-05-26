using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadPanel : BasePanel
{
    public override void ShowMe()
    {
        Time.timeScale = 0;
        PoolMgr.Instance.ClearPool();
        EventCenter.Instance.ClearEvent();
        GameDataMgr.Instance.SavePlayerData();
    }

    public override void HideMe()
    {
        Time.timeScale = 1;
    }

    protected override void ClickButton(string buttonName)
    {
        switch (buttonName)
        {
            //返回主菜单
            case "btnMain":
                //进入主菜单场景
                UIMagr.Instance.HidePanel<DeadPanel>();
                UIMagr.Instance.HidePanel<GamePanel>();
                SceneMgr.Instance.LoadSceneAsync("BeginScene", () =>
                {
                    
                });
                break;
            //返回安全区
            case "btnStore":
                //进入安全区场景
                UIMagr.Instance.HidePanel<DeadPanel>();
                UIMagr.Instance.HidePanel<GamePanel>();
                SceneMgr.Instance.LoadSceneAsync("EntranceScene", () =>
                {
                    GameMgr.Instance.InstantiatePlayerObj();
                });
                break;
        }
    }
}

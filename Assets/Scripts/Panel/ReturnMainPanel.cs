using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMainPanel : BasePanel
{
    public override void ShowMe()
    {
        Time.timeScale = 0;
    }

    public override void HideMe()
    {
        Time.timeScale = 1;
    }

    protected override void ClickButton(string buttonName)
    {
        switch (buttonName)
        {
            case "btnYes":
                //返回主菜单场景
                SceneMgr.Instance.LoadSceneAsync("BeginScene", () =>
                {
                    UIMagr.Instance.HidePanel<ReturnMainPanel>();
                    UIMagr.Instance.HidePanel<GamePanel>();
                    PoolMgr.Instance.ClearPool();
                    EventCenter.Instance.ClearEvent();
                });
                break;
            case "btnNo":
                UIMagr.Instance.HidePanel<ReturnMainPanel>();
                break;
        }
    }
}

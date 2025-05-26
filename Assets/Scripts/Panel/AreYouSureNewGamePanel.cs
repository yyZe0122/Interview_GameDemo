using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreYouSureNewGamePanel : BasePanel
{
    public override void ShowMe()
    {
        
    }

    public override void HideMe()
    {
        
    }

    protected override void ClickButton(string buttonName)
    {
        switch (buttonName)
        {
            case "btnYes":
                UIMagr.Instance.HidePanel<AreYouSureNewGamePanel>();
                UIMagr.Instance.HidePanel<BeginPanel>();
                //摄像机动画播放完毕后显示选角面板
                Camera.main.GetComponent<BeginCamera>().FirstOrLast(false,() =>
                {
                    //显示选角面板
                    UIMagr.Instance.ShowPanel<ChooseHeroPanel>();
                });
                break;
            case "btnNo":
                UIMagr.Instance.HidePanel<AreYouSureNewGamePanel>();
                break;
        }
    }
    
    
}

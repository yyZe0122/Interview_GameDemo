using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementPanel : BasePanel
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
            case "btnAchievementExit":
                UIMagr.Instance.HidePanel<AchievementPanel>();
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPanel : BasePanel
{
    public override void ShowMe()
    {
        
    }

    public override void HideMe()
    {
        Time.timeScale = 1;
    }

    protected override void ClickButton(string buttonName)
    {
        switch (buttonName)
        {
            case "btnSetExit":
                UIMagr.Instance.HidePanel<SetPanel>();
                break;
            case "btnMusic":
                UIMagr.Instance.ShowPanel<MusicPanel>();
                break;
            case "btnKeyboard": 
                print("btnKeyboard");
                break;
        }
    }
}

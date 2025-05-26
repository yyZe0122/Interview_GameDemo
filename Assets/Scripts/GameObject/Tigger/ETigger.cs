using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触发器游戏对象脚本
/// </summary>
public class ETigger : MonoBehaviour
{
    //判断是不是进入触发器
    private void OnTriggerEnter(Collider collider)
    {
        switch (gameObject.name)
        { 
            case "e1":
                EventCenter.Instance.EventTrigger(E_EventType.E_Tigger_E1);
                break;
               
            case "e2":
                UIMagr.Instance.GetPanel<GamePanel>((panel) =>
                { 
                    panel.ETiggerPlayerText("按下 E 键打开成就中心");
                });
                break;
            case "e3":
                UIMagr.Instance.GetPanel<GamePanel>((panel) =>
                { 
                    panel.ETiggerPlayerText("按下 E 键打开商店");
                });
                break;
        }
    }

    //判断是不是离开触发器
    private void OnTriggerExit(Collider collider)
    {
            switch (gameObject.name)
            {
                case "e2":
                    UIMagr.Instance.GetPanel<GamePanel>((panel) =>
                    {
                        panel.ETiggerPlayerText("");
                    });
                    break;
                case "e3":
                    UIMagr.Instance.GetPanel<GamePanel>((panel) =>
                    {
                        panel.ETiggerPlayerText("");
                    });
                    break;
            }
        
    }
    
    //判断是不是相交触发器
    private void OnTriggerStay(Collider collider)
    {
            //判断触发器是否触发 触发的话通知事件中心
            //判断是不是在触发器里面按下键盘E
            if (Input.GetKeyDown(KeyCode.E))
            {
                //用名字来判断触发的事件
                switch (gameObject.name)
                {
                    case "e2":
                        EventCenter.Instance.EventTrigger(E_EventType.E_Tigger_E2);
                        break;
                    case "e3":
                        EventCenter.Instance.EventTrigger(E_EventType.E_Tigger_E3);
                        break;
                }
            }
    }
    
}

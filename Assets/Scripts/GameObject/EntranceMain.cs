using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceMain : MonoBehaviour
{
    void Awake()
    {
        //显示玩家的游戏面板
        UIMagr.Instance.ShowPanel<GamePanel>();
        //添加场景交互触发器监听
        //用来显示面板或者切换场景
        EventCenter.Instance.AddEventListener(E_EventType.E_Tigger_E1, () =>
        {
            EventCenter.Instance.ClearEvent();
            UIMagr.Instance.HidePanel<GamePanel>();
            //进入游戏场景
            SceneMgr.Instance.LoadSceneAsync("GameScene", () =>
            {
                GameMgr.Instance.InstantiatePlayerObj();
            });
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Tigger_E2, () =>
        {
            UIMagr.Instance.ShowPanel<AchievementPanel>();
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Tigger_E3, () =>
        {
            UIMagr.Instance.ShowPanel<StorePanel>();
        });
    }
}

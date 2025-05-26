using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public Transform bossTransform;
    public Transform heroTransform;
    public GameObject door;
    private bool isOpen = true;
    public GameObject bossDoor;
    void Awake()
    {
        BagMgr.Instance.Init();
        //显示游戏面板
        UIMagr.Instance.ShowPanel<GamePanel>();
        //添加监听door事件
        EventCenter.Instance.AddEventListener(E_EventType.E_Tigger_Door, () =>
        {
            if (isOpen)
            {
                door.transform.position = new Vector3(35, 0, -13.5f);
                isOpen = false;
            }
        });
        //监听Boss死亡事件
        EventCenter.Instance.AddEventListener(E_EventType.E_Monster_BossDead, () =>
        {
            Invoke("BossDoorOpne",3f);
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Tigger_BossDoor, () =>
        {
            UIMagr.Instance.HidePanel<GamePanel>();
            PoolMgr.Instance.ClearPool();
            EventCenter.Instance.ClearEvent();
            //保存数据
            GameDataMgr.Instance.SavePlayerData();
            //Boss死亡后开启的传送门激活 然后返回安全区场景
            SceneMgr.Instance.LoadSceneAsync("EntranceScene", () =>
            {
                GameMgr.Instance.InstantiatePlayerObj();
            });
        });
        
        
        //监听需要音效的事件
        EventCenter.Instance.AddEventListener(E_EventType.E_Monster_Dead, () =>
        {
            MusicMgr.Instance.PlaySound("monster/dead1");
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Monster_BossDead, () =>
        {
            MusicMgr.Instance.PlaySound("monster/dead2");
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Tigger_Door, () =>
        {
            MusicMgr.Instance.PlaySound("other/doorclose");
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Tigger_Door, () =>
        {
            MusicMgr.Instance.PlaySound("other/doorclose");
        });
        
    }

    public void BossDoorOpne()
    {
        MusicMgr.Instance.PlaySound("other/bossdoor");
        //Boss死亡后 传送门激活
        bossDoor.SetActive(true);
    }
    
}

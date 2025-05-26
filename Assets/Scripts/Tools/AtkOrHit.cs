using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击或者受伤的方法
/// </summary>
public class AtkOrHit : SingleAutoMono<AtkOrHit>
{
    private HeroInfo heroInfo;

    public void Init()
    {
        heroInfo = GameDataMgr.Instance.heroData;
        Skill2();
        Skill3();
        Skill4();
    }

    private AtkOrHit()
    {

    }
    
    /// <summary>
    /// 自己攻击 对方受伤的方法
    /// </summary>
    /// <param name="atk"></param>
    /// <param name="def"></param>
    /// <param name="defhp">对方剩余血量</param>
    /// <param name="hitAnimator">受伤方的动画组件</param>
    /// <returns>返回受到攻击后的血量</returns>
    public int Hit(int atk, int hitdef, int defhp ,Animator hitAnimator)
    {
        //如果是玩家的血量
        if (hitAnimator.gameObject.GetComponent<PlayerMove>() != null)
        {
            defhp = hitAnimator.gameObject.GetComponent<PlayerMove>().playerNowHp;
        }
        if (atk - hitdef <= 0)
        {
            defhp -= 1;
        }
        else
        {
            defhp -= atk - hitdef;
        }
        hitAnimator.SetTrigger("hit");
        //判断是怪物还是玩家死亡
        if ( defhp <= 0 )
        {
            //是怪物
            if (hitAnimator.gameObject.GetComponent<MonsterMove>() !=null)
            {
                //防止死了被再次攻击
                if (!hitAnimator.gameObject.GetComponent<MonsterMove>().isDead)
                {
                    if (hitAnimator.gameObject.GetComponent<MonsterType>().id == 4)
                    {
                        Debug.Log("Boss死亡!!!!!!!!!!!!!!!!!");
                        //如果是boss死亡 触发boss死亡事件
                        EventCenter.Instance.EventTrigger(E_EventType.E_Monster_BossDead);
                    }
                    hitAnimator.gameObject.GetComponent<MonsterMove>().isDead = true;
                    //播放死亡动画
                    hitAnimator.SetTrigger("dead");
                    //触发死亡事件
                    EventCenter.Instance.EventTrigger(E_EventType.E_Monster_Dead);
                    //生成掉落奖励
                    Instantiate(RewardTools.Instance.ItemCount(),
                        new Vector3(hitAnimator.gameObject.transform.position.x,
                            hitAnimator.gameObject.transform.position.y + 1.5f,
                            hitAnimator.gameObject.transform.position.z), Quaternion.identity);
                }
                //2秒后销毁游戏对象
                Destroy(hitAnimator.gameObject,2f);
            }
            //是玩家
            else
            {
                //玩家死亡 开启死亡面板 然后选择返回主菜单还是返回主城
                UIMagr.Instance.ShowPanel<DeadPanel>();
            }
        }
        //如果是玩家的血量
        if (hitAnimator.gameObject.GetComponent<PlayerMove>() != null)
        {
            hitAnimator.gameObject.GetComponent<PlayerMove>().playerNowHp = defhp;
            EventCenter.Instance.EventTrigger<int>(E_EventType.E_Player_Hit,
                hitAnimator.gameObject.GetComponent<PlayerMove>().playerNowHp);
        }
        return defhp;
    }

    /// <summary>
    /// 远程攻击发射东西的方法
    /// </summary>
    public void AtkFire(GameObject atkObj,GameObject defObj,LayerMask parentLayerMask)
    {
        GameObject obj;
        //代表是玩家
        if (parentLayerMask == 6)
        {
            if (heroInfo.heroID == 3)
            {
                //法师攻击 使用对象池
                obj = PoolMgr.Instance.GetObject("Bullet/3");
            }
            else
            {
                //射手攻击
                obj = PoolMgr.Instance.GetObject("Bullet/4");
            }
            obj.transform.position = new Vector3(atkObj.transform.position.x, atkObj.transform.position.y + 1.25f,
                atkObj.transform.position.z);
            obj.GetComponent<Bullet>().Fire(defObj,parentLayerMask);
        }
        //代表是怪物
        else
        {
            if (atkObj.GetComponent<MonsterType>().id == 2)
            {
                //法师攻击 使用对象池
                obj = PoolMgr.Instance.GetObject("Bullet/3");
            }
            else
            {
                //射手攻击
                obj = PoolMgr.Instance.GetObject("Bullet/4");
            }
            obj.transform.position = new Vector3(atkObj.transform.position.x, atkObj.transform.position.y + 1.25f,
                atkObj.transform.position.z);
            obj.GetComponent<Bullet>().Fire(defObj,parentLayerMask);
        }
    }
    
    /// <summary>
    /// 监听按下skill2技能
    /// </summary>
    /// <param name="obj"></param>
    public void Skill2()
    {
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill2, () =>
        {
            switch (heroInfo.heroID)
            {
                case 1:
                    print("使用了技能1");
                    break;
                case 2:
                    print("使用了技能1");
                    break;
                case 3:
                    print("使用了技能1");
                    break;
                case 4:
                    print("使用了技能1");
                    break;
            }
        });
    }
    
    /// <summary>
    /// 监听按下skill3技能
    /// </summary>
    /// <param name="obj"></param>
    public void Skill3()
    {
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill2, () =>
        {
            switch (heroInfo.heroID)
            {
                case 1:
                    print("使用了技能1");
                    break;
                case 2:
                    print("使用了技能1");
                    break;
                case 3:
                    print("使用了技能1");
                    break;
                case 4:
                    print("使用了技能1");
                    break;
            }
        });
    }
    
    /// <summary>
    /// 监听按下skill4技能
    /// </summary>
    /// <param name="obj"></param>
    public void Skill4()
    {
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill2, () =>
        {
            switch (heroInfo.heroID)
            {
                case 1:
                    print("使用了技能1");
                    break;
                case 2:
                    print("使用了技能1");
                    break;
                case 3:
                    print("使用了技能1");
                    break;
                case 4:
                    print("使用了技能1");
                    break;
            }
        });
    }
}

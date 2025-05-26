using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject effObj;
    private GameObject obj;
    private HeroInfo heroInfo;
    private Vector3 direction;
    private Vector3 horizontalDirection;
    private LayerMask layerMask;
    
    private void Awake()
    {
        heroInfo = GameDataMgr.Instance.heroData;
        if (heroInfo.heroID == 3)
        {
            effObj = Resources.Load<GameObject>("Eff/BulletEff1");
        }
        else
        {
            effObj = Resources.Load<GameObject>("Eff/BulletEff2");
        }
    }

    /// <summary>
    /// 攻击的目标
    /// </summary>
    /// <param name="obj">目标obj</param>
    public void Fire(GameObject obj,LayerMask parentLayerMask)
    {
        layerMask = parentLayerMask;
        // 计算目标方向
        direction = obj.transform.position - transform.position;
        // 将方向投影到水平面（XZ平面），忽略Y轴差异
        horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;
        // 应用旋转（仅影响Y轴）
        if (horizontalDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(horizontalDirection);
        }
    }

    private void Update()
    {
        // 沿当前朝向移动
        gameObject.transform.Translate(transform.forward * 30 * Time.deltaTime,Space.World);
    }

    /// <summary>
    /// 碰撞体碰撞之后 加入缓存池
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        //如果是玩家的子弹 并且撞到了敌人
        if (layerMask == 6)
        {
            if (other.gameObject.layer != 6 && other.gameObject.layer != 8)
            {
                if (other.gameObject.layer == 7)
                {
                    other.gameObject.GetComponent<MonsterMove>().monsterNowHp = AtkOrHit.Instance.Hit(10, 1, 
                        other.gameObject.GetComponent<MonsterMove>().monsterNowHp, other.gameObject.GetComponent<Animator>());
                }
                obj = Instantiate(effObj, gameObject.transform);
                obj.transform.parent = null;
                PoolMgr.Instance.PushObj(gameObject);
                gameObject.SetActive(false);
            }
        }
        //如果是敌人的子弹 并且撞到了玩家
        else if (layerMask == 7)
        {
            if (other.gameObject.layer != 7 && other.gameObject.layer != 8)
            {
                if (other.gameObject.layer == 6)
                {
                    AtkOrHit.Instance.Hit(1, 1, other.gameObject.GetComponent<PlayerMove>().playerNowHp, other.gameObject.GetComponent<Animator>());
                }
                obj = Instantiate(effObj, gameObject.transform);
                obj.transform.parent = null;
                PoolMgr.Instance.PushObj(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class MonsterMove : MonoBehaviour
{
    //判断是否需要等待玩家进入到视线内才攻击
    public bool isIdle = true;

    private Transform player;

    //怪物自动寻路相关
    private NavMeshAgent agent;
    private Animator animator;
    private int atkTriggerHash = Animator.StringToHash("atk");

    private int runBoolHash = Animator.StringToHash("isRun");

    //攻击范围
    private int attackRange;

    private int nowID;

    //怪物和寻路目标点的距离
    private float distance;

    //攻击间隔
    private float atkTimer;
    public int monsterNowHp = 30;

    private string atkAnimatrName;

    //攻击是否正在进行
    private bool isAttacking = false;

    //是否死亡
    public bool isDead;
    private Transform deadTransform;
    
    //Boss的攻击方式
    private int bossAtkType;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        nowID = GetComponent<MonsterType>().id;
        switch (nowID)
        {
            case 1:
                attackRange = 2;
                monsterNowHp = 30;
                break;
            case 2:
                attackRange = 8;
                monsterNowHp = 20;
                break;
            case 3:
                attackRange = 8;
                monsterNowHp = 20;
                break;
            case 4:
                attackRange = 3;
                monsterNowHp = 100;
                break;
        }
    }

    /// <summary>
    /// 判断玩家是否进入怪物的视线
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer == 6)
        {
            isIdle = false;
            player = collider.transform;
            if (isDead)
            {
                return;
            }

            //玩家是否进入到怪物攻击范围
            distance = Vector3.Distance(player.position, transform.position);
            //玩家在攻击范围之外
            if (distance > attackRange)
            {
                agent.isStopped = false;
                animator.SetBool(runBoolHash, true);
                agent.SetDestination(player.position);
                isAttacking = false;
            }
            //如果玩家进入到怪物的攻击范围内
            else
            {
                if (player != null && atkTimer <= 0 && !isAttacking)
                {
                    isAttacking = true;
                    atkTimer = 2f;
                    agent.isStopped = true;
                    animator.SetBool(runBoolHash, false);
                    //攻击动画开始
                    animator.SetTrigger(atkTriggerHash);
                    Atk();
                }
            }
        }
    }
    
    /// <summary>
    /// 怪物攻击玩家
    /// </summary>
    void Atk()
    {
        //提前设置攻击状态
        //开始攻击玩家
        switch (nowID)
        {
            case 1:
                //如果是近战兵
                AtkOrHit.Instance.Hit(1, 2, 1, player.gameObject.GetComponent<Animator>());
                break;
            case 2:
                //如果是法师
                AtkOrHit.Instance.AtkFire(this.gameObject, player.gameObject, gameObject.layer);
                break;
            case 3:
                //如果是射手
                AtkOrHit.Instance.AtkFire(this.gameObject, player.gameObject, gameObject.layer);
                break;
            case 4:
                //如果是Boss
                bossAtkType = UnityEngine.Random.Range(1, 4);//1-3
                switch (bossAtkType)
                {
                    case 1 :
                        animator.SetTrigger("atk1");
                        AtkOrHit.Instance.Hit(3, 2, 1, player.gameObject.GetComponent<Animator>());
                        break;
                    case 2 :
                        animator.SetTrigger("atk2");
                        AtkOrHit.Instance.Hit(3, 2, 1, player.gameObject.GetComponent<Animator>());
                        break;
                    case 3 :
                        animator.SetTrigger("atk3");
                        AtkOrHit.Instance.Hit(3, 2, 1, player.gameObject.GetComponent<Animator>());
                        break;
                }
                break;
        }
        isAttacking = false;
    }

    void Update()
    {
        // 递减攻击计时器
        atkTimer -= Time.deltaTime;
        isAttacking = false;
        if (isDead)
        {
            //死亡 停止移动和寻路
            agent.enabled = false;
            return;
        }
        //路径完成且无后续路径
        if (agent.pathStatus == NavMeshPathStatus.PathComplete && !agent.hasPath && !isIdle && !isDead)
        {
            //到达目标点
            agent.isStopped = true;
            animator.SetBool(runBoolHash, false);
            isIdle = true;
            player = null;
        }
    }
}

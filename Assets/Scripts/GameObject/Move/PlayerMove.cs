using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    private HeroInfo heroInfo;
    private Transform transform;
    private Animator animator;
    // 定义动画触发器的哈希值
    private int atkTriggerHash = Animator.StringToHash("atk");
    private int skill2TriggerHash = Animator.StringToHash("skill2");
    private int skill3TriggerHash = Animator.StringToHash("skill3");
    private int skill4TriggerHash = Animator.StringToHash("skill4");

    private KeyInfo keyInfo;
    private CharacterController controller;
    //是否在移动
    private bool isMoving;
    //需要移动的目标点
    private Vector3 targetPosition;
    //移动方向
    private Vector3 moveDirection;
    //转动到达目标角度
    private Quaternion targetRotation;
    private float moveSpeed = 10f;
    //攻击范围外的怪物目标
    private GameObject targetMonster;
    //攻击距离
    private int attackRange;
    //玩家当前血量
    public int playerNowHp;
    
    //获取玩家数据
    private PlayerData playerData;
    //攻击间隔
    private float atkTimer;
    
    
    private void Awake()
    {
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        keyInfo = GameDataMgr.Instance.keyInfo;
        heroInfo = GameDataMgr.Instance.heroData;
        //初始化玩家血量
        playerNowHp = heroInfo.STR * 2;
    }

    private void Start()
    {
        //添加需要监听的事件 用来改变动画
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill1, () =>
        {
            animator.SetTrigger(atkTriggerHash);
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill2, () =>
        {
            animator.SetTrigger(skill2TriggerHash);
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill3, () =>
        {
            animator.SetTrigger(skill3TriggerHash);
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill4, () =>
        {
            animator.SetTrigger(skill4TriggerHash);
        });
        InputMgr.Instance.StartOrEndInputMgr(true);
    }

    private void Update()
    {   
        atkTimer -= Time.deltaTime;
        //向游戏管理器传递玩家对象
        GameMgr.Instance.playerObj = gameObject;
        //进行射线检测,并且不是在点击UI界面
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //优先检测怪物
            if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Monster"),QueryTriggerInteraction.Ignore))
            {
                if (atkTimer > 0)
                {
                    return;
                }
                    targetMonster = hit.transform.gameObject;
                    // 判断是不是近战
                    if (heroInfo.heroID == 1 || heroInfo.heroID == 2)
                    {
                        attackRange = 4;
                    }
                    // 判断是不是远程
                    else if (heroInfo.heroID == 3 || heroInfo.heroID == 4)
                    {
                        attackRange = 12;
                    }
                    //进行攻击
                    if (Vector3.Distance(transform.position,targetMonster.transform.position) <= attackRange )
                    {
                        EventCenter.Instance.EventTrigger(E_EventType.E_Input_Skill1);
                        if (attackRange == 12)
                        {
                            //发射子弹
                            AtkOrHit.Instance.AtkFire(this.gameObject,targetMonster,gameObject.layer);
                        }
                        else
                        {
                            MusicMgr.Instance.PlaySound("skill/melee");
                            //近战攻击直接造成伤害
                            targetMonster.gameObject.GetComponent<MonsterMove>().monsterNowHp = AtkOrHit.Instance.Hit(10, 1,
                                targetMonster.gameObject.GetComponent<MonsterMove>().monsterNowHp,
                                targetMonster.gameObject.GetComponent<Animator>());
                        }
                        targetMonster = null;
                        isMoving = false;
                    }
                    else
                    {
                            MoveToTarget(transform.position + (hit.transform.position - transform.position).normalized * attackRange);
                            isMoving = true;
                    }
                    //重置攻击间隔
                    atkTimer = heroInfo.atkTimer;
            }
            //若未点击中怪物，检测地面移动
            else if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Plane"),QueryTriggerInteraction.Ignore))
            {
                targetMonster = null;
                MoveToTarget(hit.point);
                isMoving = true;
            }
        }

        //按下ese开启设置界面
        if (Input.GetKeyDown(keyInfo.keyEse))
        {
            UIMagr.Instance.ShowPanel<SetPanel>();
            //并且暂停时间
            Time.timeScale = 0;
        }
        
        //是否打开背包
        if (Input.GetKeyDown(keyInfo.keyI))
        {
            UIMagr.Instance.ShowPanel<RolePanel>();
        }

        //是否返回主菜单
        if (Input.GetKeyDown(keyInfo.keyM))
        {
            UIMagr.Instance.ShowPanel<ReturnMainPanel>();
        }
        //是否按下技能按键
        if (Input.GetKeyDown(keyInfo.keySkill2))
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_Input_Skill2);
        }
        
        if (Input.GetKeyDown(keyInfo.keySkill3))
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_Input_Skill3);
        }

        if (Input.GetKeyDown(keyInfo.keySkill4))
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_Input_Skill4);
        }
        
        //血瓶
        if (Input.GetKeyDown(keyInfo.keyHp))
        {

        }
        //蓝瓶
        if (Input.GetKeyDown(keyInfo.keyMp))
        {

        }
    }
    
    /// <summary>
    /// 移动到的目标点
    /// </summary>
    /// <param name="target"></param>
    void MoveToTarget(Vector3 target)
    {
        targetPosition = target;
        isMoving = true;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            //计算水平方向（忽略 Y 轴高度差）
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;

            //平滑转向
            targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * 5 * Time.deltaTime);

            //添加移动目标并且改变动画移动
            animator.SetFloat("v",1f);
            moveDirection = transform.forward * moveSpeed * Time.deltaTime;
            //是否接地 不然就添加重力
            moveDirection.y = controller.isGrounded ? 0 : Physics.gravity.y * Time.deltaTime;
            //移动
            controller.Move(moveDirection);


            //到达目标点停止
            if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
            {
                animator.SetFloat("v",0f);
                isMoving = false;
                
                // 如果有目标怪物，检查是否在攻击范围内
                if (targetMonster != null)
                {
                    if (Vector3.Distance(transform.position, targetMonster.transform.position) <= attackRange)
                    {
                        EventCenter.Instance.EventTrigger(E_EventType.E_Input_Skill1);
                        if (heroInfo.heroID == 3 || heroInfo.heroID == 4)
                        {
                            // 发射子弹
                            AtkOrHit.Instance.AtkFire(this.gameObject, targetMonster, gameObject.layer);
                        }
                        targetMonster = null;
                    }
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型枚举
/// </summary>
public enum E_EventType
{
    /// <summary>
    /// 怪物死亡事件 参数:Monster
    /// </summary>
    E_Monster_Dead,
    
    /// <summary>
    /// Boss死亡事件 参数:Monster
    /// </summary>
    E_Monster_BossDead,
    
    /// <summary>
    /// 玩家死亡 参数:Player
    /// </summary>
    E_Player_Dead,
    
    /// <summary>
    /// 玩家受伤 参数:int
    /// </summary>
    E_Player_Hit,
    
    /// <summary>
    /// 玩家获得奖励 参数:int
    /// </summary>
    E_Player_GetReward,
    
    /// <summary>
    /// 场景加载时进度变化
    /// </summary>
    E_SceneLoadChange,

    #region 输入行为

    /// <summary>
    /// 输入系统触发技能1行为
    /// </summary>
    E_Input_Skill1,
    
    /// <summary>
    /// 输入系统触发技能2行为
    /// </summary>
    E_Input_Skill2,
    
    /// <summary>
    /// 输入系统触发技能3行为
    /// </summary>
    E_Input_Skill3,
    
    /// <summary>
    /// 输入系统触发技能4行为
    /// </summary>
    E_Input_Skill4,
    
    /// <summary>
    /// 水平热键-1到1
    /// </summary>
    E_Input_Horizontal,
    
    /// <summary>
    /// 数值热键-1到1
    /// </summary>
    E_Input_Vertical,

    #endregion

    #region 触发器

    /// <summary>
    /// 触发器e1 进入地牢场景
    /// </summary>
    E_Tigger_E1,
    
    /// <summary>
    /// 触发器e2 查看统计数据
    /// </summary>
    E_Tigger_E2,
    
    /// <summary>
    /// 触发器e3 打开商店界面
    /// </summary>
    E_Tigger_E3,
    
    /// <summary>
    /// 开启boss的房间门
    /// </summary>
    E_Tigger_Door,
    
    /// <summary>
    /// 打败boss的房间门
    /// </summary>
    E_Tigger_BossDoor,

    #endregion

    #region 背包

    /// <summary>
    /// 开始拖动
    /// </summary>
    E_Bag_BeginDrag,
    
    /// <summary>
    /// 拖动中
    /// </summary>
    E_Bag_Drag,
    
    /// <summary>
    /// 结束拖动
    /// </summary>
    E_Bag_EndDrag,
    
    /// <summary>
    /// 鼠标进入
    /// </summary>
    E_Bag_PointerEnter,
    
    /// <summary>
    /// 鼠标移动
    /// </summary>
    E_Bag_PointerExit,

    #endregion
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 英雄的三相之力转换为面板属性的数据类
/// 1力量等于2血量2近战攻击力1远程攻击力
/// 1敏捷等于2防御力1近战攻击力2远程攻击力
/// 1智力等于2蓝量3魔法攻击力
/// </summary>
public class HeroAttInfo
{
    /// <summary>
    /// 血量
    /// </summary>
    public int hp = 0;
    /// <summary>
    /// 蓝量
    /// </summary>
    public int mp = 0;
    /// <summary>
    /// 防御力
    /// </summary>
    public int def = 0;
    /// <summary>
    /// 近战攻击力
    /// </summary>
    public int meleeAtk = 0;
    /// <summary>
    /// 远程攻击力
    /// </summary>
    public int remoteAtk = 0;
    /// <summary>
    /// 魔法攻击力
    /// </summary>
    public int magicAtk = 0;
}

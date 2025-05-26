using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色的初始数据
/// 1力量等于2血量2近战攻击力1远程攻击力
/// 1敏捷等于2防御力1近战攻击力2远程攻击力
/// 1智力等于2蓝量3魔法攻击力
/// </summary>
public class HeroInfo
{
    public int heroID;
    public string heroName;
    /// <summary>
    /// 力量
    /// </summary>
    public int STR;
    /// <summary>
    /// 敏捷
    /// </summary>
    public int DEX;
    /// <summary>
    /// 智力
    /// </summary>
    public int INT;

    //最大攻击间隔
    public int atkTimer;
    public int skill2Timer;
    public int skill3Timer;
    public int skill4Timer;
}

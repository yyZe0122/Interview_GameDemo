using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateAttTools : BaseManager<CalculateAttTools>
{
    private HeroAttInfo heroAttInfo = new HeroAttInfo();
    
    private CalculateAttTools()
    {

    }
    
    /// <summary>
    /// 英雄的三相之力转换为面板属性
    /// 1力量等于2血量2近战攻击力1远程攻击力
    /// 1敏捷等于2防御力1近战攻击力2远程攻击力
    /// 1智力等于2蓝量3魔法攻击力
    /// </summary>
    /// <param name="STR">力量</param>
    /// <param name="DEX">敏捷</param>
    /// <param name="INT">智力</param>
    /// <returns>返回HeroAttInfo</returns>
    public HeroAttInfo CalculateAtt(int STR, int DEX, int INT)
    {
        //计算转换
        heroAttInfo.hp = STR * 2;
        heroAttInfo.mp = INT * 2;
        heroAttInfo.def = DEX * 2;
        heroAttInfo.meleeAtk = STR * 2 + DEX;
        heroAttInfo.remoteAtk = STR + DEX * 2;
        heroAttInfo.magicAtk = INT * 3;
        return heroAttInfo;
    }
}

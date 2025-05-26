using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用于处理字符串的公共功能
/// </summary>
public class TextUtil
{
    #region 字符串拆分

        /// <summary>
    /// 拆分字符串 返回字符串数组
    /// </summary>
    /// <param name="str">被拆分的字符串</param>
    /// <param name="type">被拆分的类型 1是;2是,3是%4是:5是空格6是|7是_</param>
    /// <returns></returns>
    public static string[] SplitString(string str, int type = 1)
    {
        if (str == "")
        {
            return new string[0];
        }

        string newStr = str;
        switch (type)
        {
            case 1:
                //为了防止英文符号写成中文
                while (newStr.IndexOf("；") != -1)
                {
                    //先替换
                    newStr = newStr.Replace("；", ";");
                }
                return newStr.Split(';');
            case 2:
                //为了防止英文符号写成中文
                while (newStr.IndexOf("，") != -1)
                {
                    //先替换
                    newStr = newStr.Replace("，", ",");
                }
                return newStr.Split(',');
            case 3:
                return newStr.Split('%');
            case 4:
                //为了防止英文符号写成中文
                while (newStr.IndexOf("：") != -1)
                {
                    //先替换
                    newStr = newStr.Replace("：", ":");
                }
                return newStr.Split(':');
            case 5:
                return newStr.Split(' ');
            case 6:
                return newStr.Split('|');
            case 7:
                return newStr.Split('_');
        }
        return new string[0];
    }

    /// <summary>
    /// 拆分字符串 返回整形数组
    /// </summary>
    /// <param name="str">被拆分的字符串</param>
    /// <param name="type">被拆分的类型 1是;2是,3是%4是:5是空格6是|7是_</param>
    /// <returns></returns>
    public static int[] SplitStringIntArr(string str, int type = 1)
    {
        //得到拆分后的字符串数组
        string[] strs = SplitString(str, type);
        if (strs.Length == 0)
        {
            return new int[0];
        }
        //把字符串数组转换为int数组
        return Array.ConvertAll<string, int>(strs, (strings) =>
        {
            return int.Parse(strings);
        });
    }

    /// <summary>
    /// 专门用来处理拆分道具组信息的方法 转换为int
    /// </summary>
    /// <param name="str">被拆分的字符串</param>
    /// <param name="typeOne">道具间的分隔符 1是;2是,3是%4是:5是空格6是|7是_</param>
    /// <param name="typeTwo">道具Id和道具数量的分隔符 1是;2是,3是%4是:5是空格6是|7是_</param>
    /// <param name="callBack">委托回调函数</param>
    public static void SplitStringToIntArrTwice(string str, int typeOne, int typeTwo, UnityAction<int, int> callBack)
    {
        string[] strs = SplitString(str, typeOne);
        if (strs.Length == 0)
        {
            return;
        }
        int[] ints;
        for (int i = 0; i < strs.Length; i++)
        {
            //拆分单个道具的id和数量
            ints = SplitStringIntArr(strs[i], typeTwo);
            if (ints.Length == 0)
            {
                continue;
            }
            callBack.Invoke(ints[0],ints[1]);
        }
    }
    
    /// <summary>
    /// 专门用来处理拆分道具组信息的方法 转换为string
    /// </summary>
    /// <param name="str">被拆分的字符串</param>
    /// <param name="typeOne">道具间的分隔符 1是;2是,3是%4是:5是空格6是|7是_</param>
    /// <param name="typeTwo">道具Id和道具数量的分隔符 1是;2是,3是%4是:5是空格6是|7是_</param>
    /// <param name="callBack">委托回调函数</param>
    public static void SplitStringTwice(string str, int typeOne, int typeTwo, UnityAction<string, string> callBack)
    {
        string[] strs = SplitString(str, typeOne);
        if (strs.Length == 0)
        {
            return;
        }
        string[] strss;
        for (int i = 0; i < strs.Length; i++)
        {
            //拆分单个道具的id和数量
            strss = SplitString(strs[i], typeTwo);
            if (strss.Length == 0)
            {
                continue;
            }
            callBack.Invoke(strss[0],strss[1]);
        }
    }

    #endregion

    #region 数字转字符串相关

    /// <summary>
    /// 转字符串数字前补0
    /// </summary>
    /// <param name="value">需要补的数字</param>
    /// <param name="lenght">补到多少位</param>
    /// <returns></returns>
    public static string GetNumStr(int value, int lenght)
    {
        //ToString中传入一个 D数字 的字符串
        //代表想要将数字转换为长度为 n 的字符串
        //如果长度不够 会在前面补0
        return value.ToString($"D{lenght}");
    }

    /// <summary>
    /// 转字符串保留n位小数
    /// 指定浮点数 保留小数点后n位
    /// </summary>
    /// <param name="value">浮点数</param>
    /// <param name="lenght">保留多少位</param>
    /// <returns></returns>
    public static string GetDecimalStr(float value, int lenght)
    {
        //ToString中传入一个 F数字 的字符串
        //想要保留小数点后n位
        return value.ToString($"F{lenght}");
    }
    #endregion

    #region 秒转时分秒

    private static StringBuilder resultStr = new StringBuilder("");
    
    /// <summary>
    /// 秒转时分秒
    /// </summary>
    /// <param name="s">秒数</param>
    /// <param name="egZero">是否忽略0</param>
    /// <param name="isKeepLenght">是否至少保留两位</param>
    /// <param name="hourStr">时的拼接字符</param>
    /// <param name="minuteStr">分的拼接字符</param>
    /// <param name="secondStr">秒的拼接字符</param>
    /// <returns></returns>
    public static string SecondToHMS(int s, bool egZero = false, bool isKeepLenght = false,
        string hourStr = "时", string minuteStr = "分", string secondStr = "秒")
    {
        //防止负数显示
        if (s < 0)
        {
            s = 0;
        }
        //计算小时
        int hour = s / 3600;
        //计算分钟
        //除去小时后的剩余秒
        int second = s % 3600;
        //剩余秒转为分钟数
        int minute = second / 60;
        //计算秒
        second = s % 60;
        //拼接
        resultStr.Clear();
        //如果小时不为0 或 不忽略0
        if (hour != 0 || !egZero)
        {
            resultStr.Append(isKeepLenght?GetNumStr(hour,2) : hour);//具体几个小时
            resultStr.Append(hourStr);//时
        }
        //如果分钟不为0 或 不忽略0 或 小时不为0
        if (minute != 0 || !egZero || hour != 0)
        {
            resultStr.Append(isKeepLenght?GetNumStr(minute,2) :  minute);//具体几个小时
            resultStr.Append(minuteStr);//分
        }
        //如果秒不为0 或 不忽略0 或 小时或者分钟不为0
        if (second != 0 || !egZero || hour !=0 || minute != 0)
        {
            resultStr.Append(isKeepLenght?GetNumStr(second,2) : second);//具体几个小时
            resultStr.Append(secondStr);//秒
        }
        //如果传入的参数为0秒时
        if (resultStr.Length == 0)
        {
            resultStr.Append(0);//具体几个小时
            resultStr.Append(secondStr);//秒
        }
        return resultStr.ToString();
    }

    /// <summary>
    /// 秒转00:00:00
    /// </summary>
    /// <param name="s">秒数</param>
    /// <param name="egZero">是否忽略0</param>
    /// <returns></returns>
    public static string SecondToHMS2(int s, bool egZero = false)
    {
        return SecondToHMS(s, egZero, true, ":", ":", null);
    }
    #endregion

    #region 秒转00:00:00
    
    //修改秒转时分秒
    //简化创造 SecondToHMS2
    
    #endregion

    #region 大数据数值转字符串

    /// <summary>
    /// 大数据数值转字符串
    /// </summary>
    /// <param name="num">数值</param>
    /// <returns>n亿n千万 或 n万n千 或 9999 100</returns>
    public static string GetBigDataToString(int num)
    {
        //如果大于1亿 那么就显示 n亿n千万
        if (num >= 100000000)
        {
            return BigDataChange(num, 100000000, "亿", "千万");
        }
        //如果大于1万 那么就显示 n万n千
        else if (num >= 10000)
        {
            return BigDataChange(num, 10000, "万", "千");
        }
        //都不满足就显示数值本身
        else
        {
            return num.ToString();
        }
    }
    
    /// <summary>
    /// 大数据数值转字符串的拼接方法
    /// </summary>
    /// <param name="num">数值</param>
    /// <param name="company">分隔单位 可以填100000000,10000</param>
    /// <param name="bigCompany">大单位 亿 万</param>
    /// <param name="littltCompany">小单位 万 千</param>
    /// <returns></returns>
    private static string BigDataChange(int num, int company, string bigCompany, string littltCompany)
    {
        resultStr.Clear();
        //有几亿 几万
        resultStr.Append(num / company);
        resultStr.Append(bigCompany);
        //有几千万 几千
        int tmpNum = num % company;
        tmpNum /= (company / 10);
        if (tmpNum != 0)
        {
            resultStr.Append(tmpNum);
            resultStr.Append(littltCompany);
        }
        return resultStr.ToString();
    }

    #endregion
}

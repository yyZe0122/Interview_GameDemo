using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加密工具类 主要提供加密需求
/// </summary>
public class EncrypotionUtil
{
    //1.获取随机密钥
    public static int GetRandomKey()
    {
       return Random.Range(0, 10000);
    }
    //2.加密数据
    public static int LockValue(int value, int key)
    {
        //主要采用异或 取余 加密 
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        value += key;
        return value;
    }
    
    //3.解密数据
    public static int UnLockValue(int value, int key)
    {
        //如果为0 那么没加密过
        if (value == 0)
        {
            Debug.LogError("数据未被加密过");
            return value;
        }
        value -= key;
        value = value ^ (1 << 5);
        value = value ^ 0xADAD;
        value = value ^ (key % 9);
        return value;
    }
}

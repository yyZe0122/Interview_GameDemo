using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入信息
/// </summary>
public class InputInfo
{
    public enum E_KeyOrMouse
    {
        //键盘输入
        Key,
        //鼠标输入
        Mouse,
    }
    
    public enum E_InputType
    {
        //按下
        Down,
        //抬起
        Up,
        //长按
        Always,
    }
    
    //输入设备的类型-键盘 鼠标
    public E_KeyOrMouse keyOrMouse;
    //输入的类型-按下 抬起 长按
    public E_InputType inputType;
    //KeyCode
    public KeyCode key;
    //mouseID
    public int mouseID;

    /// <summary>
    /// 键盘相关的输入初始化
    /// </summary>
    /// <param name="inputType"></param>
    /// <param name="key"></param>
    public InputInfo(E_InputType inputType, KeyCode key)
    {
        this.keyOrMouse = E_KeyOrMouse.Key;
        this.inputType = inputType;
        this.key = key;
    }
    
    /// <summary>
    /// 鼠标相关的输入初始化
    /// </summary>
    /// <param name="inputType"></param>
    /// <param name="mouseID"></param>
    public InputInfo(E_InputType inputType,int mouseID)
    {
        this.keyOrMouse = E_KeyOrMouse.Mouse;
        this.inputType = inputType;
        this.mouseID = mouseID;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputMgr : BaseManager<InputMgr>
{
    //是否开启输入系统检测
    private bool isStart;
    
    private Dictionary<E_EventType, InputInfo> inputDic = new Dictionary<E_EventType, InputInfo>();

    //在Update中遍历的InputInfo信息
    private InputInfo nowInputInfo;

    /// <summary>
    /// 开启或者关闭输入管理模块的检测
    /// 起到两个作用 一个开启或者关闭输入检测
    /// 一个是调用后让InputMgr被使用 可以接受事件
    /// </summary>
    /// <param name="isStart"></param>
    public void StartOrEndInputMgr(bool isStart)
    {
        this.isStart = isStart;
    }

    //用于在改键时获取输入信息的委托 只有当update中获得到信息的时候 再通过委托传递给外部
    private UnityAction<InputInfo> getInputActionCallBack = null;

    //是否开启输入检测
    private bool isBeginCheackInput = false;
    
    /// <summary>
    /// 获取下一次输入的信息
    /// </summary>
    /// <param name="callBack"></param>
    public void GetInputInfo(UnityAction<InputInfo> callBack)
    {
        getInputActionCallBack = callBack;
        //调用协程
        MonoMgr.Instance.StartCoroutine(RellyGetInputInfo());
    }

    private IEnumerator RellyGetInputInfo()
    {
        //等待一帧
        yield return 0;
        //一帧后检测
        isBeginCheackInput = true;
    }
    
    /// <summary>
    /// 自定义Update函数
    /// </summary>
    private void InputUpdata()
    {
        //当委托不为空的时候 证明想要获取按键信息
        if (isBeginCheackInput)
        {
            //当一个键按下时 遍历所有按键来得到按下的按键信息
            if (Input.anyKeyDown)
            {
                InputInfo inputInfo = null;
                //需要遍历所有键位 来得到输入的信息
                //键盘
                Array keyCodes = Enum.GetValues(typeof(KeyCode));
                foreach (KeyCode inputKey in keyCodes)
                {
                    //判断到底是哪个被按下了 那么就可以得到对应的键盘输入信息
                    if (Input.GetKeyDown(inputKey))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, inputKey);
                        break;
                    }
                }
                //鼠标
                for (int i = 0; i < 3; i++)
                {
                    if (Input.GetMouseButtonDown(i))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, i);
                        break;
                    }
                }
                //把获取到的信息传递给外部
                getInputActionCallBack.Invoke(inputInfo);
                getInputActionCallBack = null;
                //检测一次后
                isBeginCheackInput = false;
            }
        }
        
        //没有开启就不检测按键
        if (!isStart)
        {
            return;
        }
        
        //判断是键盘还是鼠标输入 输入的按键是什么 输入的类型是什么
        foreach (E_EventType eventType in inputDic.Keys)
        {
            nowInputInfo = inputDic[eventType];
            //键盘输入
            if (nowInputInfo.keyOrMouse == InputInfo.E_KeyOrMouse.Key)
            {
                //键盘是抬起 按下 还是长按
                switch (nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if (Input.GetKeyDown(nowInputInfo.key))
                        {
                            EventCenter.Instance.EventTrigger(eventType);
                        }
                        break;
                    case InputInfo.E_InputType.Up:
                        if (Input.GetKeyUp(nowInputInfo.key))
                        {
                            EventCenter.Instance.EventTrigger(eventType);
                        }
                        break;
                    case InputInfo.E_InputType.Always:
                        if (Input.GetKey(nowInputInfo.key))
                        {
                            EventCenter.Instance.EventTrigger(eventType);
                        }
                        break;
                }
            }
            //鼠标输入
            else
            {
                //鼠标是抬起 按下 还是长按
                switch (nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if (Input.GetMouseButtonDown(nowInputInfo.mouseID))
                        {
                            EventCenter.Instance.EventTrigger(eventType);
                        }
                        break;
                    case InputInfo.E_InputType.Up:
                        if (Input.GetMouseButtonUp(nowInputInfo.mouseID))
                        {
                            EventCenter.Instance.EventTrigger(eventType);
                        }
                        break;
                    case InputInfo.E_InputType.Always:
                        if (Input.GetMouseButton(nowInputInfo.mouseID))
                        {
                            EventCenter.Instance.EventTrigger(eventType);
                        }
                        break;
                }
            }
        }

        //热键
        EventCenter.Instance.EventTrigger(E_EventType.E_Input_Horizontal, Input.GetAxis("Horizontal"));
        EventCenter.Instance.EventTrigger(E_EventType.E_Input_Vertical, Input.GetAxisRaw("Vertical"));

    }
    
    /// <summary>
    /// 键盘改键 或者 初始化的方法
    /// </summary>
    /// <param name="eventType">调用的事件</param>
    /// <param name="key">哪一个按键</param>
    /// <param name="inputType">输入的类型</param>
    public void ChangeKeyboardInfo(E_EventType eventType, KeyCode key, InputInfo.E_InputType inputType)
    {
        //初始化
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType,new InputInfo(inputType,key));
        }
        //改键
        else
        {
            //如果之前是鼠标 必须要修改按键类型
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Key;
            
            inputDic[eventType].key = key;
            inputDic[eventType].inputType = inputType;
        }
    }
    
    /// <summary>
    /// 鼠标改键 或者初始化的方法
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="mouseID"></param>
    /// <param name="inputType"></param>
    public void ChangeMouseInfo(E_EventType eventType, int mouseID, InputInfo.E_InputType inputType)
    {
        //初始化
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType,new InputInfo(inputType,mouseID));
        }
        //改键
        else
        {
            //如果之前是鼠标 必须要修改按键类型
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Mouse;
            
            inputDic[eventType].mouseID = mouseID;
            inputDic[eventType].inputType = inputType;
        }
    }

    /// <summary>
    /// 移除指定行为的输入监听
    /// </summary>
    /// <param name="eventType"></param>
    public void RemoveInputInfo(E_EventType eventType)
    {
        if (inputDic.ContainsKey(eventType))
        {
            inputDic.Remove(eventType);
        }
    }
    
    /// <summary>
    /// 将InputMgr加入到MonoMgr的Update中
    /// 使其没有继承Mono也可以Update
    /// </summary>
    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdata);
    }
}

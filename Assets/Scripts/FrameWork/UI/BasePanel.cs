using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// GUI面板基类
/// 需要继承Mono 因为要挂载到面板对象上
/// 抽象函数 子类必须实现其中的方法
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 用于存储所有要用到的UI控件 用于里氏替换原则
    /// </summary>
    protected Dictionary<string, UIBehaviour> controlDic = new Dictionary<string, UIBehaviour>();
    
    /// <summary>
    /// 存储默认名字的控件
    /// 默认名字的控件不需要代码修改
    /// </summary>
    private static List<string> defaultName = new List<string>()
    {
        "Image",
        "Text (TMP)",
        "RawImage",
        "Arrow",
        "Background",
        "Checkmark",
        "Label",
        "Fill",
        "Handle",
        "Placeholder",
        "Text",
    };

    protected virtual void Awake()
    {
        //为了避免某一个对象上面存在两种控件的情况
        //我们应该查找更重要的组件
        FindeChildrenControl<Button>();
        FindeChildrenControl<Toggle>();
        FindeChildrenControl<Slider>();
        FindeChildrenControl<InputField>();
        FindeChildrenControl<ScrollRect>();
        FindeChildrenControl<Dropdown>();
        //即使对象上挂在了多个组件 只要优先找到了重要组件
        //之后也可以通过重要组件得到身上其它挂载的内容
        FindeChildrenControl<Image>();
        FindeChildrenControl<TextMeshProUGUI>();
        FindeChildrenControl<Text>();
    }

    /// <summary>
    /// 面板显示
    /// 抽象函数 子类必须实现
    /// </summary>
    public abstract void ShowMe();

    /// <summary>
    /// 面板隐藏
    /// 抽象函数 子类必须实现
    /// </summary>
    public abstract void HideMe();

    /// <summary>
    /// 获取指定名字以及指定类型的组件
    /// </summary>
    /// <param name="name">组件名字</param>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns></returns>
    public T GetControl<T>(string name) where T: UIBehaviour
    {
        //判断是否存在传入的名字的组件
        if (controlDic.ContainsKey(name))
        {
            T control = controlDic[name] as T;
            //判断是否和传入的不同类型的组件
            if (control == null)
            {
                Debug.LogError($"不存在对应名字为{name}类型为{typeof(T)}的组件");
                return null;
            }
                return control;
        }
        else
        {
            Debug.LogError($"不存在对应名字为{name}的组件");
            return null;
        }
    }
    
    /// <summary>
    /// 查找并且监听UI组件的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindeChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>(true);
        for (int i = 0; i < controls.Length; i++)
        {
            //获得循环中当前控件的名字
            string controlName = controls[i].gameObject.name;
            //判断是否有相同的key
            if (!controlDic.ContainsKey(controlName))
            {
                //判断是不是默认名字 是默认名字不加
                if (!defaultName.Contains(controlName))
                {
                    //将对应组件记录到字典中
                    controlDic.Add(controlName,controls[i]);
                    
                    //判断控件的类型 决定是否加事件监听
                    //判断是不是按钮
                    if (controls[i] is Button)
                    {
                        (controls[i] as Button).onClick.AddListener(() =>
                        {
                            ClickButton(controlName);
                        });
                    }
                    //判断是不是滑动条
                    if (controls[i] is Slider)
                    {
                        (controls[i] as Slider).onValueChanged.AddListener((value) =>
                        {
                            ClickSlider(controlName,value);
                        });
                    }
                    //判断是不是单选框
                    if (controls[i] is Toggle)
                    {
                        (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                        {
                            ClickTiggle(controlName,value);
                        });
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 父类的Button虚函数
    /// 子类可以重写来处理事件
    /// </summary>
    /// <param name="buttonName"></param>
    protected virtual void ClickButton(string buttonName)
    {
        
    }
    
    /// <summary>
    /// 父类的Slider虚函数
    /// 子类可以重写来处理事件
    /// </summary>
    /// <param name="SliderName"></param>
    protected virtual void ClickSlider(string sliderName,float value)
    {
        
    }
    
    /// <summary>
    /// 父类的Tiggle虚函数
    /// 子类可以重写来处理事件
    /// </summary>
    /// <param name="SliderName"></param>
    protected virtual void ClickTiggle(string toggleName,bool isOpen)
    {
        
    }
    
}

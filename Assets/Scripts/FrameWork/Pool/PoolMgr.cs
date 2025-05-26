using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 抽屉(缓存池内的数据)游戏对象
/// </summary>
public class PoolData
{
    //用来存储抽屉中的对象 记录的时没有使用的对象
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    
    //记录使用中的对象
    private List<GameObject> usedList = new List<GameObject>();

    //抽屉上限 场景上同时存在的数量
    private int maxNum;
    
    //抽屉根对象 用来进行布局管理的对象
    private GameObject rootObj;

    //给外部提供方法
    //获取容器中是否有对象
    public int Count => dataStack.Count;

    public int UsedCount => usedList.Count;

    /// <summary>
    /// 使用中对象的数量 和 缓存池最大的容量比较 小于返回true
    /// </summary>
    public bool NeedCreate => usedList.Count < maxNum;
    
    /// <summary>
    /// 初始化构造函数
    /// </summary>
    /// <param name="root">传入父对象</param>
    public PoolData(GameObject root,string name,GameObject usedObj)
    {
        //开启功能时 才会动态创建父子关系
        if (PoolMgr.isOpenLayout)
        {
            //创建抽屉的父对象
            rootObj = new GameObject(name + "Parent");
            //和柜子的父对象建立父子关系
            rootObj.transform.SetParent(root.transform);
        }
        //在创建抽屉时外部动态创建对象 我们将其记录到使用中的对象容器中
        PushUsedList(usedObj);

        //数量上限获取
        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if (poolObj == null)
        {
            Debug.LogError("请为缓存池对象设置上限");
            return;
        }
        //记录缓存池上限
        maxNum = poolObj.maxNum;
    }

    /// <summary>
    /// 从抽屉中弹出数据对象
    /// </summary>
    /// <returns></returns>
    public GameObject Pop()
    {
        //取出对象
        GameObject obj;
        //判断抽屉中是有对象 还是 是缓存池超过上限
        if (Count > 0)
        {
            //从没有的容器中取出使用
            obj = dataStack.Pop();
            //从使用着的容器中添加
            usedList.Add(obj);
        }
        else
        {
            //取出第0个索引 代表就是使用最长的
            obj = usedList[0];
            //并且把他从使用中的对象中移除
            usedList.RemoveAt(0);
            //由于还需要去使用 那么再记录回来
            //并且添加到尾部
            usedList.Add(obj);
        }
        
        //激活对象 再返回
        obj.SetActive(true);
        if (PoolMgr.isOpenLayout)
        {
            //取出去后断开父子关系
            obj.transform.SetParent(null);
        }

        return obj;
    }

    /// <summary>
    /// 从抽屉中添加数据对象
    /// </summary>
    /// <param name="obj"></param>
    public void Push(GameObject obj)
    {
        //不删除而隐藏对象
        //失活对象
        obj.SetActive(false);
        //将失活的对象(要放入抽屉中的对象) 父对象设置为池子的根对象
        if (PoolMgr.isOpenLayout)
        {
            obj.transform.SetParent(rootObj.transform);
        }
        //通过栈记录对应的对象数据
        dataStack.Push(obj);
        //从使用着的容器中移除(对象已经失活) 对象进入栈中
        usedList.Remove(obj);
    }
    
    /// <summary>
    /// 将对象压入使用中的容器记录
    /// </summary>
    /// <param name="obj"></param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

/// <summary>
/// 方便字典中里氏替换原则
/// </summary>
public class PoolClassBase {}

/// <summary>
/// 存储 数据结构类 和 逻辑类 不继承Mono的类
/// </summary>
/// <typeparam name="T"></typeparam>
public class PoolClass<T>: PoolClassBase  where T : class
{
    public Queue<T> poolObj = new Queue<T>();
}

/// <summary>
/// 想要被复用的数据结构类 或逻辑类要继承该接口
/// </summary>
public interface IPoolClass
{
    /// <summary>
    /// 重置数据的方法
    /// </summary>
    void ResetInfo();
}

/// <summary>
/// 缓存池(对象池)管理器
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    //柜子当中有抽屉的体现
    //值代表 抽屉对象
    /// <summary>
    /// 用于存储游戏对象的池子字典容器
    /// </summary>
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    
    /// <summary>
    /// 用于存储数据结构类 逻辑类对象的池子字典容器
    /// </summary>
    private Dictionary<string, PoolClassBase> poolClassDic = new Dictionary<string, PoolClassBase>();

    //池子根对象(父对象)
    private GameObject poolObj;
    
    //是否开启窗口布局功能
    public static bool isOpenLayout = true;
    
    /// <summary>
    /// 取出对象的方法
    /// </summary>
    /// <param name="name">抽屉容器的名字</param>
    /// <returns>从缓存池中取出的对象</returns>
    public GameObject GetObject(string name)
    {
        //如果没有根对象 那么就创建
        if (poolObj == null && isOpenLayout)
        {
            poolObj = new GameObject("Pool");
        }
        
        GameObject obj;
        //没有抽屉 或者 抽屉中没有能使用的对象 并且 使用没有超过缓存池上限
        if (!poolDic.ContainsKey(name) ||
            poolDic[name].Count == 0 && poolDic[name].NeedCreate)
        {
            //动态创建对象
            //没有的时候 通过资源加载去 实例化出GameObject
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;
            //创建抽屉
            if (!poolDic.ContainsKey(name))
            {
                poolDic.Add(name,new PoolData(poolObj,obj.name,obj)); 
            }
            else
            {
                //实例化出来的对象 需要记录到使用中的对象容器中
                poolDic[name].PushUsedList(obj);
            }
        }
        //如果有抽屉并且抽屉中有对象 或者 使用超过缓存池上限
        else
        {
            obj = poolDic[name].Pop();
        }
        
        return obj;
    }
    
    /// <summary>
    /// 获取自定义的数据结构类 和逻辑类 不继承Mono的
    /// </summary>
    /// <param name="nameSpace">命名空间名 有相同名字的类 但是不同命名空间 默认为null</param>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns></returns>
    public T GetObjcet<T>(string nameSpace = "") where T: class,IPoolClass,new()
    {
        //池子的名字就是类名 
        string poolName = "" + "_" + typeof(T).Name;
        //有池子的
        if (poolClassDic.ContainsKey(poolName))
        {
            PoolClass<T> pool = poolClassDic[poolName] as PoolClass<T>;
            //池子当中是否有可以复用的内容
            if (pool.poolObj.Count > 0)
            {
                //从队列中取出对象 进行复用
                T obj = pool.poolObj.Dequeue() as T;
                return obj;
            }
            //池子当中是空的
            else
            {
                //必须存在无参构造函数
                T obj = new T();
                return obj;
            }
        }
        //没有池子的
        else
        {
            //必须存在无参构造函数
            T obj = new T();
            return obj;
        }
    }

    /// <summary>
    /// 往缓存池中放入对象
    /// </summary>
    /// <param name="name">抽屉(对象)的名字</param>
    /// <param name="obj">希望放入的对象</param>
    public void PushObj(GameObject obj)
    {
        //栈(抽屉)中放对象
        poolDic[obj.name].Push(obj);
    }


    /// <summary>
    /// 将自定义的数据结构类 和逻辑类 加入池子
    /// </summary>
    /// <param name="obj">加入池子的对象</param>
    /// <param name="nameSpace">命名空间名 有相同名字的类 但是不同命名空间 默认为null</param>
    /// <typeparam name="T"></typeparam>
    public void PushObj<T>(T obj, string nameSpace = "") where T: class,IPoolClass,new()
    {
        if (obj == null)
        {
            Debug.LogError("警告:加入Pool缓存池的对象为空对象 请检查名字是否填写正确");
            return;
        }
        //池子的名字就是类名 
        string poolName = "" + "_" + typeof(T).Name;
        PoolClass<T> pool;
        //有池子的
        if (poolClassDic.ContainsKey(poolName))
        {
            //取出池子
            pool = poolClassDic[poolName] as PoolClass<T>;
        }
        //没有池子的
        else
        {
            pool = new PoolClass<T>();
            //将对象加入字典
            poolClassDic.Add(poolName,pool);
        }
        //在放入池子之前 先重置
        obj.ResetInfo();
        //加入对象
        pool.poolObj.Enqueue(obj);
        
    }
    
    /// <summary>
    /// 用于清空对象池
    /// 主要是切换场景时
    /// </summary>
    //如果切换场景 清空对象池 防止内存泄漏
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
        
        poolClassDic.Clear();
    }
    
    //防止外部new
    private PoolMgr()
    {
        
    }
}

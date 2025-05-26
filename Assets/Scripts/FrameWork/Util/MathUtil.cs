using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MathUtil
{
    #region 角度转弧度的方法

    /// <summary>
    /// 角度转弧度的方法
    /// </summary>
    /// <param name="deg">角度值</param>
    /// <returns></returns>
    public static float Deg2Rad(float deg)
    {
        return deg * Mathf.Deg2Rad;
    }
    
    /// <summary>
    /// 弧度转角度的方法
    /// </summary>
    /// <param name="rad">弧度值</param>
    /// <returns></returns>
    public static float Rad2Deg(float rad)
    {
        return rad * Mathf.Rad2Deg;
    }

    #endregion

    #region 距离判断相关

    /// <summary>
    /// 获取XZ平面上 两点的距离
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns></returns>
    public static float GetObjDistanceXZ(Vector3 srcPos, Vector3 targetPos)
    {
        srcPos.y = 0;
        targetPos.y = 0;
        return Vector3.Distance(srcPos, targetPos);
    }

    /// <summary>
    /// XZ平面上 判断两点之间距离是否小于目标距离 
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <param name="dis">距离</param>
    /// <returns></returns>
    public static bool CheckObjDistanceXZ(Vector3 srcPos, Vector3 targetPos, float dis)
    {
        return GetObjDistanceXZ(srcPos, targetPos) <= dis;
    }
        
    /// <summary>
    /// 获取XY平面上 两点的距离
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns></returns>
    public static float GetObjDistanceXY(Vector3 srcPos, Vector3 targetPos)
    {
        srcPos.z = 0;
        targetPos.z = 0;
        return Vector3.Distance(srcPos, targetPos);
    }
    
    /// <summary>
    /// XY平面上 判断两点之间距离是否小于目标距离 
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <param name="dis">距离</param>
    /// <returns></returns>
    public static bool CheckObjDistanceXY(Vector3 srcPos, Vector3 targetPos, float dis)
    {
        return GetObjDistanceXY(srcPos, targetPos) <= dis;
    }

    #endregion

    #region 屏幕外判断的方法

    /// <summary>
    /// 判断世界坐标系下的点 是否在屏幕可见范围内
    /// </summary>
    /// <param name="pos">世界坐标系下的位置</param>
    /// <returns>在可见范围内 返回true</returns>
    public static bool IsWorldPosOutScreen(Vector3 pos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        //判断是否在屏幕内
        if (screenPos.x >= 0 && screenPos.x <= Screen.width && 
            screenPos.y >= 0 && screenPos.y <= Screen.height)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region 扇形范围判断

    /// <summary>
    /// 判断某一个位置 是否在指定扇形范围内(注意:传入的坐标向量要基于同一个坐标系)
    /// </summary>
    /// <param name="pos">扇形中心点位置</param>
    /// <param name="forward">面朝向</param>
    /// <param name="targetPos">目标对象</param>
    /// <param name="radius">半径</param>
    /// <param name="angle">扇形角度</param>
    /// <returns>在范围内返回true</returns>
    public static bool IsInSectorRangeXZ(Vector3 pos, Vector3 forward, Vector3 targetPos, float radius, float angle)
    {
        pos.y = 0;
        forward.y = 0;
        targetPos.y = 0;
        //距离 角度
        //先判断距离是否在半径内
        //再判断面朝向的角度和目标向量 是否在扇形的1/2内
        return Vector3.Distance(pos, targetPos) <= radius && Vector3.Angle(forward, targetPos - pos) <= angle / 2f;

    }

    #endregion

    #region 射线检测

    /// <summary>
    /// 射线检测 获取一个对象
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数(会把碰到的RaycastHit信息传递出去)</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast(Ray ray, UnityAction<RaycastHit> callBack, float maxDistance, int layerMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray,out hitInfo,maxDistance,layerMask))
        {
            //传出委托
            callBack.Invoke(hitInfo);
        }
    }

    /// <summary>
    /// 射线检测 获取一个对象
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数(会把碰到的Gameobjec信息传递出去)</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast(Ray ray, UnityAction<GameObject> callBack, float maxDistance, int layerMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray,out hitInfo,maxDistance,layerMask))
        {
            //获取碰撞体 传出委托
            callBack.Invoke(hitInfo.collider.gameObject);
        }
    }
    
    /// <summary>
    /// 射线检测 获取一个对象
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数(会把碰到对象上挂载的指定脚本传递出去)</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast<T>(Ray ray, UnityAction<T> callBack, float maxDistance, int layerMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray,out hitInfo,maxDistance,layerMask))
        {
            //传出委托
            callBack.Invoke(hitInfo.collider.gameObject.GetComponent<T>());
        }
    }

    /// <summary>
    /// 射线检测 获取多个对象
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数(会把碰到的RaycastHit信息传递出去) 每个对象都会调用一次</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCastAll(Ray ray, UnityAction<RaycastHit> callBack, float maxDistance, int layerMask)
    {
        //获取碰撞到的所有物体信息
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
        for (int i = 0; i < hitInfos.Length; i++)
        {
            callBack.Invoke(hitInfos[i]);
        }
    }
    
    /// <summary>
    /// 射线检测 获取多个对象
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数(会把碰到的GameObject信息传递出去) 每个对象都会调用一次</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCastAll(Ray ray, UnityAction<GameObject> callBack, float maxDistance, int layerMask)
    {
        //获取碰撞到的所有物体信息
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
        for (int i = 0; i < hitInfos.Length; i++)
        {
            callBack.Invoke(hitInfos[i].collider.gameObject);
        }
    }
    
    /// <summary>
    /// 射线检测 获取多个对象
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数(会把碰到对象上挂载的指定脚本传递出去) 每个对象都会调用一次</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCastAll<T>(Ray ray, UnityAction<T> callBack, float maxDistance, int layerMask)
    {
        //获取碰撞到的所有物体信息
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
        for (int i = 0; i < hitInfos.Length; i++)
        {
            callBack.Invoke(hitInfos[i].collider.gameObject.GetComponent<T>());
        }
    }
    #endregion

    #region 范围检测

    /// <summary>
    /// 盒装范围检测
    /// </summary>
    /// <param name="center">盒子中心点</param>
    /// <param name="rotation">盒子的角度</param>
    /// <param name="halfExtents">长宽高的一半</param>
    /// <param name="layerMask">层级筛选</param>
    /// <param name="callBack">回调函数</param>
    /// <typeparam name="T">想要获取的信息类型 Collider GameObject 以及对象的依附脚本</typeparam>
    public static void OverlapBox<T>(Vector3 center, Quaternion rotation, Vector3 halfExtents, int layerMask,
        UnityAction<T> callBack) where T : class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapBox(center, halfExtents, rotation, layerMask,QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (type == typeof(Collider))
            {
                //获取Collider
                callBack.Invoke(colliders[i] as T);
            }
            else if (type == typeof(GameObject))
            {
                //获取GameObject
                callBack.Invoke(colliders[i].gameObject as T);
            }
            else
            {
                //获取依附的组件
                callBack.Invoke(colliders[i].gameObject.GetComponent<T>());
            }
        }
    }
    
    /// <summary>
    /// 球体范围检测
    /// </summary>
    /// <param name="center">球体中心点</param>
    /// <param name="radius">球体半径</param>
    /// <param name="layerMask">层级筛选</param>
    /// <param name="callBack">回调函数</param>
    /// <typeparam name="T">想要获取的信息类型 Collider GameObject 以及对象的依附脚本</typeparam>
    public static void OverlapSphere<T>(Vector3 center, float radius, int layerMask, UnityAction<T> callBack)
        where T : class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapSphere(center, radius, layerMask,QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (type == typeof(Collider))
            {
                //获取Collider
                callBack.Invoke(colliders[i] as T);
            }
            else if (type == typeof(GameObject))
            {
                //获取GameObject
                callBack.Invoke(colliders[i].gameObject as T);
            }
            else
            {
                //获取依附的组件
                callBack.Invoke(colliders[i].gameObject.GetComponent<T>());
            }
        }
    }

    #endregion

}

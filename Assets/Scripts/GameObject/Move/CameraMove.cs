using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //摄像机的目标对象 玩家
    public Transform playTransform;
    //相对于目标对象的偏移位置
    public Vector3 offsetPos;
    //移动速度
    public float moveSpeed;
    //需要到的位置
    private Vector3 targetPos;
    
    void FixedUpdate()
    {
        //判断有没有目标
        if (playTransform == null)
        {
            return;
        }
        //根据目标对象 来计算 摄像机当前的位置和角度
        //位置计算
        targetPos.x = playTransform.position.x + offsetPos.x;
        targetPos.y = playTransform.position.y + offsetPos.y;
        targetPos.z = playTransform.position.z + offsetPos.z;
        //用差值运算 让摄像机向目标靠拢 缓动效果
        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, moveSpeed * Time.deltaTime);
    }
    /// <summary>
    /// 设置摄像机看向的目标对象
    /// </summary>
    /// <param name="player"></param>
    public void SetTarget(Transform player)
    {
        playTransform = player;
    }
}

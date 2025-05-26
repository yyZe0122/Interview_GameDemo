using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTigger : MonoBehaviour
{
    /// <summary>
    /// 判断玩家是否接触door触发器
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 6)
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_Tigger_Door);
        }
    }
}

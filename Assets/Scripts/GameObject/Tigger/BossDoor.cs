using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_Tigger_BossDoor);
    }
}

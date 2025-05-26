using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeginCamera : MonoBehaviour
{
    public Animator animator;
    private UnityAction callBack;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FirstOrLast(bool firstOrLast,UnityAction callBack = null)
    {
        animator.SetBool("FirstOrLast",firstOrLast);
        this.callBack += callBack;
        Invoke("InvokeCallBack", 1f);
    }

    public void InvokeCallBack()
    {
        callBack?.Invoke();
        callBack = null;
    }
}

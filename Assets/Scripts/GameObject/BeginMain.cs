using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginMain : MonoBehaviour
{
    private void Awake()
    {
        UIMagr.Instance.ShowPanel<BeginPanel>();
        //开始播放背景音乐
        MusicMgr.Instance.PlayMusic("Dungeon-Crawler");
    }
}

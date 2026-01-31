/****************************************************
    文件：SystemRoot.cs
    作者：k0itoyuu
    日期：2025/12/16 20:42:13
    功能：业务系统基类
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemRoot : MonoBehaviour
{
    protected ResSvc resSvc;
    protected AudioSvc audioSvc;
    public virtual void  InitSys() {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
    }
}
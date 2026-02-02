/****************************************************
    文件：GameRoot.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：游戏入口
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance=null;
    public LoadingWnd loadingWind;
    public DynamicWnd dynamicWnd;
    public BottonWnd bottonWnd;
    
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        Debug.Log("GameStart>>>");

        Init();
    }
    private void Init()
    {
        //资源加载
        ResSvc res = GetComponent<ResSvc>();
        res.InitSvc();
        
         //音乐加载
         AudioSvc audio = GetComponent<AudioSvc>();
         audio.InitSvc();
        
        //业务系统初始化
         BeginSys begin = GetComponent<BeginSys>();
         begin.InitSys();
        
        
        //进入登录场景并加载相应UI
        begin.EnterLogin();
        bottonWnd.SetWndState();
        
    }
    public static void AddTips(string tips) {
        Instance.dynamicWnd.AddTips(tips);
    }
}

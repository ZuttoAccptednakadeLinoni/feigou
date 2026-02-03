/****************************************************
    文件：LoginSys.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：Nothing
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginSys : SystemRoot
{
    public static BeginSys Instance = null;
    //public SelectLevelWnd selectLevelWind;//选择关卡窗口
    //public CreateWnd createWnd;
    public BeginWnd beginWnd ;
    /// <summary>
    /// 初始化
    /// </summary>
    public override void InitSys() {
        base.InitSys();

        Instance = this;
        Debug.Log("Init LoginSys...");
    }

    /// <summary>
    /// 进入登录场景
    /// </summary>
    public void EnterLogin()
    {
        resSvc.AsyncLoadScene(Constants.SceneLogin, () => {
            //加载完成以后再打开选择界面
            //selectLevelWind.SetWndState();
            //audioSvc.PlayBGMusic(Constants.BGLogin);
            // GameRoot.AddTips(("左键加速，右键减速"));
            // GameRoot.AddTips(("load"));
            // GameRoot.AddTips(("load111"));
            // GameRoot.AddTips(("load31w"));
            beginWnd.SetWndState();
        });
    }   
    public void RspLogin() {
        GameRoot.AddTips("进入游戏");
     

        //selectLevelWind.SetWndState(false);
    }
}


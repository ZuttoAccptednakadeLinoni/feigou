/****************************************************
    文件：AchievementWnd.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：Nothing
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementWnd : WindowRoot
{
    public BeginWnd beginWnd;
    public BottonWnd bottonWnd;
    protected override void InitWnd() {
        base.InitWnd();
        //EventCenter.Instance.AddEventListener("ClickAction",ClickAction);//事件监听
    }
    public void ExitClick()
    {
        SetWndState(false);
        bottonWnd.SetWndState();
        beginWnd.SetWndState(true);
        beginWnd.SetMoveAvtive(90f);
    }

    
}

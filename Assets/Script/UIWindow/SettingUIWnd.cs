/****************************************************
    文件：SettingUI.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：音量设置界面
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIWnd : WindowRoot
{
	[Header("音乐控制")]
	[SerializeField] private Slider bgAudioSlider;
	
	[Header("音效控制")]
	[SerializeField] private Slider uiAudioSlider;

	public BeginWnd beginWnd;
	public BottonWnd bottonWnd;
	protected  override void InitWnd()
	{
		base.InitWnd();
		bottonWnd.SetWndState(false);
		InitializeUI();
	}
	private void InitializeUI()
	{
		// 根据配置设置滑块值
		bgAudioSlider.value = audioSvc.GetBgVolume();
		uiAudioSlider.value = audioSvc.GetUiVolume();
		//Debug.Log("UI初始化完成，音量滑块值: " + volumeSlider.value);
	}

	public void SaveClick()
	{
		Debug.Log(audioSvc==null);
		Debug.Log(resSvc==null);
		audioSvc.SetBGAudioVolume(bgAudioSlider.value);
		audioSvc.SetUIAudioVolume(uiAudioSlider.value);
	}
	public void ExitClick()
	{
		SetWndState(false);
		bottonWnd.SetWndState();
		beginWnd.SetWndState(true);
		beginWnd.SetMoveAvtive(270f);
	}
}

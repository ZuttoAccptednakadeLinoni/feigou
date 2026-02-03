/****************************************************
    文件：BeginWnd.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：Nothing
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginWnd : WindowRoot
{
	public GameObject achievementsUI;
	public GameObject developerUI;
	public GameObject gamePlayUI;
	public GameObject settingsUI;
	protected  override void InitWnd()
	{
		base.InitWnd();

		SetActive(achievementsUI, true);

		SetActive(developerUI, false);

		SetActive(gamePlayUI, true);

		SetActive(settingsUI, true);
		Debug.Log("Yes");
	}

	public void SetMoveAvtive(float currentYRotation)
	{
		if (currentYRotation == 0)
		{
			SetActive(achievementsUI, true);

			SetActive(developerUI, false);

			SetActive(gamePlayUI, true);

			SetActive(settingsUI, true);
		}else if (currentYRotation == 90)
		{
			SetActive(achievementsUI, true);

			SetActive(developerUI, true);

			SetActive(gamePlayUI, true);

			SetActive(settingsUI, false);
		}else if (currentYRotation == 180)
		{
			SetActive(achievementsUI, true);

			SetActive(developerUI, true);

			SetActive(gamePlayUI, false);

			SetActive(settingsUI, true);
		}else if (currentYRotation == 270)
		{
			SetActive(achievementsUI, false);

			SetActive(developerUI, true);

			SetActive(gamePlayUI, true);

			SetActive(settingsUI, true);
		}
	}
}

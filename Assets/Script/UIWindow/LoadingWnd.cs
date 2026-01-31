/****************************************************
    文件：LoadingWind.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：等待加载界面
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingWnd : WindowRoot
{
    public TextMeshProUGUI txtTips;
    public Image ImgFG;
    public Image imgPoint;
    public TextMeshProUGUI txtPrg;

    private float fgWidth;
    public void InitWind()
    {
        fgWidth = ImgFG.GetComponent<RectTransform>().sizeDelta.x;
        SetText(txtTips, "这是一条游戏Tips");
        SetText(txtPrg, "0%");
        ImgFG.fillAmount = 0;
        imgPoint.transform.localPosition = new Vector3(-545f, 0, 0);
        
    }

    public void SetProgress(float prg)
    {
        SetText(txtPrg, (int)(prg * 100) + "%");
        ImgFG.fillAmount = prg;
        
        float posx=prg * fgWidth - 545;
        imgPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(posx, 0);
    }
}


/****************************************************
    文件：LoadingWind.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：等待加载界面
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingWnd : WindowRoot
{
    [Header("TMP Text References")]
    [SerializeField] private TextMeshProUGUI[] tmpTexts; // 三个TMP文本
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float intervalTime = 1f;
    [SerializeField] private float initialDelay = 0.5f;

    protected  override void InitWnd()
    {
        base.InitWnd();
        InitializeTexts();
        StartSequenceAnimation();
    }

    // 初始化文本为不可见
    private void InitializeTexts()
    {
        foreach (var tmp in tmpTexts)
        {
            // 设置透明度为0
            Color color = tmp.color;
            color.a = 0;
            tmp.color = color;
            tmp.gameObject.SetActive(true); // 确保物体激活
        }
    }

    // 开始顺序动画
    private void StartSequenceAnimation()
    {
        // 创建一个序列
        Sequence sequence = DOTween.Sequence();
        
        // 添加初始延迟
        sequence.AppendInterval(initialDelay);
        
        // 为每个TMP添加淡入动画
        for (int i = 0; i < tmpTexts.Length; i++)
        {
            int index = i; // 避免闭包问题
            
            // 添加延迟（除了第一个）
            if (i > 0)
            {
                sequence.AppendInterval(intervalTime);
            }
            
            // 添加淡入动画
            sequence.Append(tmpTexts[index].DOFade(1, fadeDuration));
        }
        
        // 设置循环（可选）
        //sequence.SetLoops(-1, LoopType.Restart);
    }

    // 手动开始动画（如果不在Start中调用）
    public void StartLoadingAnimation()
    {
        InitializeTexts();
        StartSequenceAnimation();
    }

    // 停止动画
    public void StopAnimation()
    {
        DOTween.Kill(this); // 停止所有属于这个对象的动画
        // 或者 DOTween.KillAll();
    }

    // 重置所有文本
    public void ResetTexts()
    {
        foreach (var tmp in tmpTexts)
        {
            Color color = tmp.color;
            color.a = 0;
            tmp.color = color;
        }
    }
}


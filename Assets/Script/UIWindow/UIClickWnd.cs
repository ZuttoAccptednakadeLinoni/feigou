/****************************************************
    文件：UIClickWnd.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：Nothing
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
public class UIClickWnd : WindowRoot
{
[Header("初始化设置")]
    [Tooltip("物体初始时在目标位置上方的高度偏移")]
    [SerializeField] private float startHeightOffset = 5f;
    
    [Tooltip("移动到初始位置的持续时间")]
    [SerializeField] private float dropDuration = 1f;
    
    [Tooltip("下落使用的缓动函数")]
    [SerializeField] private Ease dropEase = Ease.OutBounce;
    
    [Tooltip("延迟开始时间（秒）")]
    [SerializeField] private float startDelay = 0f;
    
    [Header("高级设置")]
    [Tooltip("使用本地坐标而不是世界坐标")]
    [SerializeField] private bool useLocalPosition = false;
    
    [Tooltip("是否在启动时自动播放动画")]
    [SerializeField] private bool playOnStart = true;
    
    private Vector3 targetPosition; // 初始位置（目标位置）
    private Sequence dropSequence;

    private void Start() {
        targetPosition = useLocalPosition ? transform.localPosition : transform.position;
        if (playOnStart)
        {
            StartDropAnimation();
        }
    }
    public void ClickLog() {
        Debug.Log(true);
    }
    
    void OnDestroy()
    {
        // 清理动画序列
        if (dropSequence != null && dropSequence.IsActive())
        {
            dropSequence.Kill();
        }
    }
    
    /// <summary>
    /// 开始下落动画
    /// </summary>
    public void StartDropAnimation()
    {
        // 设置初始位置（在目标位置上方）
        Vector3 startPosition = targetPosition + Vector3.up * startHeightOffset;
        
        if (useLocalPosition)
        {
            transform.localPosition = startPosition;
        }
        else
        {
            transform.position = startPosition;
        }
        // 创建下落动画
        CreateDropSequence();
        // 延迟后播放动画
         dropSequence.SetDelay(startDelay).Play();
    }
    
    /// <summary>
    /// 创建下落动画序列
    /// </summary>
    private void CreateDropSequence()
    {
        // 清理之前的动画
        if (dropSequence != null && dropSequence.IsActive())
        {
            dropSequence.Kill();
        }
        
        dropSequence = DOTween.Sequence();
        AddSimpleDropAnimation();
    }
    
    /// <summary>
    /// 添加简单下落动画
    /// </summary>
    private void AddSimpleDropAnimation()
    {
        if (useLocalPosition)
        {
            dropSequence.Append(transform.DOLocalMove(targetPosition, dropDuration)
                .SetEase(dropEase));
        }
        else
        {
            dropSequence.Append(transform.DOMove(targetPosition, dropDuration)
                .SetEase(dropEase));
        }
    }
    
}


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
    
    [Header("动画效果")]
    [Tooltip("是否启用下落时的弹跳效果")]
    [SerializeField] private bool enableBounce = true;
    
    [Tooltip("弹跳次数（仅在启用弹跳时有效）")]
    [SerializeField] private int bounceCount = 1;
    
    [Tooltip("弹跳高度（相对于总下落距离的百分比）")]
    [SerializeField] private float bounceHeight = 0.3f;
    
    [Header("高级设置")]
    [Tooltip("使用本地坐标而不是世界坐标")]
    [SerializeField] private bool useLocalPosition = false;
    
    [Tooltip("是否在启动时自动播放动画")]
    [SerializeField] private bool playOnStart = true;
    
    private Vector3 targetPosition; // 初始位置（目标位置）
    private Sequence dropSequence;
    
    void Awake()
    {
        // 记录目标位置（物体的初始位置）
        targetPosition = useLocalPosition ? transform.localPosition : transform.position;
    }
    
    void Start()
    {
        if (playOnStart)
        {
            StartDropAnimation();
        }
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
        
        
            // 普通下落
            AddSimpleDropAnimation();

        
        // 动画完成回调
        dropSequence.OnComplete(() => 
        {
            Debug.Log("物体已移动到初始位置");
            OnDropComplete();
        });
        
        dropSequence.OnKill(() => 
        {
            // 确保物体最终在目标位置
            if (useLocalPosition)
            {
                transform.localPosition = targetPosition;
            }
            else
            {
                transform.position = targetPosition;
            }
        });
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
    
    
    /// <summary>
    /// 下落动画完成时的回调
    /// </summary>
    protected virtual void OnDropComplete()
    {
        // 可以在这里添加自定义逻辑
        // 例如：启用碰撞体、播放声音等
    }
    
    /// <summary>
    /// 重置物体到起始位置（上方）
    /// </summary>
    public void ResetToStartPosition()
    {
        // 停止当前动画
        if (dropSequence != null && dropSequence.IsActive())
        {
            dropSequence.Kill();
        }
        
        // 重置到起始位置
        Vector3 startPosition = targetPosition + Vector3.up * startHeightOffset;
        
        if (useLocalPosition)
        {
            transform.localPosition = startPosition;
        }
        else
        {
            transform.position = startPosition;
        }
    }

    /// <summary>
    /// 设置新的下落参数
    /// </summary>
    public void SetDropParameters(float newHeightOffset, float newDuration, Ease newEase = Ease.OutBounce)
    {
        startHeightOffset = newHeightOffset;
        dropDuration = newDuration;
        dropEase = newEase;
    }
  
}


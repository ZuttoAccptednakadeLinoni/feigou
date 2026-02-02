/****************************************************
    文件：BottonWnd.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：选择按钮界面
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BottonWnd : WindowRoot
{
     [Header("UI按钮")]
    public Button leftButton;
    public Button rightButton;
    public Button resetButton;
    
    [Header("旋转目标")]
    public Transform beginObject;
    
    [Header("Tween设置")]
    public float rotationDuration = 0.5f;
    public Ease rotationEase = Ease.OutBack;
    public RotateMode rotateMode = RotateMode.FastBeyond360;
    
    private Sequence rotationSequence;
    private float currentYRotation = 0f;
    
    void Start()
    {

        
        // 保存初始旋转
        if (beginObject != null)
        {
            currentYRotation = beginObject.eulerAngles.y;
        }
    }
    
    void OnDestroy()
    {
        // 清理DOTween
        if (rotationSequence != null && rotationSequence.IsActive())
        {
            rotationSequence.Kill();
        }
        

    }
    
    void RotateLeft()
    {
        // 从右向左旋转：Y轴减90度
        RotateTo(currentYRotation - 90f);
    }
    
    void RotateRight()
    {
        // 从左向右旋转：Y轴加90度
        RotateTo(currentYRotation + 90f);
    }
    
    void ResetRotation()
    {
        // 重置为0度
        RotateTo(0f);
    }
    
    void RotateTo(float targetAngle)
    {
        // 停止当前动画
        if (rotationSequence != null && rotationSequence.IsActive())
        {
            rotationSequence.Kill();
        }
        
        // 更新当前角度
        currentYRotation = targetAngle;
        
        // 使用DOTween旋转
        rotationSequence = DOTween.Sequence();
        rotationSequence.Append(
            beginObject.DORotate(
                new Vector3(0, targetAngle, 0),
                rotationDuration,
                rotateMode
            ).SetEase(rotationEase)
        );
        
        // 可以添加回调
        rotationSequence.OnComplete(() => {
            Debug.Log($"旋转完成，当前角度: {beginObject.eulerAngles.y:F1}°");
        });
    }
    
    // 快速旋转方法
    public void QuickRotateLeft()
    {
        RotateWithDuration(currentYRotation - 90f, 0.2f);
    }
    
    public void QuickRotateRight()
    {
        RotateWithDuration(currentYRotation + 90f, 0.2f);
    }
    
    void RotateWithDuration(float targetAngle, float duration)
    {
        currentYRotation = targetAngle;
        beginObject.DOKill(); // 停止所有DOTween动画
        beginObject.DORotate(new Vector3(0, targetAngle, 0), duration)
            .SetEase(Ease.OutQuad);
    }
}

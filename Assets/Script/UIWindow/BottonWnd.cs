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
    
    public LoadingWnd loadingWnd;
    public BeginWnd beginWnd;
    public SettingUIWnd settingUiWnd;
    public AchievementWnd achievementWnd;
    
    
    private Sequence rotationSequence;
    private float currentYRotation = 0f;
    protected  override void InitWnd()
    {
        base.InitWnd();
        EventCenter.Instance.AddEventListener("ClickAction",ClickAction);//事件监听
    }
    private void ClickAction()//事件监听
    {
        Debug.Log("True");
        achievementWnd.SetWndState();
        
    }
    void OnDestroy()
    {
        // 清理DOTween
        if (rotationSequence != null && rotationSequence.IsActive())
        {
            rotationSequence.Kill();
        }
        

    }
    public void RotateCenter()
    {
        
        if (currentYRotation == 0)//开始游戏
        {
            loadingWnd.SetWndState(true);
            beginWnd.SetWndState(false);
            Debug.Log(Constants.Level1);
            resSvc.AsyncLoadScene(Constants.Level1, () =>
            {
                SetWndState(false);
            });
        }else if (currentYRotation == 270)
        {
            beginWnd.SetWndState(false);
            settingUiWnd.SetWndState();
            
        }else if (currentYRotation == 90) {Debug.Log(currentYRotation);
            EventCenter.Instance.EventTrigger("ClickAction");
        }
    }
   
    // 快速旋转方法
    public void QuickRotateLeft()//向左
    {
        beginWnd.SetMoveAvtive((currentYRotation - 90f+360)%360);
        RotateWithDuration((currentYRotation - 90f+360)%360, 0.2f);
    }
    
    public void QuickRotateRight()//向右
    {
        beginWnd.SetMoveAvtive((currentYRotation + 90f)%360);
        RotateWithDuration((currentYRotation + 90f)%360, 0.2f);
    }
    
    void RotateWithDuration(float targetAngle, float duration)
    {
        currentYRotation = targetAngle;
        beginObject.DOKill(); // 停止所有DOTween动画
        beginObject.DORotate(new Vector3(0, targetAngle, 0), duration)
            .SetEase(Ease.OutQuad);
    }
}

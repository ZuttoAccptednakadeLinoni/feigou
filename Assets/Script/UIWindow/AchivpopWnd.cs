/****************************************************
    文件：AchivpopWnd.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：弹出窗口
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AchivpopWnd : WindowRoot
{
	public GameObject popUI;
	public float animationDuration = 0.5f; // 动画持续时间
	public float displayDuration = 3f; // 显示持续时间
	public float popupHeight = 100f; //
	//protected EventCenter eventCenter= null;
	
	
	private RectTransform popRectTransform;
	private Vector2 originalPosition;
	protected  override void InitWnd()
	{
		base.InitWnd();
		EventCenter.Instance.AddEventListener("MoveAchiv",MoveAchiv);//事件监听
		popRectTransform = popUI.GetComponent<RectTransform>();
		if (popRectTransform != null)
		{
			// 保存原始位置（屏幕下方）
			originalPosition = popRectTransform.anchoredPosition;
		}
		Debug.Log("Yes");
	}
	private void MoveAchiv()//事件监听
	{
		popUI.SetActive(true);
		StartUIPopupAnimation();
	}
	private void StartUIPopupAnimation()
	{
		// 1. 先将UI移动到屏幕下方（初始位置）
		popRectTransform.anchoredPosition = new Vector2(
			originalPosition.x, 
			originalPosition.y - popupHeight
		);
		// 2. 创建动画序列
		Sequence sequence = DOTween.Sequence();
        
		// 向上弹出动画
		sequence.Append(popRectTransform.DOAnchorPos(originalPosition, animationDuration)
			.SetEase(Ease.OutBack)); // OutBack效果会有轻微超过然后弹回的效果
        
		// 等待3秒
		sequence.AppendInterval(displayDuration);
        
		// 向下收回动画（可选）
		// 如果不需要收回动画，可以跳过这一步，直接设置为不可见
		sequence.Append(popRectTransform.DOAnchorPos(
				new Vector2(originalPosition.x, originalPosition.y - popupHeight), 
				animationDuration)
			.SetEase(Ease.InBack));
        
		// 动画完成后设置为不可见
		sequence.OnComplete(() => {
			popUI.SetActive(false);
			// 重置位置（可选）
			popRectTransform.anchoredPosition = originalPosition;
		});
		sequence.Play();
	}

}

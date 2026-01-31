/****************************************************
    文件：DynamicWnd.cs
    作者：k0itoyuu
    日期：#CreateTime#
    功能：动态UI元素界面
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DynamicWnd : WindowRoot
{
    public Animation tipsAni;
    public  TextMeshProUGUI txtTips;
    
    private bool isTipsShow = false;
    private Queue<string> tipsQue = new Queue<string>();
    protected override void InitWnd() {
        base.InitWnd();

        SetActive(txtTips, false);
    }

    public void AddTips(string tips) {
        lock (tipsQue) {
            tipsQue.Enqueue(tips);
        }
    }
    private void Update() {
        if (tipsQue.Count > 0 && isTipsShow == false) {
            lock (tipsQue) {
                string tips = tipsQue.Dequeue();
                isTipsShow = true;
                SetTips(tips);
            }
        }
    }
    private void SetTips(string tips) {
        Debug.Log(txtTips==null);
        SetActive(txtTips);
        SetText(txtTips, tips);

        AnimationClip clip = tipsAni.GetClip("New Animation");
        tipsAni.Play();
        //延时关闭激活状态

        AniPlayDone(clip.length, () => {
            SetActive(txtTips, false);
            isTipsShow = false;
        });
    }
    private async void AniPlayDone(float sec, Action cb) {
        Debug.Log(sec);
        await UniTask.Delay((int)(sec * 1000));
        if (cb != null) {
            cb();
        }
    }
}


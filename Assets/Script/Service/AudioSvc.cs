/****************************************************
    文件：AudioSvc.cs
    作者：k0itoyuu
    日期：#CreateTime#
    功能：音频加载
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSvc : MonoBehaviour
{
    public static AudioSvc Instance = null;
    public AudioSource bgAudio;
    public AudioSource uiAudio;
    public void InitSvc() {
        Instance = this;
        Debug.Log("Init AudioSvc...");
    }
    public void PlayBGMusic(string name, bool isLoop = true) {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        if (bgAudio.clip == null || bgAudio.clip.name != audio.name) {
            bgAudio.clip = audio;
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }

    public void PlayUIAudio(string name) {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        uiAudio.clip = audio;
        uiAudio.Play();
    }
}

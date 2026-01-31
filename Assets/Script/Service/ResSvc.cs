/****************************************************
    文件：ResSvc.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：资源加载
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///
/// 资源加载服务
/// </summary>
public class ResSvc : MonoBehaviour
{
    public static ResSvc Instance = null;
    public void InitSvc()
    {
        Instance = this;
        Debug.Log("InitSvc");
    }

    public Action prgCB = null;
    public async void AsyncLoadScene(string scenename, Action loaded)
    {
        GameRoot.Instance.loadingWind.SetWndState();
        await SceneManager.LoadSceneAsync(scenename).ToUniTask(
            (Progress.Create<float>((p) =>
            {
                GameRoot.Instance.loadingWind.SetProgress(p);
                Debug.Log(p);
                if (p *100>= 1)
                {
                    Debug.Log("加载完成");
                    if (loaded != null) {
                        loaded();
                    }
                    prgCB = null;
                    GameRoot.Instance.loadingWind.gameObject.SetActive(false);
                }
            })));
 
        

    }

    private void Update()
    {
        if (prgCB != null)
        {
            prgCB();
        }
    }
    private Dictionary<string, AudioClip> adDic = new Dictionary<string, AudioClip>();
    public AudioClip LoadAudio(string path, bool cache = false) {
        AudioClip au = null;
        if (!adDic.TryGetValue(path, out au)) {
            au = Resources.Load<AudioClip>(path);
            if (cache) {
                adDic.Add(path, au);
            }
        }
        return au;
    }
    #region InitCfgs
    
    #endregion
}


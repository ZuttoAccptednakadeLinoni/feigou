/****************************************************
    文件：WindowRoot.cs
    作者：k0itoyuu
    日期：#CreateTime#
    功能：UI加载父类
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowRoot : MonoBehaviour
{
    // Start is called before the first frame update
    protected ResSvc resSvc = null;
    protected AudioSvc audioSvc = null;
    public void SetWndState(bool isActive = true) {//设置窗口可见
        if (gameObject.activeSelf != isActive) {
            SetActive(gameObject, isActive);
        }
        if (isActive) {
            InitWnd();
        }
        else {
            ClearWnd();
        }
    }
    protected virtual void InitWnd() {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
    }

    protected virtual void ClearWnd() {
        resSvc = null;
        audioSvc = null;
    }
    #region Tool Functions

    protected void SetActive(GameObject go, bool isActive = true) {
        go.SetActive(isActive);
    }
    protected void SetActive(Transform trans, bool state = true) {
        trans.gameObject.SetActive(state);
    }
    protected void SetActive(RectTransform rectTrans, bool state = true) {
        rectTrans.gameObject.SetActive(state);
    }
    protected void SetActive(Image img, bool state = true) {
        img.transform.gameObject.SetActive(state);
    }
    protected void SetActive(TextMeshProUGUI txt, bool state = true) {
        txt.transform.gameObject.SetActive(state);
    }

    protected void SetText(TextMeshProUGUI txt, string context = "") {
        txt.text = context;
    }
    protected void SetText(Transform trans, int num = 0) {
        SetText(trans.GetComponent<TextMeshProUGUI>(), num);
    }
    protected void SetText(Transform trans, string context = "") {
        SetText(trans.GetComponent<TextMeshProUGUI>(), context);
    }
    protected void SetText(TextMeshProUGUI txt, int num = 0) {
        SetText(txt, num.ToString());
    }

    #endregion
}

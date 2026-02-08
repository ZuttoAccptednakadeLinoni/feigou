using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CursorVisualController : MonoBehaviour
{
    public RectTransform cursorRect;
    public Image cursorImage;

    [Header("Sprites")]
    public Sprite normalSprite;      // Normal 和 Hover 共用
    public Sprite lockedSprite;      // 只有锁定时才换

    public Sprite detailSprite;      // 细节查看时的光标

    [Header("Scale")]
    public float normalScale = 1f;
    public float hoverScale = 1.2f;
    public float scaleSpeed = 10f;

    private Coroutine scaleRoutine;
    private bool isLocked = false;
    public Transform lockedTarget;
    private Camera worldCamera;

    void Start(){
        Cursor.visible = false;
        worldCamera = Camera.main;
        cursorImage.sprite = normalSprite;
        cursorRect.localScale = Vector3.one * normalScale;
    }

    void Update(){
        if (!isLocked){
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                cursorRect.parent as RectTransform,
                Input.mousePosition,
                null,
                out pos
            );
            cursorRect.anchoredPosition = pos;
        }
        else if (lockedTarget != null){
            Vector3 screenPos = worldCamera.WorldToScreenPoint(lockedTarget.position);

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                cursorRect.parent as RectTransform,
                screenPos,
                null,
                out pos
            );
            cursorRect.anchoredPosition = pos;
        }
    }
    public void SetDetail() {
        cursorImage.sprite = detailSprite;
        SmoothScaleTo(hoverScale);
    }

    public void SetNormal(){
        if (isLocked) return;
        cursorImage.sprite = normalSprite;
        SmoothScaleTo(normalScale);
    }

    public void SetHover(){
        if (isLocked) return;
        cursorImage.sprite = normalSprite;   // 不换图，只放大
        SmoothScaleTo(hoverScale);
    }

    public void SetLocked(Transform target){
        isLocked = true;
        lockedTarget = target;
        cursorImage.sprite = lockedSprite;   // 只有锁定时换图
        SmoothScaleTo(hoverScale);
    }

    public void Unlock(){
        isLocked = false;
        lockedTarget = null;
        cursorImage.sprite = normalSprite;
        SmoothScaleTo(normalScale);
    }

    private void SmoothScaleTo(float targetScale){
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleRoutine(targetScale));
    }

    private IEnumerator ScaleRoutine(float targetScale){
        while (Mathf.Abs(cursorRect.localScale.x - targetScale) > 0.001f){
            float s = Mathf.Lerp(cursorRect.localScale.x, targetScale, Time.deltaTime * scaleSpeed);
            cursorRect.localScale = Vector3.one * s;
            yield return null;
        }
        cursorRect.localScale = Vector3.one * targetScale;
        scaleRoutine = null;
    }
}

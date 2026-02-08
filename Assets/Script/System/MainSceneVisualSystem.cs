using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class MainSceneVisualSystem : MonoBehaviour
{
    private enum VisualState
    {
        Normal,
        Hover,
        Locked,
        detail
    }
    private VisualState currentState = VisualState.Normal;
    [Header("Camera")]
    public Camera gameCamera;

    [Header("Layer")]
    public LayerMask planetLayerMask;

    [Header("Planet Hover Settings")]
    public float scaleMultiplier = 1.1f;
    public float scaleSpeed = 10f;
    private PlanetHover currentHover;
    public CursorVisualController cursor;
    public CameraInspectController cameraInspect;

    void Update(){
        PlanetHover hover = null;
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        bool ishit = Physics.Raycast(ray, out RaycastHit hit, 1000000f, planetLayerMask);

        if (ishit) {
            hover = hit.collider.GetComponentInParent<PlanetHover>();
        }
        //Debug.Log("当前状态"+currentState);

        switch(currentState){
            case VisualState.Normal:
                if (ishit){//普通进hover
                    currentHover = hover;
                    currentHover.SetHover(true, scaleMultiplier, scaleSpeed);
                    cursor.SetHover();
                    currentState = VisualState.Hover;
                }
                else if(currentHover != null){//防止漏掉离开hover
                    currentHover.SetHover(false, scaleMultiplier, scaleSpeed);
                    currentHover = null;
                }
                break;
            case VisualState.Hover:
                if (!ishit){//离开hover
                    currentHover.SetHover(false, scaleMultiplier, scaleSpeed);
                    currentHover = null;
                    cursor.SetNormal();
                    currentState = VisualState.Normal;
                }
                else if(hover != currentHover){//hover之间切换
                    currentHover.SetHover(false, scaleMultiplier, scaleSpeed);
                    currentHover = hover;
                    currentHover.SetHover(true, scaleMultiplier, scaleSpeed);
                    cursor.SetHover();
                }
                else if(Input.GetMouseButtonDown(0)){//点击进入detail
                    cursor.SetLocked(currentHover.GetComponentInParent<Transform>());
                    currentState = VisualState.Locked;
                }
                break;
            case VisualState.Locked:
                if(Input.GetMouseButtonDown(0)){//点击退出detail
                    cursor.Unlock();
                    cursor.SetNormal();
                    currentState = VisualState.Normal;
                }
                if(Input.GetKeyDown(KeyCode.E)){//进入Detail
                    cursor.Unlock();
                    cursor.SetDetail();
                    cameraInspect.SetDetailOn(currentHover.GetComponentInParent<PlanetController>().cameraTarget);
                    currentState = VisualState.detail;
                }
                break;
            case VisualState.detail:
                if(Input.GetKeyDown(KeyCode.E)){//退出Detail
                    cursor.Unlock();
                    cursor.SetNormal();
                    cameraInspect.SetDetailOff(currentHover.GetComponentInParent<PlanetController>().cameraTarget);
                    currentState = VisualState.Normal;
                }
                break;
        }
    }
}

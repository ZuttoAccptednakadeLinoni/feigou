using UnityEngine;

public class CameraInspectController : MonoBehaviour {
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject mainvirtualCamera;
    public void SetDetailOn(GameObject target) {
        target.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 999;
    }
    public void SetDetailOff(GameObject target) {
        target.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 0;
    }
    void Start() {
        mainvirtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 100;
    }
}

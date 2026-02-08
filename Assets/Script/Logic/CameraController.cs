/****************************************************
    文件：CameraController.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：相机控制
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class CameraController  : MonoBehaviour
{
    [Header("目标设置")]
    [Tooltip("要跟踪速度的目标对象")]
    public Rigidbody2D  targetObject;
    
    
    [Header("视野设置")]
    [Tooltip("最小正交大小（速度最小时）")]
    [Range(1f, 10f)]
    public float minOrthoSize = 5f;
    [Tooltip("最大正交大小（速度最大时）")]
    [Range(5f, 20f)]
    public float maxOrthoSize = 15f;
    
    [Header("速度阈值")]
    [Tooltip("视为最小速度的值")]
    [Range(0f, 10f)]
    public float minSpeed = 0f;
    [Tooltip("视为最大速度的值")]
    [Range(5f, 50f)]
    public float maxSpeed = 20f;
    
    [Header("平滑过渡")]
    [Tooltip("视野变化平滑时间（秒）")]
    [Range(0f, 1f)]
    public float smoothTime = 0.3f;
    [Tooltip("是否使用平滑阻尼")]
    public bool useSmoothing = true;
    
    [Header("调试信息")]
    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private float targetOrthoSize;
    [SerializeField]
    private float currentOrthoSize;
    
    // 私有变量
    private CinemachineVirtualCamera virtualCamera;
    private Vector3 lastPosition;
    private float velocity;
    private Camera cam;
    void Start()
    {
        // 获取相机组件
        //virtualCamera =GetComponent<CinemachineVirtualCamera>();
        cam = GetComponent<Camera>();
        
        // 初始化当前正交大小
        // currentOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        // targetOrthoSize = currentOrthoSize;
        currentOrthoSize = cam.orthographicSize;
        targetOrthoSize = currentOrthoSize;
    }
    
    void Update()
    {
        // 如果目标对象不存在，返回
        if (targetObject == null) return;
        
        // 计算当前速度
        currentSpeed = targetObject.velocity.magnitude ;
        //Debug.Log(currentSpeed);
        // 根据速度计算目标视野大小
        float speedRatio = Mathf.Clamp01((currentSpeed - minSpeed) / (maxSpeed - minSpeed));
        targetOrthoSize = Mathf.Lerp(minOrthoSize, maxOrthoSize, speedRatio);
        
        // 应用视野变化
        if (useSmoothing)
        {
            currentOrthoSize = Mathf.SmoothDamp(currentOrthoSize, targetOrthoSize, ref velocity, smoothTime);
            cam.fieldOfView = currentOrthoSize*10;
            Debug.Log(currentOrthoSize);
            // currentOrthoSize = Mathf.SmoothDamp(currentOrthoSize, targetOrthoSize, ref velocity, smoothTime);
            // virtualCamera.m_Lens.OrthographicSize = currentOrthoSize;
            // //Debug.Log(cam.orthographicSize);
        }
        
    }
}
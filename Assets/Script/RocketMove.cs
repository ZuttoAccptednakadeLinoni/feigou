/****************************************************
    文件：RocketMove.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：控制火箭移动
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDirectionMovement : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float minSpeed = 0.1f;
    
    [Header("转向参数")]
    [SerializeField] private bool enableRotation = true; // 是否启用旋转
    [SerializeField] private float rotationSpeed = 10f; // 旋转速度
    [SerializeField] private float rotationSmoothTime = 0.2f; // 旋转平滑时间
    [SerializeField] private RotationMode rotationMode = RotationMode.VelocityDirection; // 旋转模式
    
    [Header("鼠标参数")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool useCameraCenter = true;
    [SerializeField] private bool normalizeDirection = true;
    
    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private Vector2 mouseDirection;
    private float currentSpeed;
    private bool isMoving = false;
    
    // 旋转相关变量
    private float currentRotationAngle = 0f;
    private float targetRotationAngle = 0f;
    private float rotationVelocity = 0f;
    
    // 方向箭头指示器（可选）
    [SerializeField] private Transform directionIndicator;
    [SerializeField] private float indicatorOffset = 0.5f;
    
    // 旋转模式枚举
    public enum RotationMode
    {
        VelocityDirection,    // 朝向速度方向
        MouseDirection,       // 朝向鼠标方向
        Hybrid,              // 混合模式（低速时朝向鼠标，高速时朝向速度）
        Custom               // 自定义旋转
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("需要Rigidbody2D组件！");
            gameObject.AddComponent<Rigidbody2D>();
            rb = GetComponent<Rigidbody2D>();
        }
        
        // 设置刚体属性
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.drag = 0.5f;
        
        // 如果没有指定相机，使用主相机
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                Debug.LogError("没有找到主相机！");
            }
        }
        
        currentSpeed = 0f;
        currentRotationAngle = transform.eulerAngles.z;
        targetRotationAngle = currentRotationAngle;
    }

    void Update()
    {
        // 获取鼠标位置并计算方向
        CalculateMouseDirection();
        
        // 检查鼠标按键
        CheckMouseInput();
        
        // 更新移动状态
        UpdateMovementState();
        
        // 计算目标旋转角度
        CalculateTargetRotation();
    }

    void FixedUpdate()
    {
        // 物理更新
        HandleMovement();
        
        // 更新旋转
        HandleRotation();
    }

    void CalculateMouseDirection()
    {
        if (targetCamera == null) return;
        
        // 获取鼠标在世界中的位置
        Vector3 mouseWorldPosition = targetCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
                       -targetCamera.transform.position.z)
        );
        
        // 计算从物体到鼠标的方向
        if (useCameraCenter)
        {
            // 从相机中心计算方向（适合射击类游戏）
            Vector3 cameraPosition = targetCamera.transform.position;
            mouseDirection = (mouseWorldPosition - (Vector3)cameraPosition).normalized;
        }
        else
        {
            // 从物体当前位置计算方向（适合跟随鼠标移动）
            mouseDirection = (mouseWorldPosition - transform.position).normalized;
        }
        
        // 如果不归一化，方向向量长度会随距离变化
        if (!normalizeDirection && !useCameraCenter)
        {
            mouseDirection = (mouseWorldPosition - transform.position);
        }
    }

    void CheckMouseInput()
    {
        // 左键：加速
        if (Input.GetMouseButton(0))
        {
            Accelerate();
        }
        // 右键：减速
        else if (Input.GetMouseButton(1))
        {
            Decelerate();
        }
        // 没有按键：自然减速
        else
        {
            NaturalDeceleration();
        }
    }

    void Accelerate()
    {
        // 计算目标速度
        Vector2 targetVelocity = mouseDirection * moveSpeed;
        
        // 平滑加速
        currentVelocity = Vector2.Lerp(
            currentVelocity, 
            targetVelocity, 
            acceleration * Time.deltaTime
        );
        
        // 更新当前速度值
        currentSpeed = currentVelocity.magnitude;
        isMoving = true;
    }

    void Decelerate()
    {
        // 计算减速方向（与鼠标方向相反）
        Vector2 decelerationDirection = -mouseDirection;
        Vector2 decelerationForce = decelerationDirection * deceleration;
        
        // 应用减速力
        currentVelocity += decelerationForce * Time.deltaTime;
        
        // 如果速度很小，直接停止
        if (currentVelocity.magnitude < minSpeed)
        {
            currentVelocity = Vector2.zero;
            currentSpeed = 0f;
            isMoving = false;
        }
        else
        {
            currentSpeed = currentVelocity.magnitude;
            isMoving = true;
        }
    }

    void NaturalDeceleration()
    {
        // 逐渐减速到0
        currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * 0.5f * Time.deltaTime);
        
        // 检查是否停止
        if (currentVelocity.magnitude < minSpeed)
        {
            currentVelocity = Vector2.zero;
            currentSpeed = 0f;
            isMoving = false;
        }
        else
        {
            currentSpeed = currentVelocity.magnitude;
            isMoving = true;
        }
    }

    void HandleMovement()
    {
        // 限制最大速度
        currentVelocity = Vector2.ClampMagnitude(currentVelocity, maxSpeed);
        
        // 应用速度到刚体
        rb.velocity = currentVelocity;
    }

    void CalculateTargetRotation()
    {
        if (!enableRotation) return;
        
        switch (rotationMode)
        {
            case RotationMode.VelocityDirection:
                // 朝向速度方向
                if (currentVelocity.magnitude > 0.1f)
                {
                    targetRotationAngle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg - 90f;
                }
                break;
                
            case RotationMode.MouseDirection:
                // 朝向鼠标方向
                if (mouseDirection.magnitude > 0.1f)
                {
                    targetRotationAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg - 90f;
                }
                break;
                
            case RotationMode.Hybrid:
                // 混合模式：根据速度大小决定
                if (currentVelocity.magnitude > 0.1f)
                {
                    float speedRatio = currentSpeed / maxSpeed;
                    float velocityAngle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg - 90f;
                    float mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg - 90f;
                    
                    // 速度越快越倾向于速度方向，越慢越倾向于鼠标方向
                    targetRotationAngle = Mathf.LerpAngle(mouseAngle, velocityAngle, speedRatio);
                }
                break;
                
            case RotationMode.Custom:
                // 自定义旋转，不自动计算
                break;
        }
    }

    void HandleRotation()
    {
        if (!enableRotation) return;
        
        // 平滑旋转
        currentRotationAngle = Mathf.SmoothDampAngle(
            currentRotationAngle,
            targetRotationAngle,
            ref rotationVelocity,
            rotationSmoothTime,
            rotationSpeed
        );
        
        // 应用旋转
        transform.rotation = Quaternion.Euler(0f, 0f, currentRotationAngle);
        
        // 更新方向指示器
        UpdateDirectionIndicator();
    }

    void UpdateDirectionIndicator()
    {
        if (directionIndicator == null) return;
        
        // 计算指示器位置
        Vector2 indicatorPosition = (Vector2)transform.position + 
                                   (currentVelocity.normalized * indicatorOffset);
        
        directionIndicator.position = indicatorPosition;
        
        // 设置指示器旋转
        if (currentVelocity.magnitude > 0.1f)
        {
            float indicatorAngle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            directionIndicator.rotation = Quaternion.Euler(0f, 0f, indicatorAngle);
            
            // 根据速度调整指示器大小
            float indicatorScale = 0.5f + (currentSpeed / maxSpeed) * 0.5f;
            directionIndicator.localScale = new Vector3(indicatorScale, indicatorScale, 1f);
        }
    }

    void UpdateMovementState()
    {
        // 这里可以添加动画状态更新或其他视觉效果
        // 例如：根据速度大小调整粒子效果、声音等
    }

    // ===== 公共方法 =====
    
    // 立即停止移动
    public void StopImmediately()
    {
        currentVelocity = Vector2.zero;
        currentSpeed = 0f;
        rb.velocity = Vector2.zero;
        isMoving = false;
    }
    
    // 设置最大速度
    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxSpeed = Mathf.Max(0, newMaxSpeed);
    }
    
    // 获取当前移动方向
    public Vector2 GetCurrentDirection()
    {
        return currentVelocity.normalized;
    }
    
    // 获取当前速度值
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    // 检查是否正在移动
    public bool IsMoving()
    {
        return isMoving;
    }
    
    // 获取鼠标方向（归一化）
    public Vector2 GetMouseDirection()
    {
        return mouseDirection;
    }
    
    // 获取到鼠标的距离
    public float GetDistanceToMouse()
    {
        if (targetCamera == null) return 0f;
        
        Vector3 mouseWorldPosition = targetCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
                       -targetCamera.transform.position.z)
        );
        
        return Vector2.Distance(transform.position, mouseWorldPosition);
    }
    
    // ===== 旋转控制相关方法 =====
    
    // 设置旋转模式
    public void SetRotationMode(RotationMode mode)
    {
        rotationMode = mode;
    }
    
    // 设置旋转速度
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
    
    // 设置平滑时间
    public void SetRotationSmoothTime(float smoothTime)
    {
        rotationSmoothTime = Mathf.Max(0.01f, smoothTime);
    }
    
    // 强制设置目标旋转角度（用于自定义旋转）
    public void SetTargetRotation(float angle)
    {
        targetRotationAngle = angle;
    }
    
    // 立即旋转到指定角度
    public void RotateImmediately(float angle)
    {
        currentRotationAngle = angle;
        targetRotationAngle = angle;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    
    // 获取当前旋转角度
    public float GetCurrentRotation()
    {
        return currentRotationAngle;
    }
    
    // 获取目标旋转角度
    public float GetTargetRotation()
    {
        return targetRotationAngle;
    }
    
    // 启用/禁用旋转
    public void SetRotationEnabled(bool enabled)
    {
        enableRotation = enabled;
    }
    
    // 设置方向指示器
    public void SetDirectionIndicator(Transform indicator)
    {
        directionIndicator = indicator;
        if (directionIndicator != null)
        {
            directionIndicator.parent = null; // 确保指示器不会随物体旋转
        }
    }
}
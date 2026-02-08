
/****************************************************
    文件：PhysicsRocketController.cs
    作者：k0itoyuu
    日期：#CreateTime#
    功能：2D物理火箭控制器 - 重力环境下的鼠标加速控制
*****************************************************/
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsRocketController : MonoBehaviour
{
    [Header("物理设置")]
    [SerializeField, Range(0f, 50f)] 
    private float downwardAcceleration = 0f; // 默认向下的加速度（模拟重力）
    [SerializeField, Range(1f, 50f)] 
    private float mouseAccelerationForce = 20f; // 鼠标加速力大小
    [SerializeField, Range(0.1f, 30000f)] 
    private float maxSpeed = 15f;              // 最大速度限制
    [SerializeField, Range(0f, 1f)] 
    private float airResistance = 0.05f;       // 空气阻力系数
    
    [Header("鼠标控制")]
    [SerializeField] 
    private KeyCode accelerationKey = KeyCode.Mouse0; // 加速按键
    [SerializeField, Range(0.1f, 5f)] 
    private float minMouseDistance = 0.5f;     // 最小有效鼠标距离
    [SerializeField, Range(0f, 2f)] 
    private float accelerationDeadzone = 0.2f; // 加速死区范围
    
    [Header("视觉效果")]
    [SerializeField] 
    private TrailRenderer trailRenderer;      // 拖尾效果
    [SerializeField] 
    private ParticleSystem accelerationParticles; // 加速粒子效果
    [SerializeField] 
    private ParticleSystem gravityParticles;  // 重力影响粒子效果（可选）
    
    [Header("调试信息")]
    [SerializeField]
    private Vector2 currentVelocity;
    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private Vector2 totalAcceleration;
    [SerializeField]
    private bool isMouseAccelerating;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 mouseDirection;
    private float distanceToMouse;
    private float lastAccelerationTime;

    public AchivpopWnd achivpopWnd;
    protected SaveSvc save = null;
    void Start()
    {
        save = SaveSvc.Instance;
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        achivpopWnd.SetWndState();
        // 设置 Rigidbody2D 参数
        rb.gravityScale = 0f; // 禁用Unity物理重力，使用自定义重力
        rb.drag = 0f; // 禁用默认阻力，使用自定义空气阻力
        //EventCenter.Instance.AddEventListener<int>("EventSunNumChange",CheckUnLock);
        // 初始化视觉效果
        if (trailRenderer != null)
            trailRenderer.emitting = true;
            
        if (gravityParticles != null && gravityParticles.isPlaying)
            gravityParticles.Stop();
    }
    
    void Update()
    {
        HandleMouseInput();
        UpdateDebugInfo();
        // if (rb.velocity.magnitude > 10&&SaveSvc.Instance.Building.achiv1==false)//事件监听
        // {
        //     Debug.Log(rb.velocity.magnitude);
        //     save.Building.achiv1 = true;
        //     EventCenter.Instance.EventTrigger("MoveAchiv");
        // }
    }
    void FixedUpdate()
    {
        ApplyPhysicsForces();
        ClampMaxSpeed();
    }

    /// <summary>
    /// 处理鼠标输入
    /// </summary>
    private void HandleMouseInput()
    {
        // 获取鼠标在世界空间的位置
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // 计算鼠标相关信息
        mouseDirection = (mouseWorldPos - (Vector2)transform.position).normalized;
        distanceToMouse = Vector2.Distance(transform.position, mouseWorldPos);
        
        // 检测按键状态
        isMouseAccelerating = Input.GetKey(accelerationKey) && distanceToMouse > minMouseDistance;
        
        // 记录最后加速时间
        if (isMouseAccelerating)
            lastAccelerationTime = Time.time;
    }

    /// <summary>
    /// 应用物理力
    /// </summary>
    private void ApplyPhysicsForces()
    {
        // 1. 始终应用向下的加速度（模拟重力）
        Vector2 downwardForce = Vector2.down * downwardAcceleration * rb.mass;
        rb.AddForce(downwardForce, ForceMode2D.Force);
        
        // 2. 如果鼠标正在加速，应用鼠标方向的力
        if (isMouseAccelerating)
        {
            // 根据距离调整力的大小（非线性响应）
            float distanceFactor = CalculateDistanceFactor(distanceToMouse);
            Vector2 mouseForce = mouseDirection * mouseAccelerationForce * rb.mass * distanceFactor;
            
            // 应用鼠标力
            rb.AddForce(mouseForce, ForceMode2D.Force);
            
            // 计算总加速度用于调试
            totalAcceleration = (downwardForce + mouseForce) / rb.mass;
        }
        else
        {
            totalAcceleration = downwardForce / rb.mass;
        }
    }

    /// <summary>
    /// 计算距离因子（非线性响应曲线）
    /// </summary>
    private float CalculateDistanceFactor(float distance)
    {
        // 使用平滑曲线：近距离时响应较弱，远距离时响应较强
        if (distance < accelerationDeadzone)
            return 0f;
            
        float normalizedDistance = Mathf.Clamp01((distance - accelerationDeadzone) / 10f);
        return Mathf.Pow(normalizedDistance, 0.7f);
    }

   

    /// <summary>
    /// 限制最大速度
    /// </summary>
    private void ClampMaxSpeed()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
    /// <summary>
    /// 更新调试信息
    /// </summary>
    private void UpdateDebugInfo()
    {
        currentVelocity = rb.velocity;
        currentSpeed = rb.velocity.magnitude;
    }

    /// <summary>
    /// 在 Scene 视图中绘制调试信息
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || rb == null) return;
        
        // 绘制速度向量（绿色）
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)rb.velocity);
        
        // 绘制总加速度向量（红色）
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)totalAcceleration);
        
        // 绘制向下的加速度向量（黄色）
        Gizmos.color = Color.yellow;
        Vector2 downwardAccelVector = Vector2.down * downwardAcceleration;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)downwardAccelVector);
        
        // 绘制最大速度范围
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, maxSpeed * 0.3f);
        
        // 绘制鼠标方向
        if (isMouseAccelerating && mainCamera != null)
        {
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, mouseWorldPos);
            
            // 绘制最小距离范围
            Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
            Gizmos.DrawWireSphere(transform.position, minMouseDistance);
        }
    }

    /// <summary>
    /// 外部方法：设置向下的加速度（重力）
    /// </summary>
    public void SetDownwardAcceleration(float newAcceleration)
    {
        downwardAcceleration = Mathf.Max(0f, newAcceleration);
    }

    /// <summary>
    /// 外部方法：设置鼠标加速度
    /// </summary>
    public void SetMouseAcceleration(float newAcceleration)
    {
        mouseAccelerationForce = Mathf.Max(0f, newAcceleration);
    }

    /// <summary>
    /// 外部方法：设置最大速度
    /// </summary>
    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxSpeed = Mathf.Max(0.1f, newMaxSpeed);
    }

    /// <summary>
    /// 获取当前速度百分比
    /// </summary>
    public float GetSpeedPercentage()
    {
        return Mathf.Clamp01(currentSpeed / maxSpeed);
    }

    /// <summary>
    /// 获取总加速度向量
    /// </summary>
    public Vector2 GetTotalAcceleration()
    {
        return totalAcceleration;
    }

    /// <summary>
    /// 获取重力与鼠标力的比例
    /// </summary>
    public float GetGravityToMouseForceRatio()
    {
        float gravityMagnitude = downwardAcceleration;
        float mouseForceMagnitude = isMouseAccelerating ? mouseAccelerationForce * CalculateDistanceFactor(distanceToMouse) : 0f;
        
        if (mouseForceMagnitude == 0) return float.PositiveInfinity;
        return gravityMagnitude / mouseForceMagnitude;
    }
}
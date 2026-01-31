/****************************************************
    文件：RocketGravityController.cs
    作者：k0itoyuu
    日期：#CreateTime#
    功能：火箭引力计算器 - 计算多个星球对火箭的引力并矢量合并
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RocketGravityController : MonoBehaviour
{
    [Header("星球设置")]
    [SerializeField] 
    private List<GameObject> planets = new List<GameObject>(); // 星球对象列表
    
    [Header("引力常数")]
    [SerializeField, Range(0.1f, 100f)] 
    private float gravitationalConstant = 90.81f; // 引力常数G
    
    [Header("物理设置")]
    [SerializeField, Range(0f, 100f)] 
    private float rocketMass = 1f; // 火箭质量
    [SerializeField, Range(0.1f, 10f)] 
    private float minDistance = 1f; // 最小距离，避免距离过小时引力过大
    [SerializeField, Range(0f, 1f)] 
    private float timeScale = 1f; // 引力计算时间缩放
    
    [Header("调试显示")]
    [SerializeField] 
    private bool showDebugLines = true; // 是否显示调试线
    [SerializeField] 
    private bool showForceVectors = true; // 是否显示力向量
    [SerializeField] 
    private bool showResultantForce = true; // 是否显示合力
    
    // 调试信息
    private Vector2 resultantForce = Vector2.zero;
    private List<Vector2> individualForces = new List<Vector2>();
    private List<float> individualForceMagnitudes = new List<float>();
    private float maxForceMagnitude = 0f;
    
    // 组件引用
    private Rigidbody2D rb;
    private LineRenderer forceLineRenderer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
  
        // 创建或获取LineRenderer用于调试显示
        SetupLineRenderer();
        
        // 如果没有通过Inspector设置星球，尝试通过标签查找
        if (planets.Count == 0)
        {
            FindPlanetsByTag();
        }
        
        // 初始化力列表
        InitializeForceLists();
    }
    
    void Update()
    {
        // 更新调试显示
        if (showDebugLines)
            UpdateDebugDisplay();
    }
    
    void FixedUpdate()
    {
        // 计算并应用引力
        Vector2 totalGravityForce = CalculateTotalGravityForce();
        
        // 应用力到刚体
        if (timeScale > 0)
        {
            rb.AddForce(totalGravityForce * timeScale, ForceMode2D.Force);
        }
        
        // 更新调试信息
        UpdateDebugInfo(totalGravityForce);
    }
    
    /// <summary>
    /// 设置LineRenderer组件
    /// </summary>
    private void SetupLineRenderer()
    {
        forceLineRenderer = GetComponent<LineRenderer>();
        if (forceLineRenderer == null)
        {
            forceLineRenderer = gameObject.AddComponent<LineRenderer>();
            forceLineRenderer.startWidth = 0.05f;
            forceLineRenderer.endWidth = 0.05f;
            forceLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            forceLineRenderer.startColor = Color.cyan;
            forceLineRenderer.endColor = Color.blue;
            forceLineRenderer.positionCount = 2;
        }
        forceLineRenderer.enabled = showDebugLines && showResultantForce;
    }
    
    /// <summary>
    /// 通过标签查找星球
    /// </summary>
    private void FindPlanetsByTag()
    {
        GameObject[] foundPlanets = GameObject.FindGameObjectsWithTag("Planet");
        planets.AddRange(foundPlanets);
        
        // 如果没找到，尝试其他常见标签
        if (planets.Count == 0)
        {
            foundPlanets = GameObject.FindGameObjectsWithTag("CelestialBody");
            planets.AddRange(foundPlanets);
        }
        
        Debug.Log($"通过标签找到 {planets.Count} 个星球");
    }
    
    /// <summary>
    /// 初始化力列表
    /// </summary>
    private void InitializeForceLists()
    {
        individualForces.Clear();
        individualForceMagnitudes.Clear();
        
        for (int i = 0; i < planets.Count; i++)
        {
            individualForces.Add(Vector2.zero);
            individualForceMagnitudes.Add(0f);
        }
    }
    
    /// <summary>
    /// 计算并返回所有星球对火箭的总引力（矢量合并）
    /// </summary>
    public Vector2 CalculateTotalGravityForce()
    {
        if (planets.Count == 0)
            return Vector2.zero;
        
        Vector2 totalForce = Vector2.zero;
        maxForceMagnitude = 0f;
        
        for (int i = 0; i < planets.Count; i++)
        {
            GameObject planet = planets[i];
            
            if (planet == null)
                continue;
                
            // 获取星球的质量
            float planetMass = GetPlanetMass(planet);
            if (planetMass <= 0)
                continue;
            
            // 计算火箭到星球的方向和距离
            Vector2 direction = planet.transform.position - transform.position;
            float distance = direction.magnitude;
            
            // 避免除零和距离过小时力过大
            if (distance < minDistance)
                distance = minDistance;
            
            // 计算引力大小 F = G * (m1 * m2) / r^2
            float forceMagnitude = gravitationalConstant * (rocketMass * planetMass) / (distance * distance);
            
            // 计算引力方向（从火箭指向星球）
            Vector2 forceDirection = direction.normalized;
            
            // 计算力向量
            Vector2 forceVector = forceDirection * forceMagnitude;
            
            // 累加到总力
            totalForce += forceVector;
            
            // 存储单个力用于调试
            if (i < individualForces.Count)
            {
                individualForces[i] = forceVector;
                individualForceMagnitudes[i] = forceMagnitude;
                
                if (forceMagnitude > maxForceMagnitude)
                    maxForceMagnitude = forceMagnitude;
            }
        }
        
        resultantForce = totalForce;
        return totalForce;
    }
    
    /// <summary>
    /// 获取星球的质量
    /// </summary>
    private float GetPlanetMass(GameObject planet)
    {
        // 尝试从PlanetMass组件获取质量
        PlanetMass planetMassComponent = planet.GetComponent<PlanetMass>();
        if (planetMassComponent != null)
            return planetMassComponent.mass;
        
        // 尝试从Rigidbody2D获取质量
        Rigidbody2D planetRb = planet.GetComponent<Rigidbody2D>();
        if (planetRb != null)
            return planetRb.mass;
        
        // 返回默认质量
        return 1000f;
    }
    
    /// <summary>
    /// 更新调试信息
    /// </summary>
    private void UpdateDebugInfo(Vector2 totalForce)
    {
        // 这里可以添加其他调试信息更新逻辑
    }
    
    /// <summary>
    /// 更新调试显示
    /// </summary>
    private void UpdateDebugDisplay()
    {
        // 显示合力向量
        if (showResultantForce && forceLineRenderer != null)
        {
            forceLineRenderer.enabled = true;
            
            // 设置LineRenderer位置
            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + (Vector3)resultantForce.normalized * Mathf.Log(1 + resultantForce.magnitude);
            
            forceLineRenderer.SetPosition(0, startPos);
            forceLineRenderer.SetPosition(1, endPos);
            
            // 根据力的大小调整颜色
            float forceRatio = Mathf.Clamp01(resultantForce.magnitude / (maxForceMagnitude > 0 ? maxForceMagnitude : 1f));
            Color lineColor = Color.Lerp(Color.cyan, Color.red, forceRatio);
            forceLineRenderer.startColor = lineColor;
            forceLineRenderer.endColor = lineColor;
        }
        else if (forceLineRenderer != null)
        {
            forceLineRenderer.enabled = false;
        }
    }
    
    /// <summary>
    /// 在Scene视图中绘制调试信息
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;
        
        if (!showDebugLines)
            return;
        
        // 绘制每个星球的引力向量
        if (showForceVectors)
        {
            for (int i = 0; i < planets.Count; i++)
            {
                if (planets[i] == null || i >= individualForces.Count)
                    continue;
                    
                Vector2 force = individualForces[i];
                if (force.magnitude > 0.01f)
                {
                    // 使用不同颜色表示不同大小的力
                    float forceRatio = Mathf.Clamp01(individualForceMagnitudes[i] / (maxForceMagnitude > 0 ? maxForceMagnitude : 1f));
                    Gizmos.color = Color.Lerp(Color.green, Color.red, forceRatio);
                    
                    Vector3 startPos = transform.position;
                    Vector3 endPos = transform.position + (Vector3)force.normalized * Mathf.Log(1 + force.magnitude);
                    
                    Gizmos.DrawLine(startPos, endPos);
                    
                    // 绘制箭头
                    DrawArrow(startPos, endPos, 0.2f);
                }
            }
        }
        
        // 绘制合力向量
        if (showResultantForce && resultantForce.magnitude > 0.01f)
        {
            Gizmos.color = Color.cyan;
            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + (Vector3)resultantForce.normalized * Mathf.Log(1 + resultantForce.magnitude) * 1.5f;
            
            Gizmos.DrawLine(startPos, endPos);
            
            // 绘制更醒目的箭头
            DrawArrow(startPos, endPos, 0.3f);
            
            // 绘制力的大小文本
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(endPos, $"合力: {resultantForce.magnitude:F2}");
            #endif
        }
    }
    
    /// <summary>
    /// 绘制箭头
    /// </summary>
    private void DrawArrow(Vector3 start, Vector3 end, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawLine(start, end);
        
        Vector3 direction = (end - start).normalized;
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        
        Gizmos.DrawLine(end, end + right * arrowHeadLength);
        Gizmos.DrawLine(end, end + left * arrowHeadLength);
    }
    
    /// <summary>
    /// 添加星球到列表
    /// </summary>
    public void AddPlanet(GameObject planet)
    {
        if (planet != null && !planets.Contains(planet))
        {
            planets.Add(planet);
            individualForces.Add(Vector2.zero);
            individualForceMagnitudes.Add(0f);
        }
    }
    
    /// <summary>
    /// 从列表中移除星球
    /// </summary>
    public void RemovePlanet(GameObject planet)
    {
        if (planets.Contains(planet))
        {
            int index = planets.IndexOf(planet);
            planets.RemoveAt(index);
            
            if (index < individualForces.Count)
                individualForces.RemoveAt(index);
                
            if (index < individualForceMagnitudes.Count)
                individualForceMagnitudes.RemoveAt(index);
        }
    }
    
    /// <summary>
    /// 清除所有星球
    /// </summary>
    public void ClearAllPlanets()
    {
        planets.Clear();
        individualForces.Clear();
        individualForceMagnitudes.Clear();
    }
    
    /// <summary>
    /// 获取当前合力向量
    /// </summary>
    public Vector2 GetResultantForce()
    {
        return resultantForce;
    }
    
    /// <summary>
    /// 获取合力的方向
    /// </summary>
    public Vector2 GetResultantForceDirection()
    {
        return resultantForce.normalized;
    }
    
    /// <summary>
    /// 获取合力的大小
    /// </summary>
    public float GetResultantForceMagnitude()
    {
        return resultantForce.magnitude;
    }
    
    /// <summary>
    /// 获取引力最强的星球
    /// </summary>
    public GameObject GetStrongestGravityPlanet()
    {
        if (planets.Count == 0 || individualForceMagnitudes.Count == 0)
            return null;
            
        int maxIndex = 0;
        float maxForce = 0;
        
        for (int i = 0; i < individualForceMagnitudes.Count; i++)
        {
            if (individualForceMagnitudes[i] > maxForce && i < planets.Count && planets[i] != null)
            {
                maxForce = individualForceMagnitudes[i];
                maxIndex = i;
            }
        }
        
        return planets[maxIndex];
    }
    
    /// <summary>
    /// 设置引力常数
    /// </summary>
    public void SetGravitationalConstant(float newConstant)
    {
        gravitationalConstant = Mathf.Max(0.01f, newConstant);
    }
    
    /// <summary>
    /// 设置火箭质量
    /// </summary>
    public void SetRocketMass(float newMass)
    {
        rocketMass = Mathf.Max(0.01f, newMass);
    }
}
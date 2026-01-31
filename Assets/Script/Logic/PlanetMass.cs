/****************************************************
    文件：PlanetMass.cs
    作者：k0itoyuu
    日期：#CreateTime#
    功能：Nothing
*****************************************************/

using UnityEngine;

public class PlanetMass : MonoBehaviour
{
    [Header("星球属性")]
    [SerializeField, Range(0.1f, 10000f)] 
    public float mass = 1000f; // 星球质量
    
    [Header("可视化")]
    [SerializeField] 
    private Color gizmoColor = Color.yellow; // Gizmo颜色
    
    [Header("自动设置")]
    [SerializeField] 
    private bool autoSetMassFromRigidbody = true; // 是否从Rigidbody自动获取质量
    
    void Start()
    {
        if (autoSetMassFromRigidbody)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                mass = rb.mass;
            }
        }
    }
    
    /// <summary>
    /// 设置星球质量
    /// </summary>
    public void SetMass(float newMass)
    {
        mass = Mathf.Max(0.1f, newMass);
        
        // 同时更新Rigidbody2D的质量（如果存在）
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.mass = mass;
        }
    }
    
    /// <summary>
    /// 在Scene视图中绘制调试信息
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.6f, $"质量: {mass:F0}");
#endif
    }
}

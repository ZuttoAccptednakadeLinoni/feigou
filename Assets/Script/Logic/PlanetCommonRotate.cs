using UnityEngine;

public class UltraSimpleOrbit : MonoBehaviour
{
    public Transform centerObject;
    public float orbitRadius = 5f;
    public float orbitSpeed = 30f;
    public bool enableSelfRotation = true;
    public float selfRotationSpeed = 50f;
    
    private float currentAngle;
    
    void Start()
    {
        if (centerObject == null)
        {
            Debug.LogError("请设置公转中心物体！", this);
            enabled = false;
            return;
        }
        
        // 初始位置
        currentAngle = 0f;
        UpdateOrbitPosition();
    }
    
    void Update()
    {
        if (centerObject == null) return;
        
        // 公转
        currentAngle += orbitSpeed * Time.deltaTime * Mathf.Deg2Rad;
        UpdateOrbitPosition();
        
        // 自转
        if (enableSelfRotation)
            transform.Rotate(0, selfRotationSpeed * Time.deltaTime, 0, Space.Self);
    }
    
    void UpdateOrbitPosition()
    {
        // 简单的圆形轨道公式
        float x = Mathf.Cos(currentAngle) * orbitRadius;
        float z = Mathf.Sin(currentAngle) * orbitRadius;
        transform.position = centerObject.position + new Vector3(x, 0, z);
    }
    
    public void SetOrbitSpeed(float speed) => orbitSpeed = speed;
    public void SetSelfRotation(bool enabled) => enableSelfRotation = enabled;
}
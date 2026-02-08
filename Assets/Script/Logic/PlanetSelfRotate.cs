using UnityEngine;

public class RotateOnYAxis : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("旋转速度（度/秒）")]
    [SerializeField] private float rotationSpeed = 90f; // 默认90度/秒
    
    [Tooltip("是否启用旋转")]
    [SerializeField] private bool enableRotation = true;
    
    [Tooltip("旋转方向")]
    [SerializeField] private RotationDirection direction = RotationDirection.Clockwise;
    
    // 旋转方向枚举
    public enum RotationDirection
    {
        Clockwise,      // 顺时针
        CounterClockwise // 逆时针
    }

    void Update()
    {
        // 如果旋转未启用，直接返回
        if (!enableRotation) return;
        
        // 计算当前帧的旋转角度（考虑方向）
        float rotationAmount = rotationSpeed * Time.deltaTime;
        
        // 根据方向调整旋转值
        if (direction == RotationDirection.Clockwise)
        {
            rotationAmount = -rotationAmount; // 顺时针为负方向
        }
        
        // 绕Y轴旋转
        transform.Rotate(0f, rotationAmount, 0f, Space.World);
        
        // 或者使用局部坐标系（二选一）：
        // transform.Rotate(0f, rotationAmount, 0f, Space.Self);
    }
    
    // 公共方法，用于在运行时调整旋转速度
    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }
    
    // 公共方法，用于在运行时切换旋转状态
    public void ToggleRotation(bool isEnabled)
    {
        enableRotation = isEnabled;
    }
    
    // 公共方法，用于在运行时切换旋转方向
    public void SetRotationDirection(RotationDirection newDirection)
    {
        direction = newDirection;
    }
    
    // 快捷方法：切换顺时针/逆时针
    public void ToggleDirection()
    {
        direction = (direction == RotationDirection.Clockwise) ? 
            RotationDirection.CounterClockwise : 
            RotationDirection.Clockwise;
    }
}
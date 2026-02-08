using UnityEngine;

/// <summary>
/// 全局时间配置：tick 的长度
/// </summary>
public static class GlobalConfig
{
    private const double EPS = 1e-6;
    // 每个 tick 对应的真实时间（秒）
    public const float TickDuration = 1f / 120f;
}

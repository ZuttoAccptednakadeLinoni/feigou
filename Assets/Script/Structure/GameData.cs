using System;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class GlobalGameData {
    public int TechLevel = 0;//总科技等级
    public int PeriodCount = 0;//运行周期数
    public float GameProcess = 0f;//游戏进度
}
/// <summary>
/// 运行时的星球数据（包含 GameObject）
/// 不直接用于存档
/// </summary>
[Serializable]
public class PlanetData
{
    public string PlanetId;                 // 星球唯一ID

    public GameObject PlanetObject;         // 场景中的星球对象

    public int SourceAmount = 0;                 // 资源量
    public float Radius;
    [HideInInspector] public float StartAngle;
    [HideInInspector] public float RevolveSpeed;
    public float selfRotateSpeed;
    [HideInInspector] public int PeriodTicks;

    public List<SatelliteData> Satellites;   // 该星球的卫星/空间站
    public List<FacilityData> Facilities;    // 该星球的设施

    public float PlanetFOV = 31f;
    public GameObject InspectPoint;
}

/// <summary>
/// 运行时的卫星数据
/// </summary>
[Serializable]
public class SatelliteData
{
    public string SatelliteId;              // 卫星唯一ID
    public GameObject SatelliteObject;      // 场景中的卫星对象
    public float Radius;
    public float StartAngle;
    public float RevolveSpeed;

    public float selfRotateSpeed;
    public int PeriodTicks;

    public bool IsBuilt = false;               // 是否建造
}

/// <summary>
/// 运行时的设施数据
/// </summary>
[Serializable]
public class FacilityData
{
    public string FacilityType;             // 类型标识
    public GameObject FacilityObject;      // 场景中的设施对象
    public bool IsBuilt = false;               // 是否建造
}

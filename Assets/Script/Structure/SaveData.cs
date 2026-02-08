using System;
using System.Collections.Generic;



/// <summary>
/// 整个游戏的存档数据（纯数据，不含任何 Unity 对象）
/// </summary>
[Serializable]
public class SaveData
{
    public GameStatus Status;              // 游戏状态（如资源、科技等）
    public long GlobalTick;                 // 全局 tick
    public List<PlanetSaveData> Planets;    // 星球数据
    public List<RouteSaveData> Routes;      // 航线数据
}


[Serializable]
public class GameStatus
{
    public int TechLevel = 0;
    public int PeriodCount = 0;
    public float GameProcess = 0f;
}
[Serializable]
public class RouteSaveData
{
    public string RouteId;
    public List<string> PlanetIds; // 路线经过的星球ID列表
}
/// <summary>
/// 单个星球的存档数据
/// </summary>
[Serializable]
public class PlanetSaveData
{
    public string PlanetId;                 // 星球唯一ID
    public int PeriodTicks;

    public int SourceAmount;

    public List<SatelliteSaveData> Satellites;   // 卫星 / 空间站
    public List<FacilitySaveData> Facilities;    // 星球上的设施
}

/// <summary>
/// 卫星 / 空间站的存档数据
/// </summary>
[Serializable]
public class SatelliteSaveData
{
    public string SatelliteId;// 类型
    public float RevolveSpeed;
    public int PeriodTicks;
    public bool IsBuilt; // 是否建造
}

/// <summary>
/// 设施（发射站/工业站）存档数据
/// </summary>
[Serializable]
public class FacilitySaveData
{
    public string FacilityType;
    public bool IsBuilt;
}

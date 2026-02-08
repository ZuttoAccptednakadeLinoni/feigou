using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SolarController : MonoBehaviour
{
    [SerializeField] private GlobalGameData globalGameData;
    [SerializeField] private List<PlanetData> planets = new List<PlanetData>();
    private long tickCount = 0; // 计数器 
    private float tickTimeAcc = 0f; // 累积时间
    private void Awake(){
        for(int i=0;i<planets.Count;i++){
            planets[i].PlanetId = $"Planet{i}";
        }
        //LoadFromDisk();
        float[] FOV = {31f,31f,31f,31f,31f,31f};//详细界面视野

    	int[] speedCount = { 15, 20, 30, 40, 60, 120 }; // 运转周期 秒/圈 
        
        for (int i = 0; i < planets.Count; i++){
            var p = planets[i];

            float periodSeconds = speedCount[i];//指定速度
            p.RevolveSpeed = 360f / periodSeconds;
            p.PeriodTicks = Mathf.Max(1, Mathf.RoundToInt(periodSeconds / GlobalConfig.TickDuration));

            if (p.Satellites != null){
                foreach (var s in p.Satellites){
                    s.SatelliteId = $"{p.PlanetId}_Satellite{p.Satellites.IndexOf(s)}";
                    // 卫星周期定义 这里为一半
                    float satPeriodSeconds = periodSeconds * 0.5f;
                    s.RevolveSpeed = 360f / satPeriodSeconds;
                    s.PeriodTicks = Mathf.Max(1, Mathf.RoundToInt(satPeriodSeconds / GlobalConfig.TickDuration));

                    if(s.IsBuilt == false){
                        s.SatelliteObject.SetActive(false);
                    }
                    else{
                        s.SatelliteObject.SetActive(true);
                    }
                }
            }
            if(p.Facilities != null){
                foreach (var f in p.Facilities){
                    if(f.IsBuilt == false){
                        f.FacilityObject.SetActive(false);
                    }
                    else{
                        f.FacilityObject.SetActive(true);
                    }
                }
            }
            planets[i] = p;
        }
    }
    // 计算一个物体在圆轨道上的位置
    private void UpdateOrbit(GameObject obj, float radius, float startAngleDeg, int periodTicks, float selfRotateSpeed){
        long phase = tickCount % periodTicks;
        float t = (float)phase / periodTicks;
        float angleRad = t * Mathf.PI * 2f + startAngleDeg * Mathf.Deg2Rad;

        obj.transform.localPosition = new Vector3(
            Mathf.Cos(angleRad) * radius,
            0f,
            Mathf.Sin(angleRad) * radius
        );
        if(selfRotateSpeed < 0f) {
            
        }
        else if (selfRotateSpeed > 0f){
            obj.GetComponent<PlanetController>().RotateTarget();
            //obj.transform.Rotate(Vector3.up, selfRotateSpeed * Time.deltaTime);
        }
        else{//没有自转即为潮汐锁定
            obj.GetComponent<PlanetController>().LockTarget();
            //obj.transform.rotation = Quaternion.Euler(0f, -angleRad * Mathf.Rad2Deg + 90f, 0f);
        }
    }
    private void UpdatePlanets(){
        foreach (var p in planets){
            UpdateOrbit(p.PlanetObject, p.Radius, p.StartAngle, p.PeriodTicks,p.selfRotateSpeed);//更新所有行星
            if (p.Satellites != null){
                foreach (var s in p.Satellites){
                    if (s.SatelliteObject != null && s.PeriodTicks > 0){
                        if(s.IsBuilt == false){
                            s.SatelliteObject.SetActive(false);
                        }
                        else{
                            s.SatelliteObject.SetActive(true);
                        }
                        UpdateOrbit(s.SatelliteObject, s.Radius, s.StartAngle, s.PeriodTicks,s.selfRotateSpeed);//更新所有卫星
                    }
                }
            }
            if(p.Facilities != null){
                foreach (var f in p.Facilities){
                    if(f.IsBuilt == false){
                        f.FacilityObject.SetActive(false);
                    }
                    else{
                        f.FacilityObject.SetActive(true);
                    }
                }
            }
        }
    }

    private void Update(){
        /*if (Input.GetKeyDown(KeyCode.Z)){//按下z键
            SaveToDisk();
        }*/
    //用 deltaTime 推进整数 tick
        tickTimeAcc += Time.deltaTime;

        while (tickTimeAcc >= GlobalConfig.TickDuration){
            tickTimeAcc -= GlobalConfig.TickDuration;
            tickCount++;
        }
        //更新所有行星的位置
        UpdatePlanets();
    }
    public SaveData BuildSaveData(){//保存存档
        var save = new SaveData();
        save.GlobalTick = tickCount;
        save.Planets = new List<PlanetSaveData>();

        foreach (var p in planets){
            var ps = new PlanetSaveData();
            ps.PlanetId = p.PlanetId;
            ps.PeriodTicks = p.PeriodTicks;

            // 卫星
            ps.Satellites = new List<SatelliteSaveData>();
            if (p.Satellites != null){
                foreach (var s in p.Satellites){
                    ps.Satellites.Add(new SatelliteSaveData{
                        SatelliteId = s.SatelliteId,
                        RevolveSpeed = s.RevolveSpeed,
                        PeriodTicks = s.PeriodTicks,
                        IsBuilt = s.IsBuilt
                    });
                }
            }

            // 设施
            ps.Facilities = new List<FacilitySaveData>();
            if (p.Facilities != null){
                foreach (var f in p.Facilities){
                    ps.Facilities.Add(new FacilitySaveData{
                        FacilityType = f.FacilityType,
                        IsBuilt = f.IsBuilt
                    });
                }
            }
            save.Planets.Add(ps);
        }

        return save;
    }
    public void ApplySaveData(SaveData save){//读取存档
        tickCount = save.GlobalTick;

        foreach (var ps in save.Planets){
            var p = planets.Find(x => x.PlanetId == ps.PlanetId);
            if (p == null) continue;
            
            p.PeriodTicks = ps.PeriodTicks;

             // 卫星
            // 卫星
            if (ps.Satellites != null && p.Satellites != null){
                foreach (var ss in ps.Satellites){
                    var s = p.Satellites.Find(x => x.SatelliteId == ss.SatelliteId);
                    if (s == null) continue;

                    s.PeriodTicks = ss.PeriodTicks;
                }
            }

            // 设施
            if (ps.Facilities != null && p.Facilities != null){
                foreach (var fs in ps.Facilities){
                    var f = p.Facilities.Find(x => x.FacilityType == fs.FacilityType);
                    if (f == null) continue;
                    f.IsBuilt = fs.IsBuilt;
                }
            }
        }
    }
    public void SaveToDisk(){//保存
        var data = BuildSaveData();
        SaveManager.Save(data);
    }

    public void LoadFromDisk(){//加载
        var data = SaveManager.Load();
        ApplySaveData(data);
    }

}

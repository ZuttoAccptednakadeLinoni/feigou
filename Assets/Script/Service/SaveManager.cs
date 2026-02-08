using System.IO;
using UnityEngine;

/// <summary>
/// 负责把 SaveData 存到硬盘 / 从硬盘读出来
/// </summary>
public static class SaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"存档已保存到: {SavePath}");
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("存档不存在，返回空存档");
            return new SaveData { Planets = new System.Collections.Generic.List<PlanetSaveData>() };
        }

        string json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"存档已加载: {SavePath}");
        return data;
    }
}

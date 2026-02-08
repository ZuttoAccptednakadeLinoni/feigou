/****************************************************
    文件：SaveSvc.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：存档功能
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;




[Serializable]  // 可序列化对象
public class BaseBuilding
{ 
	public string BID;  // 三个属性均为可序列化属性，所以可以直接使用方法进行序列化。
	public bool achiv1;
	public bool achiv2;
}
	public class SaveSvc:MonoBehaviour
	{
		public static SaveSvc Instance;
		public BaseBuilding Building;
		private string customSavePath;
		
		public void InitSvc()
		{
			Instance = this;
			Debug.Log("InitSvc");
			customSavePath = Application.persistentDataPath + "/MyGameSaves/";
			if (!System.IO.Directory.Exists(customSavePath))
			{
				Debug.Log(customSavePath);
				System.IO.Directory.CreateDirectory(customSavePath);
				SaveData();
			}
		}
    
		public void SaveData()
		{
			string filePath = customSavePath + "gameSave.es3";
			ES3.Save("BuildingData", Building, filePath);
		}
		// 读取存档数据的方法
		public bool LoadData()
		{
			string filePath = customSavePath + "gameSave.es3";
			// 从文件加载数据
			Building = ES3.Load<BaseBuilding>("BuildingData", filePath);
			Debug.Log("数据加载成功: " + filePath);
			return true;
			
		}
	}

/****************************************************
    文件：PlantAsset.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：行星集合
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Plant/PlantAsset")]
public class PlantAsset : ScriptableObject
{
    [SerializeField]
    private Object[] Plant;
}

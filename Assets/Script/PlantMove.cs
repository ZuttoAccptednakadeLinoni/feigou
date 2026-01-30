/****************************************************
    文件：PlantMove.cs
	作者：k0itoyuu
    日期：#CreateTime#
	功能：控制星球移动
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantMove : MonoBehaviour
{
    public Transform center;
    private float radius;
    public float speed = 90f;
    public bool clockwise = true;
    
    private float angle = 0f;

    private void Awake()
    {
        Vector3 direction = center.transform.position - this.transform.position;
        radius = direction.magnitude; 
    }

    void Update()
    {
        if (center == null) return;
        
        // 更新角度
        angle += (clockwise ? -1 : 1) * speed * Time.deltaTime * Mathf.Deg2Rad;
        
        // 计算新位置
        float x = center.position.x + Mathf.Cos(angle) * radius;
        float y = center.position.y + Mathf.Sin(angle) * radius;
        
        // 更新位置
        transform.position = new Vector3(x, y, transform.position.z);
    }
}

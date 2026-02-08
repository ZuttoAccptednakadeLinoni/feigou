using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [SerializeField] public GameObject selfrotateTarget;
    [SerializeField] public GameObject cameraTarget;
    public void LockTarget() {
        Vector3 dir = (Vector3.zero - selfrotateTarget.transform.position).normalized;
        selfrotateTarget.transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);   
    }
    public void RotateTarget(){
        selfrotateTarget.transform.Rotate(Vector3.up, 10f * Time.deltaTime);
    }
}

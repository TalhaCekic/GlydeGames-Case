using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardFX : MonoBehaviour
{
    public Transform camTransform;

    Quaternion originalRotation;

    void Start()
    {
        originalRotation = Quaternion.Euler(-transform.rotation.x, -transform.rotation.y, -transform.rotation.z);
    }

    void Update()
    {
        //transform.rotation = camTransform.rotation * originalRotation;   
        transform.rotation = Camera.main.transform.rotation * originalRotation;   
    }
}
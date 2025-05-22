using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;

public class Door : NetworkBehaviour
{
    [SyncVar] public bool isOpened;
    [SyncVar] public int direction;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void DoorSystem()
    {
        // isOpened = !isOpened;
        // if (isOpened)
        // {
        //     Vector3 OpenRot = new Vector3(0, 60, 0)*direction;
        //     this.transform.DORotate(OpenRot,0.5f);
        // }
        // else
        // {
        //     Vector3 CloseRot = new Vector3(0, -20,0)*direction;
        //     this.transform.DORotate(CloseRot,0.5f);
        // }
        
        isOpened = !isOpened;
        if (isOpened)
        {
            Vector3 OpenRot = new Vector3(0, 200, 0)*direction;
            this.transform.DORotate(OpenRot,0.5f);
        }
        else
        {
            Vector3 CloseRot = new Vector3(0, 132,0)*direction;
            this.transform.DORotate(CloseRot,0.5f);
        }
    }
}

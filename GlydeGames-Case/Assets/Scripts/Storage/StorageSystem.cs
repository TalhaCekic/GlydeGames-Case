using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class StorageSystem : NetworkBehaviour
{
    public GameObject obj;
    [SyncVar] public float interactDelay;
    [SyncVar] public bool isInteracting;

    private void Update()
    {
        //if(!isServer)return;
        ResInteract();
    }

    [Server]
    private void ResInteract()
    {
        if (isInteracting)
        {
            if (interactDelay > 0.5f)
            {
                interactDelay = 0;
                isInteracting = false;
            }
            else
            {
                interactDelay += Time.deltaTime;
            }
        }
    }

    public void TransportPutItem(GameObject playerObject)
    {
       // playerObject.transform.SetParent(transform);
        playerObject.GetComponent<PlayerInteract>().HandRes();
        isInteracting = true;
    }
    public void TransportTakeItem(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerInteract>().HandTakeAll();
        playerObject.GetComponent<PlayerInteract>().InteractObj = obj;
        obj = null;
    }
}

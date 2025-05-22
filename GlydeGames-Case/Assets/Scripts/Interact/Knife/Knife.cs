using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;

public class Knife : NetworkBehaviour
{
    [SyncVar] public bool isStateFree;
    [SyncVar] public bool isInteract;

    public Rigidbody rb;
    public Collider cl;


    [Header("El Tutacak Yer")] 
    public Transform RightHandPos;
    public Transform LeftHandPos;

    public Transform RightPosObj;
    public Transform LeftPosObj;

    [Header("Child Obj")] 
    public GameObject ChildObj;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cl = GetComponent<Collider>();
        if (isServer)
        {
            ServerStateChange(true);
        }
    }

    void Update()
    {
        if (isServer)
        {
            ServerStateChange();
            ServerBoxParentUpdate();
        }
    }
    [Server]
    private void ServerBoxParentUpdate()
    {
        RpcBoxParentUpdate();
    }

    [ClientRpc]
    private void RpcBoxParentUpdate()
    {
        if (ChildObj != null && isInteract)
        {
            this.transform.position = ChildObj.transform.position;
            if (RightPosObj != null && LeftPosObj != null)
            {
                RightPosObj.position = RightHandPos.position;
                LeftPosObj.position = LeftHandPos.position;
            }
        }
    }

    [Server]
    private void ServerStateChange()
    {
        RpcStateChange();
    }

    [ClientRpc]
    private void RpcStateChange()
    {
        if (rb != null && cl != null)
        {
            if (!isStateFree)
            {
                rb.isKinematic = true;
                cl.enabled = false;
            }
            else
            {
                rb.isKinematic = false;
                cl.enabled = true;
            }
        }
    }
    
    public void ItemNotParentObj(Vector3 offset)
    {
        isInteract = false;
        this.transform.parent = null;
        this.transform.DOMove(offset, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => PutItem());
    }

    private void PutItem()
    {
        ServerStateChange(true);
    }
    [Server]
    public void ServerStateChange(bool value)
    {
        isStateFree = value;
    }
    
    // Hand system
    public void ItemParentObj(Transform interactPosRot)
    {
        ChildObj = interactPosRot.gameObject;
        this.transform.parent = interactPosRot.parent;
        this.transform.position = interactPosRot.position;
        this.transform.localRotation = Quaternion.Euler(0, 90, 0);
        isInteract = true;
    }

    public void HandSystem(Transform RightPos, Transform RightRot, Transform LeftPos, Transform LeftRot)
    {
        RightPosObj = RightPos;
        LeftPosObj = LeftPos;
        RightPos.position = RightHandPos.position;
        RightPos.rotation = Quaternion.Euler(0, 90, 0);
        LeftPos.position = LeftHandPos.position;
        LeftPos.rotation = Quaternion.Euler(0, 90, 0);
    }

    private void PickUp(Vector3 offset)
    {
        this.transform.position = offset;
    }
}
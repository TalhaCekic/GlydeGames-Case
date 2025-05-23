using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class BrushInteract : NetworkBehaviour
{
  [SyncVar] public bool isInteract;

    [Header("El Tutacak Yer")] 
    public Transform RightHandPos;
    public Transform LeftHandPos;

    public Transform RightPosObj;
    public Transform LeftPosObj;
    
    public Rigidbody rb;
    public Collider collider;
    
    [Header("Child Obj")] public GameObject ChildObj;
    
    void Update()
    {
        if (isServer)
        {
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
                //RightRotObj.position = RightHandRot.position;
                LeftPosObj.position = LeftHandPos.position;
                //LeftRotObj.position = RightHandRot.position;
            }
        }
    }

    public void BoxParentObj( Transform interactPosRot)
    {
        ChildObj = interactPosRot.gameObject;
        //this.transform.parent = interactPosRot.parent;
        this.transform.SetParent(interactPosRot);
        //this.transform.position = interactPosRot.position;
        this.transform.localPosition = Vector3.zero;
        //this.transform.localRotation = Quaternion.Euler(0,-90,0);
        this.transform.localRotation = Quaternion.Euler(0,-90,0);
    }
    public void HandSystem(Transform RightPos, Transform LeftPos)
    {
        RightPosObj = RightPos;
        LeftPosObj = LeftPos;
        RightPos.position = RightHandPos.position;
        //RightRot.rotation = RightHandRot.rotation;
        LeftPos.position = LeftHandPos.position;
        //LeftRot.rotation =LeftHandRot.rotation;
    }
    void onComplateIBox()
    {
        Destroy(gameObject);
    }
    
    [Server]
    public void CmdSetInteractBox()
    {
        isInteract = true;
        RpcSetInteractBox();
    }

    [ClientRpc]
    public void RpcSetInteractBox()
    {
         rb.isKinematic = true;
         collider.enabled = false;
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
        ServerStateChange(false);
    }
    [Server]
    public void ServerStateChange(bool value)
    {
        isInteract = value;
        RpcStateChange();
    }

    [ClientRpc]
    private void RpcStateChange()
    {
         rb.isKinematic = false;
         collider.enabled = true;
    }
}

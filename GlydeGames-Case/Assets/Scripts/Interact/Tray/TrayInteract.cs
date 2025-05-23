using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class TrayInteract : NetworkBehaviour
{
    [SyncVar] public bool isInteract;
    public AudioSource source;
    
    [Header("El Tutacak Yer")] 
    public Transform RightHandPos;
    //public Transform RightHandRot;
    public Transform LeftHandPos;
    // public Transform LeftHandRot;

    public Transform RightPosObj;
    public Transform LeftPosObj;
    
    public Rigidbody rb;
    public Collider collider;
    
    [Header("Child Obj")] public GameObject ChildObj;

    [Header("Chair System")] 
    public GameObject ChairSelect;

    [SyncVar] private bool isDelete;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        
        GameObject BoxSpawnObj = GameObject.Find("BoxSpawnPos");
        if(!isServer)return;
        ServerStart();

    }
    
    [Server]
    private void ServerStart()
    {
        RpcStart();
    }
    [ClientRpc]
    private void RpcStart()
    {
        gameObject.AddComponent<NetworkRigidbodyReliable>();
    }

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
        this.transform.localRotation = Quaternion.Euler(0,-180,0);
        //transform.rotation = Quaternion.Euler(0,180,0);
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
    // Hand system
    public void ItemParentObj(Transform interactPosRot)
    {
        isInteract = false;
       // ChildObj = interactPosRot.gameObject;
        this.transform.SetParent(interactPosRot);
        this.transform.DORotate(Vector3.zero, 0.2f);
        this.transform.DOMove(interactPosRot.position, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => PutItem());
        source.Play();
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
        RpcStateChange(); ////
    }

    [ClientRpc]
    public void RpcSetInteractBox()
    {
         // rb.isKinematic = true;
         // collider.enabled = false;
         if (ChairSelect != null)
         {
             CustomerManager.instance.AddChairs(ChairSelect);
             ChairSelect = null;
         }
    }
    public void ItemNotParentObj(Vector3 offset)
    {
        if (isDelete)
        {
            isInteract = false;
            this.transform.parent = null;
            this.transform.DOMove(new Vector3(offset.x,offset.y+0.1f,offset.z-0.2f), 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => PutItem());
            this.transform.DOScale(Vector3.zero, 0.5f);
        }
        else
        {
            isInteract = false;
            this.transform.parent = null;
            RightPosObj = null;
            LeftPosObj = null;
            ChildObj = null;
            ServerStateChange(false);
            this.transform.DORotate(Vector3.zero, 0.2f);
            this.transform.DOMove(offset, 0.2f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => PutItem());
        }
        source.Play();
    }

    private void PutItem()
    {
        if (isDelete)
        {
            Destroy(gameObject);
        }
    }
    [Server]
    public void ServerStateChange(bool value)
    {
        isInteract = value;
        RpcStateChange();
    }

    [ClientRpc]
    public void RpcStateChange()
    {
        if (rb != null && collider != null)
        {
            if (!isInteract)
            {
                rb.isKinematic = false;
                collider.enabled = true;
                this.enabled = false;
            }
            else
            {
                rb.isKinematic = true;
                collider.enabled = false;
                this.enabled = true;
            }
        }
    }

    [ClientRpc]
    public void RpcChairAdd(GameObject ChairSelected)
    {
        ChairSelect = ChairSelected;
    }

    public void ServerColliderFalse()
    {
        ServerStateChange(true);
    }
    public void ServerDeleteSystem(bool value)
    {
        isDelete = value;
    }
}

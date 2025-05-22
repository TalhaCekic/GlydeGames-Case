using DG.Tweening;
using Mirror;
using Steamworks;
using Unity.Mathematics;
using UnityEngine;

public class ItemInteract : NetworkBehaviour
{
    [SyncVar] public bool isReadyProduct;

    [Header("Item")] 
    [SyncVar] public string itemName;
    [SyncVar] public bool isStateFree;
    [SyncVar] public bool isInteract;

    public Rigidbody rb;
    public Collider cl;
    public BoxCollider collision;

    [Header("El Tutacak Yer")] 
    public Transform RightHandPos;
    public Transform LeftHandPos;

    public Transform RightPosObj;
    public Transform LeftPosObj;

    [Header("Child Obj")] public GameObject ChildObj;
    
    void Start()
    {
        if (isServer)
        {
            ServerStateChange();
        }
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
                LeftPosObj.position = LeftHandPos.position;
            }
        }
    }
    // FONKSİYONLAR STATE ////////////////

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
                collision.enabled = false;
                //networkIdentity.enabled = false;
                //networkTransformReliable.enabled = false;
                //networkRigidbodyReliable.enabled = false;
            }
            else
            {
                rb.isKinematic = false;
                cl.enabled = true;
                collision.enabled = true;
                //networkIdentity.enabled = true;
                //networkTransformReliable.enabled = true;
                //networkRigidbodyReliable.enabled = true;
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
        ServerStateChange();
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
        RightPos.rotation = Quaternion.Euler(0, 0, 0);
        LeftPos.position = LeftHandPos.position;
        LeftPos.rotation = Quaternion.Euler(0, 0, 0);
    }
    
    public void Trash(Transform trashPos)
    {   
        this.transform.SetParent(null);
        isInteract = false;
        Vector3 newPos = new Vector3(trashPos.position.x, trashPos.position.y + 1.5f, trashPos.position.z);
        this.transform.DOScale(Vector3.zero, 0.8f);
        this.transform.DOMove(newPos, 0.5f)
            .OnComplete(() => onComplateTrash());
    }

    void onComplateTrash()
    {
        Destroy(this);
    }
    
}
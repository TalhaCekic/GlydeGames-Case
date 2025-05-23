using DG.Tweening;
using Edgegap;
using Mirror;
using Player.PlayerControl;
using Steamworks;
using Unity.Mathematics;
using UnityEngine;

public class ItemInteract : NetworkBehaviour
{
    [SyncVar] public bool isReadyProduct;
    public AudioSource source;

    public ItemName _ItemName;
    [Header("Item")] 
    [SyncVar] public string itemName;
    [SyncVar] public bool isStateFree;
    [SyncVar] public bool isInteract;
    [SyncVar] public bool isThrow;
    [SyncVar] public float DelayThrow;

    public Rigidbody rb;
    public BoxCollider collision;

    [Header("El Tutacak Yer")] 
    public Transform RightHandPos;

    public Transform RightPosObj;

    [Header("Child Obj")] public GameObject ChildObj;

    [SyncVar] private bool isDelete;
    void Start()
    {
        if (isServer)
        {
            ServerStateChange();
            ServerName();
        }
    }

    [Server]
    private void ServerName()
    {
        _ItemName.ServerItemName(itemName);
        RpcStart();
    }

    [ClientRpc]
    private void RpcStart()
    {
        gameObject.AddComponent<NetworkRigidbodyReliable>();
    }

    void LateUpdate()
    {
        if (isServer)
        {
            ServerBoxParentUpdate();
        }
    }

    [Server]
    public void ServerBoxParentUpdate()
    {
        RpcBoxParentUpdate();
        if (isThrow)
        {
            if (DelayThrow >= 0.5f)
            {
                isThrow = false;
                DelayThrow = 0;
            }
            else
            {
                DelayThrow += Time.deltaTime;
            }
        }
    }

    [ClientRpc]
    private void RpcBoxParentUpdate()
    {
        if (ChildObj != null && isInteract)
        {
            this.transform.position = ChildObj.transform.position;
            if (RightPosObj != null)
            {
                RightPosObj.position = RightHandPos.position;
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
    public void RpcStateChange()
    {
        if (rb != null && collision != null)
        {
            if (!isStateFree)
            {
                rb.isKinematic = true;
                collision.enabled = false;
                this.enabled = true;
            }
            else
            {
                rb.isKinematic = false;
                collision.enabled = true;
                this.enabled = false;
            }
        }
    }

    public void ItemNotParentObj(Vector3 offset)
    {
        if (isDelete)
        {
            source.Play(); // ses ekle
            isInteract = false;
            this.transform.parent = null;
            this.transform.DOMove(new Vector3(offset.x,offset.y+0.1f,offset.z), 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => PutItem());
            this.transform.DOScale(Vector3.zero, 0.5f);
        }
        else
        {
            source.Play(); // ses ekle
            isInteract = false;
            this.transform.parent = null;
            this.transform.DOMove(offset, 0.2f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => PutItem());
        }
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
        this.transform.SetParent(interactPosRot);
        this.transform.localPosition = Vector3.zero; 
        this.transform.localRotation = Quaternion.Euler(0, 0, 0);
        isInteract = true;
    }

    public void HandSystem(Transform RightPos)
    {
        RightPosObj = RightPos;
        RightPos.position = RightHandPos.position;
        RightPos.rotation = Quaternion.Euler(-77, -42, -94);
    }
    
    public void Trash(Transform trashPos)
    {   
        this.transform.SetParent(null);
        isInteract = false;
        Vector3 newPos = new Vector3(trashPos.position.x, trashPos.position.y + 0.5f, trashPos.position.z);
        this.transform.DOScale(Vector3.zero, 0.8f);
        this.transform.DOMove(newPos, 0.5f)
            .OnComplete(() => onComplateTrash());
    }

    void onComplateTrash()
    {
        Destroy(this);
    }

    public void rbForce()
    {
        rb.AddForce(Vector3.forward*10);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isThrow)
            {
                Vector3 targetPos = new Vector3(rb.velocity.x, rb.velocity.y +5, rb.velocity.z);
                other.gameObject.GetComponent<Player.PlayerControl.PlayerController>().rbForce(targetPos.normalized);
            }
        }
    }
    public void ServerDeleteSystem(bool value)
    {
        isDelete = value;
    }
    
}
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;

public class ObjInteract : NetworkBehaviour
{
    [SyncVar] public bool isInteract;
    [SyncVar] public bool isDeleteSpawnItemList;

    public string itemName;
    [SyncVar] public int _maxObjAmount;
    [SyncVar] public int _objAmount;
    
    public Rigidbody rb;
    public Collider collider;

    [Header("El Tutacak Yer")] 
    public Transform RightHandPos;
    //public Transform RightHandRot;
    public Transform LeftHandPos;
   // public Transform LeftHandRot;

    public Transform RightPosObj;
    public Transform LeftPosObj;

    [Header("Child Obj")] public GameObject ChildObj;
    
    [Header("Box Spawn System")] 
    public BoxSpawnerManager boxSpawnerManager;

    [Header("UI")] public TMP_Text AmountText;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        
        GameObject BoxSpawnObj = GameObject.Find("BoxSpawnPos");
        boxSpawnerManager = BoxSpawnObj.GetComponent<BoxSpawnerManager>();
        
        AmountText.text = _objAmount.ToString();
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
        //this.transform.localRotation = Quaternion.Euler(0,-90,0);
        this.transform.localRotation = Quaternion.Euler(0,-90,0);
    }

    public void HandSystem(Transform RightPos, Transform RightRot, Transform LeftPos, Transform LeftRot)
    {
        RightPosObj = RightPos;
        LeftPosObj = LeftPos;
        RightPos.position = RightHandPos.position;
        //RightRot.rotation = RightHandRot.rotation;
        LeftPos.position = LeftHandPos.position;
        //LeftRot.rotation =LeftHandRot.rotation;
    }

    public void Trash(Transform trashPos)
    {
        this.transform.SetParent(null);
        isInteract = false;
        Vector3 newPos = new Vector3(trashPos.position.x, trashPos.position.y + 1.5f, trashPos.position.z);
        this.transform.DOScale(Vector3.zero, 0.8f);
        this.transform.DOMove(newPos, 0.3f)
            .OnComplete(() => onComplateTrash());
    }

    void onComplateTrash()
    {
        Destroy(gameObject);
    }

    public void BoxNotParentObj()
    {
        this.transform.parent = null;
        this.enabled = false;
    }

    [Server]
    public void CmdSetInteractBox()
    {
        isInteract = true;
        RpcSetInteractBox();
        this.enabled = true;
    }

    [ClientRpc]
    public void RpcSetInteractBox()
    {
        rb.isKinematic = true;
        collider.enabled = false;
        
        // siparişi listeden siler
        if(isDeleteSpawnItemList)return;
        boxSpawnerManager.RpcItemListRemove();
        isDeleteSpawnItemList = true;
    }

    [Server]
    public void CmdSetDropBox()
    {
        isInteract = false;
        RpcSetDropBox();
    }

    [ClientRpc]
    public void RpcSetDropBox()
    {
        rb.isKinematic = false;
        collider.enabled = true;
    }
       
    public void GiveBox(int _amount)
    {
        _objAmount -= _amount;
        AmountText.text = _objAmount.ToString();
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

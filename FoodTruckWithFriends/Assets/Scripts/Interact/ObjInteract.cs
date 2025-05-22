using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;

public class ObjInteract : NetworkBehaviour
{
    public GameObject spawnObj;
    [SyncVar] public bool isSpawn;
    [SyncVar] public bool isInteract;

    public ScribtableOrderItem scribtableOrderItem;
    public string itemName;

    [Header("Close And Open Box")] 
    public GameObject BoxClosed;
    public GameObject BoxOpened;
    [SyncVar] public bool isClosed;

    [Header("PositionObj")] 
    public Transform itemsSlot;
    public List<Transform> BoxPosition = new List<Transform>();
    public List<GameObject> item = new List<GameObject>();
    public int slotCurrent;
    public float placementDuration = 1.0f;

    [SyncVar] public float SpawnDelay;

    public Rigidbody rb;
    public BoxCollider collider;

    [Header("El Tutacak Yer")]
    public Transform RightHandPos;
    public Transform LeftHandPos;  
    
    public Transform RightPosObj;
    public Transform LeftPosObj;

    [Header("Child Obj")] public GameObject ChildObj;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        if (isServer)
        {
            CmdBoxStatus();
        }
    }

    [Server]
    public void ServerBoxAling()
    {
        if (!isSpawn && BoxPosition.Count > 0)
        {
            SpawnDelay += Time.deltaTime;
            if (SpawnDelay >= 0.2f)
            {
                foreach (var Name in scribtableOrderItem.itemPrefab)
                {
                    if (Name.GetComponent<ItemInteract>().itemName == itemName)
                    {
                        int currentIndex = item.Count;
                        if (currentIndex < BoxPosition.Count)
                        {
                            //Name.GetComponent<ItemInteract>().ServerStateChange(false);
                            spawnObj = Instantiate(Name);
                            NetworkServer.Spawn(spawnObj);
                            RpcBoxItemPos(spawnObj, currentIndex);
                            SpawnDelay = 0f; // Reset timer after each spawn
                        }
                        else
                        {
                            isSpawn = true; // All items spawned
                        }
                    }
                }
            }
        }
    }

    [ClientRpc]
    public void RpcBoxItemPos(GameObject spawnObj, int i)
    {
        item.Add(spawnObj);
        spawnObj.transform.SetParent(itemsSlot);
        spawnObj.transform.rotation = quaternion.identity;
    }

    void Update()
    {
        if (isServer)
        {
            //CmdBoxStatus();
            ServerBoxAling();
            ServerBoxParentUpdate();
            Serverpos();
        }
    }

    // [Server]
    // public void ServerBoxAling()
    // {
    //     if (!isSpawn)
    //     {
    //         foreach (var Name in scribtableOrderItem.itemPrefab)
    //         {
    //             if (Name.GetComponent<ItemInteract>().itemName == itemName)
    //             {
    //                 for (int i = 0; i < BoxPosition.Count; i++)
    //                 {
    //                     Name.GetComponent<ItemInteract>().ServerStateChange(false);
    //                     spawnObj = Instantiate(Name);
    //                     NetworkServer.Spawn(spawnObj);
    //                     RpcBoxItemPos(spawnObj, i);
    //                 }
    //             }
    //         }
    //     }
    //     else
    //     {
    //         SpawnDelay += Time.deltaTime;
    //     }
    // }

    [Server]
    private void Serverpos()
    {
        if (item.Count <= BoxPosition.Count && item.Count != 0)
        {
            for (int a = 0; a < BoxPosition.Count; a++)
            {
                item[a].transform.position = BoxPosition[a].transform.position;
            }
        }
    }

    public void BoxTransferSendItem(List<Transform> Parent, Transform itemsObjPos, List<GameObject> items,
        int itemSlotCurrent, string ShelfName, bool Value)
    {
        for (int i = 0; i < item.Count; i++)
        {
            if (ShelfName == itemName)
            {
                if (itemSlotCurrent < Parent.Count)
                {
                    if (Value)
                    {
                        GameObject product = item[i];
                        Transform shelf = Parent[itemSlotCurrent];
                        float delay = i * placementDuration;

                        items.Insert(0, product);

                        item.Remove(product);
                        slotCurrent = item.Count;

                        PlaceProductOnShelf(product, shelf, itemsObjPos, delay);
                        Value = false;
                    }
                }
            }
        }
    }

    void PlaceProductOnShelf(GameObject product, Transform shelf, Transform itemsObjPos, float delay)
    {
        product.transform.DOMove(shelf.transform.position, 0.2f)
            .SetDelay(delay)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => OnProductPlaced(product, itemsObjPos));
    }

    void OnProductPlaced(GameObject product, Transform itemsObjPos)
    {
        // product.transform.SetParent(itemsObjPos);
        product.transform.parent = itemsObjPos;
        product.transform.rotation = itemsObjPos.rotation;
    }

    [Server]
    public void CmdBoxStatus()
    {
        RpcBoxStatus();
    }

    [ClientRpc]
    public void RpcBoxStatus()
    {
        if (isClosed)
        {
            BoxOpened.SetActive(false);
            BoxClosed.SetActive(true);
        }
        else
        {
            BoxClosed.SetActive(false);
            BoxOpened.SetActive(true);
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

    public void BoxParentObj(Transform InteractPos, Transform interactPosRot)
    {
        ChildObj = interactPosRot.gameObject;
        this.transform.parent = interactPosRot.parent;
        this.transform.position = interactPosRot.position;
        this.transform.localRotation = Quaternion.Euler(0, 90, 0);
    
        //this.transform.position = InteractPos.position;
        //this.transform.rotation = InteractPos.rotation;
    }

    public void HandSystem(Transform RightPos, Transform RightRot, Transform LeftPos, Transform LeftRot)
    {
        RightPosObj = RightPos;
        LeftPosObj = LeftPos;
        RightPos.position = RightHandPos.position;
        RightPos.rotation = Quaternion.Euler(0,90,0);
        //RightRot.rotation = RightHandPos.rotation;
        // RightRot.rotation = Quaternion.Euler(90,0,0);
        LeftPos.position = LeftHandPos.position;
        LeftPos.rotation = Quaternion.Euler(0,90,0);
        //LeftRot.rotation = LeftHandPos.rotation;
        //LeftRot.rotation = Quaternion.Euler(90,0,0);
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
    public void BoxCoverOpen()
    {
        isClosed = false;
    }

    public void BoxCoverClose()
    {
        isClosed = true;
    }

    public void BoxNotParentObj()
    {
        this.transform.parent = null;
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
}
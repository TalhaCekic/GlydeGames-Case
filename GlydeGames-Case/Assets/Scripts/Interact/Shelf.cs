using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using Mirror;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine.Rendering;

public class Shelf : NetworkBehaviour
{
    public Transform itemsSlot;
    public List<Transform> Slots = new List<Transform>();
    public List<GameObject> item = new List<GameObject>();
    public float placementDuration = 1.0f;

    public string ItemName;
    public int slotCurrent;

    public GameObject CurrentObj;

    // customer Target Pos
    void Start()
    {
        ItemName = null;
    }

    void Update()
    {
        if (isServer)
        {
            ServerShelf();
        }
    }

    [Server]
    public void ServerShelf()
    {
        RpcShelf();
    }

    [ClientRpc]
    private void RpcShelf()
    {
        slotCurrent = item.Count;
        if (item.Count == 0)
        {
            ItemName = null;
        }
    }

    // itemi ele almak için
    public void ShelfTransferSendItemPlayer(Transform Parent, Transform itemsObjPos, bool Value,Transform RightPos, Transform RightRot, Transform LeftPos, Transform LeftRot)
    {
        for (int i = 0; i < item.Count; i++)
        {
            if (Value)
            {
                GameObject product = item[i];
                Transform Box = Parent;
                float delay = i * placementDuration;
                
                item.Remove(product);
                slotCurrent = item.Count;

                CurrentObj = product;
                
                PlayerPlaceProductOnShelf(product, Box, itemsObjPos, delay,RightPos,RightRot,LeftPos,LeftRot);
                Value = false;
            }
        }
    }
    void PlayerPlaceProductOnShelf(GameObject product, Transform Pos, Transform itemsObjPos, float delay,Transform RightPos, Transform RightRot, Transform LeftPos, Transform LeftRot)
    {
        product.transform.DOMove(Pos.transform.position, 0.2f)
            .SetDelay(delay)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => PlayerOnProductPlaced(product, Pos, itemsObjPos,RightPos,RightRot,LeftPos,LeftRot));
    }
    void PlayerOnProductPlaced(GameObject product, Transform Pos, Transform itemsObjPos,Transform RightPos, Transform RightRot, Transform LeftPos, Transform LeftRot)
    {
        product.GetComponent<ItemInteract>().enabled = true;
        product.transform.rotation = Quaternion.Euler(-90,0,0);
        product.GetComponent<ItemInteract>().ItemParentObj( itemsObjPos);
       // product.GetComponent<ItemInteract>().HandSystem(RightPos, RightRot, LeftPos, LeftRot);
    }
    
    // kutu eylemleri için
    public void ShelfTransferSendItem(List<Transform> Parent, Transform itemsObjPos, List<GameObject> items,int itemSlotCurrent, string BoxName, bool Value)
    {
        for (int i = 0; i < item.Count; i++)
        {
            if (BoxName == ItemName)
            {
                if (Value)
                {
                    GameObject product = item[i];
                    Transform Box = Parent[Parent.Count - items.Count - 1];
                    float delay = i * placementDuration;

                    items.Insert(0, product);

                    item.Remove(product);
                    slotCurrent = item.Count;

                    PlaceProductOnShelf(product, Box, itemsObjPos, delay);
                    Value = false;
                }
            }
        }
    }

    void PlaceProductOnShelf(GameObject product, Transform Pos, Transform itemsObjPos, float delay)
    {
        product.transform.DOMove(Pos.transform.position, 0.2f)
            .SetDelay(delay)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => OnProductPlaced(product, Pos, itemsObjPos));
    }

    
    void OnProductPlaced(GameObject product, Transform Pos, Transform itemsObjPos)
    { 
        //product.transform.SetParent(itemsObjPos);
       product.transform.parent = itemsObjPos;
       product.transform.rotation = Quaternion.Euler(90,0,0);
       product.transform.position = Pos.transform.position;
    }
}
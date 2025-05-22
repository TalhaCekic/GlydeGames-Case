using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Mirror;
using UnityEngine;

[System.Serializable]
public class OrderItemPacket
{
    public string itemName;
    public int quantity;

    public OrderItemPacket(string name, int qty)
    {
        itemName = name;
        quantity = qty;
    }
    public override bool Equals(object obj)
    {
        if (obj is OrderItemPacket)
        {
            var other = (OrderItemPacket)obj;
            return this.itemName == other.itemName;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return itemName.GetHashCode();
    }
}
public class ItemPacket : NetworkBehaviour
{
    public GameObject Canvas;
    public List<OrderItemPacket> orderItems = new List<OrderItemPacket>();
    //public List<GameObject> CurrentOwnedItem = new List<GameObject>();
    //public List<string> CurrentOwnedItemString = new List<string>();

    private void Start()
    {
        if (isServer)
        {
            ServerStart();
        }
    }

    [Server]
    private void ServerStart()
    {
        RpcStart();
    }
    [ClientRpc]
    private void RpcStart()
    {
        Canvas.SetActive(false);
    }

    void Update()
    {
        
    }

    public void PossessionItem(GameObject PlayerHandObj)
    {
        PlayerHandObj.transform.parent = null; 
        PlayerHandObj.transform.DOMove(transform.position, 0.6f)
            .OnComplete(() => ComplitePossessionItem(PlayerHandObj));
        AddOrderItem(PlayerHandObj.GetComponent<ItemInteract>().itemName);
    }
    // liste ekleme clientler için
    public void AddOrderItem(string itemName)
    {
        //var newItemPacket = new OrderItemPacket(itemName);
        var existingItem = orderItems.FirstOrDefault(item => item.itemName == itemName);

        if (existingItem != null)
        {
            existingItem.quantity += 1;
        }
        else
        {
            var newItemPacket = new OrderItemPacket(itemName, 1);
            orderItems.Add(newItemPacket);
        }
    }
    private void ComplitePossessionItem(GameObject PlayerHandObj)
    {
        Destroy(PlayerHandObj);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("aa");
        }
    }
}

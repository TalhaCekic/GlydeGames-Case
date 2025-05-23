using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class CombineProduct : NetworkBehaviour
{
    public ItemInteract _ıtemInteract;
    public ItemName _ItemName;
    [SyncVar] public string ComplateItemName;
    public Datas ItemDatas;

    [Header("İçerik")] [SyncVar] public bool Meat;
    [SyncVar] public bool Cheese;
    [SyncVar] public bool Tomato;
    [SyncVar] public bool Lettuce;

    [Header("İçerik Konumları")] public Transform MeatPos;
    public Transform CheesePos;
    public Transform TomatoPos;
    public Transform LettucePos;

    [ClientRpc]
    private void ItemComplate()
    {
        if (Meat)
        {
            //ComplateItemName = "Menu - 1";
            ComplateItemName = ItemDatas._foodData[0]._name;
            _ItemName.ServerItemName(ItemDatas._foodData[0]._name);
        }

        if (Meat && Tomato)
        {
            //ComplateItemName = "Menu - 2";
            ComplateItemName = ItemDatas._foodData[1]._name;
            _ItemName.ServerItemName(ItemDatas._foodData[1]._name);
        }

        if (Meat && Lettuce)
        {
            //ComplateItemName = "Menu - 3";
            ComplateItemName = ItemDatas._foodData[2]._name;
            _ItemName.ServerItemName(ItemDatas._foodData[2]._name);
        }

        if (Meat && Cheese)
        {
            //ComplateItemName = "Menu - 4";
            ComplateItemName = ItemDatas._foodData[3]._name;
            _ItemName.ServerItemName(ItemDatas._foodData[3]._name);
        }

        if (Meat && Tomato && Lettuce)
        {
            //ComplateItemName = "Menu - 5";
            ComplateItemName = ItemDatas._foodData[4]._name;
            _ItemName.ServerItemName(ItemDatas._foodData[4]._name);
        }

        if (Meat && Tomato && Cheese)
        {
            //ComplateItemName = "Menu - 6";
            ComplateItemName = ItemDatas._foodData[5]._name;
            _ItemName.ServerItemName(ItemDatas._foodData[5]._name);
        }

        if (Meat && Lettuce && Cheese)
        {
            //ComplateItemName = "Menu - 7";
            ComplateItemName = ItemDatas._foodData[6]._name;
            _ItemName.ServerItemName(ItemDatas._foodData[6]._name);
        }

        if (Meat && Tomato && Lettuce && Cheese)
        {
            //ComplateItemName = "Menu - 8";
            ComplateItemName = ItemDatas._foodData[7]._name;
            _ItemName.ServerItemName(ItemDatas._foodData[7]._name);
        }
    }

    [Server]
    public void Combine(string name, GameObject handObj, GameObject obj)
    {
        if (name == "Baked Meat")
        {
            if (Meat) return;
            handObj.GetComponent<ItemInteract>().ServerStateChange(false);
            handObj.GetComponent<ItemInteract>().isInteract = false;
            GoMove(handObj);
           // obj.GetComponent<PlayerInteract>().CombineInteract();
           _ıtemInteract.isReadyProduct = true;
            Meat = true;
            ItemComplate();
        }

        if (name == "Cheese")
        {
            if (Cheese) return;
            handObj.GetComponent<ItemInteract>().ServerStateChange(false);
            handObj.GetComponent<ItemInteract>().isInteract = false;
            GoMove(handObj);
            //obj.GetComponent<PlayerInteract>().CombineInteract();
            _ıtemInteract.isReadyProduct = true;
            Cheese = true;
            ItemComplate();
        }

        if (name == "Tomato")
        {
            if (Tomato) return;
            handObj.GetComponent<ItemInteract>().ServerStateChange(false);
            handObj.GetComponent<ItemInteract>().isInteract = false;
            GoMove(handObj);
            //obj.GetComponent<PlayerInteract>().CombineInteract();
            _ıtemInteract.isReadyProduct = true;
            Tomato = true;
            ItemComplate();
        }

        if (name == "Lettuce")
        {
            if (Lettuce) return;
            handObj.GetComponent<ItemInteract>().ServerStateChange(false);
            handObj.GetComponent<ItemInteract>().isInteract = false;
            GoMove(handObj);
           // obj.GetComponent<PlayerInteract>().CombineInteract();
           _ıtemInteract.isReadyProduct = true;
            Lettuce = true;
            ItemComplate();
        }
    }
    [ClientRpc]
    private void GoMove(GameObject handObj)
    {
        handObj.transform.DOMove(MeatPos.position, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => parent(handObj));
    }

   // [ClientRpc]
    private void parent(GameObject handObj)
    {
        handObj.transform.SetParent(MeatPos);
        handObj.transform.localPosition = Vector3.zero;
        handObj.transform.localRotation = Quaternion.identity;
    }
}
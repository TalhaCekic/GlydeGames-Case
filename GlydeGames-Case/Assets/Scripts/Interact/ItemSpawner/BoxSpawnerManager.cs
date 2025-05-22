using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BoxSpawnerManager : NetworkBehaviour
{
    public List<Transform> Slots = new List<Transform>();
    public List<GameObject> Items = new List<GameObject>();

    [SyncVar] public float ObjEnableDelay;

    void Update()
    {
        if (isServer)
        {
            ServerUpdate();
        }
    }

    [Server]
    private void ServerUpdate()
    {
        RpcUpdate();
    }

    [ClientRpc]
    private void RpcUpdate()
    {
    }

    // listeye ekleme yapar ve slotlara ekleme yapar
    [Server]
    public void ServerItemListAdd()
    {
        RpcItemListAdd();
    }

    [ClientRpc]
    public void RpcItemListAdd()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].SetActive(false);
            Items[i].transform.parent = this.gameObject.transform;
            Items[i].transform.position = Slots[i].position;
        }
    }

    // listeye ekleme yapar ve slotları çıkartma yapapar
    //[Server]
    public void ServerItemListRemove()
    {
        ObjEnableDelay += Time.deltaTime;
        if (ObjEnableDelay > 2)
        {
            RpcItemListRemove();
            ObjEnableDelay = 0;
        }
    }

   // [ClientRpc]
    public void RpcItemListRemove()
    {
        if (Items.Count > 0)
        {
            Items[0].SetActive(true);
            Items[0].transform.parent = null;
            Items.Remove(Items[0]);
        }
        // eski spawn sistemi
        // for (int i = 0; i < Items.Count; i++)
        // {
        //     Items[i].SetActive(true);
        //     Items[i].transform.parent = null;
        //     
        // }
        // Items.Clear();
    }
}
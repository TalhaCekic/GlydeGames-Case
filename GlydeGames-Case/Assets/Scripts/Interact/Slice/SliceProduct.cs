using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SliceProduct : NetworkBehaviour
{
    public ItemInteract _ıtemInteract;
    [SyncVar] public bool isSlice;
    public GameObject Full;
    public GameObject Sliced;
    [SyncVar] public float slideDelay;

    void Start()
    {
        _ıtemInteract = GetComponent<ItemInteract>();
        if (isServer)
        {
            ServerSliceSystem();
        }
    }

    [Server]
    private void ServerSliceSystem()
    {
        RpcSliceSystemTrue();
    }

    [ClientRpc]
    public void RpcSliceSystemTrue()
    {
        Full.SetActive(true);
        Sliced.SetActive(false);
    }

    [Server]
    public void ServerDelaySlide()
    {
        slideDelay += Time.deltaTime;
        if (slideDelay >= 3)
        {
            RpcSliceSystem();
            isSlice = true;
            _ıtemInteract.isReadyProduct = true;
        }
    }
    [ClientRpc]
    public void RpcSliceSystem()
    {
        Full.SetActive(false);
        Sliced.SetActive(true);
    }
    
    
    [Server]
    public void ServerDelaySlideCanceled()
    {
        slideDelay = 0;
    }
}
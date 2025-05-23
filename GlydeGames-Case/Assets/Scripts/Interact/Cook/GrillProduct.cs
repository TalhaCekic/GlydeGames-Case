using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class GrillProduct : NetworkBehaviour
{
    public ItemInteract _ıtemInteract;
    
    public ItemName _ItemName;

    [SyncVar] public bool isCookPos;
    
    [Header("Cook")] 
    [SyncVar] public float CookDelay;
    [SyncVar] public float MaxCookDelay;
    [SyncVar] public bool isCooked;
    public Renderer _renderer;
    public Material BakedMaterial;

    public GrillMachineButton _grillMachineButton;
    
    
    void Start()
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
        //_ItemName.ServerItemName("Raw Meat");
        
    }
    [ClientRpc]
    private void RpcStart()
    {
        GameObject obj = GameObject.Find("GrillButton");
        _grillMachineButton = obj.GetComponent<GrillMachineButton>();
    }

    void Update()
    {
        if (isServer && _ıtemInteract.isStateFree)
        {
            ServerCooking();
        }
    }

    // PİŞİRME EYLEMLERİ ////////////////
    [Server]
    private void ServerCooking()
    {
        if (_ıtemInteract.isStateFree && _grillMachineButton.isStart && isCookPos)
        {
            ServerDownCook();
        }
        else
        {
            CookDelay = 0;
        }
    }

    [Server]
    private void ServerDownCook()
    {
        if (CookDelay > MaxCookDelay)
        {
            isCooked = true;
            _ItemName.ServerItemName("Baked Meat");
            ServerCook();
        }
        else
        {
            CookDelay += Time.deltaTime;
        }
    }

    [Server]
    private void ServerCook()
    {
        RpcCook();
    }

    [ClientRpc]
    private void RpcCook()
    {
        _ıtemInteract.isReadyProduct = true;
        if (_ıtemInteract.isReadyProduct)
        {
            _renderer.material = BakedMaterial;
        }

        this.enabled = false;
    }
    
    [Server]
    public void ServerPosChangeBool(bool value)
    {
        isCookPos = value;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class DrinkProduct : NetworkBehaviour
{
    public ItemInteract _ıtemInteract;
    
    public ItemName _ItemName;
    
    [SyncVar] public float isfillDelay;
    [SyncVar] public float isMaxfillDelay;

    [SyncVar] public bool isStartFull;

    public DrinkButton drinkButton;

    [Server]
    public void ServerFilling(GameObject _drinkSlot)
    {
        ServerDownCook(_drinkSlot);
        // if (_ıtemInteract.isStateFree)
        // {
        //     ServerDownCook(_drinkSlot);
        // }
        // else
        // {
        //     isfillDelay = 0;
        // }
    }

    [Server]
    private void ServerDownCook(GameObject _drinkSlot)
    {
        if (isfillDelay > isMaxfillDelay)
        {
            ServerCook(_drinkSlot);
            isStartFull = false;
        }
        else
        {
            isfillDelay += Time.deltaTime;
            isStartFull = true;
        }
    }
    

    [Server]
    private void ServerCook(GameObject _drinkSlot)
    {
        RpcCook(_drinkSlot);
    }

    [ClientRpc]
    private void RpcCook(GameObject _drinkSlot)
    {
        _ıtemInteract.isReadyProduct = true;
        _ıtemInteract.itemName = _drinkSlot.GetComponent<DrinkButton>()._name;
        
        _ItemName.ServerItemName(_ıtemInteract.itemName );

        drinkButton = _drinkSlot.GetComponent<DrinkButton>();
        //_drinkSlot.GetComponent<DrinkButton>().isStart = false; 
        _drinkSlot.GetComponent<DrinkButton>().DrinkStop(); 
        //_ıtemInteract.rb.isKinematic = false;
        //_ıtemInteract.collision.enabled = true;
        this.enabled = false;
    
    }
    
}

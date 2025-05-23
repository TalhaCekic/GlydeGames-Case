using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ItemName : NetworkBehaviour
{
     [SyncVar] public string _ItemName;

    [SyncVar] public bool isDrink, isFood, isSnack;
    
    [Server]
    public void ServerItemName(string value)
    {
        _ItemName = value;
    }
}
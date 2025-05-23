using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DrinkSlot : NetworkBehaviour
{
    public string _name;
    public GameObject Cup;
    
    public Transform ParentTransform;
    public DrinkButton _DrinkButton;
    
    [Header("Tür")]
    [SyncVar] public bool isCola, isPanta, isSoda;

    public void AddCup(GameObject handObj)
    {
        Cup = handObj;
        _DrinkButton._DrinkProduct = handObj.GetComponent<DrinkProduct>();
        Cup.GetComponent<DrinkProduct>().drinkButton = _DrinkButton;
    }
}

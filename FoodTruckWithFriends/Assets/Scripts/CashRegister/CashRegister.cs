using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class CashRegister : NetworkBehaviour
{
    [SyncVar] public bool isInteractCR;
    public Transform CamPos;
    
    // Button List Ayarları
    [Header("Button List && Scroll View")] 
    public GameObject DrinksListObj;
    public GameObject SnacksListObj;
    public GameObject MealsListObj;
    
    public void CmdSetInteractCR() {
        isInteractCR = !isInteractCR;
    }

    // oyuncunun ekrana odaklanmasını sağlar
    public void SetPlayerPosition()
    {
        Camera.main.transform.DOMove(CamPos.transform.position, 1);
        Camera.main.transform.rotation = CamPos.transform.rotation;
    }
    
    // butonları etkilşimi
    public void DrinksList()
    {
        DrinksListObj.SetActive(true);
        SnacksListObj.SetActive(false);
        MealsListObj.SetActive(false);
    } 
    public void SnacksList()
    {
        DrinksListObj.SetActive(false);
        SnacksListObj.SetActive(true);
        MealsListObj.SetActive(false);
    }
    public void MealsList()
    {
        DrinksListObj.SetActive(false);
        SnacksListObj.SetActive(false);
        MealsListObj.SetActive(true);
    }
}
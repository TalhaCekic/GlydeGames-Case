using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class DrinkButton : NetworkBehaviour
{   
    [SyncVar] public bool isStart;
    public string _name;
    public DrinkProduct _DrinkProduct;
    public DrinkSlot _DrinkSlot;

    public GameObject canvasTick;
    public Slider _Slider;

    private void Update()
    {
        if (isServer)
        {
            ServerSlotStart(); 
        }
    }

    [Server]
    private void ServerSlotStart()
    {
        RpcSlotStart();
    }
    [ClientRpc]
    private void RpcSlotStart()
    {
        if (isStart && _DrinkProduct !=null)
        {
            _DrinkProduct = _DrinkSlot.GetComponent<DrinkSlot>().Cup.GetComponent<DrinkProduct>();
            _DrinkProduct.ServerFilling(gameObject);
            RpcSliderValue(_DrinkProduct.isfillDelay, _DrinkProduct.isMaxfillDelay);
            // _DrinkProduct.GetComponent<ItemInteract>().rb.isKinematic = true;
            // _DrinkProduct.GetComponent<ItemInteract>().collision.enabled = false;
            // canvasTick.SetActive(true);
        }
        // else
        // {
        //     canvasTick.SetActive(false);
        // }
    }
    [ClientRpc]
    private void RpcSliderValue(float value , float maxValue)
    {
        _Slider.maxValue = maxValue;
        _Slider.minValue = 0;
        _Slider.value = value;
    }
    public void DrinkStart()
    {
        if(_DrinkProduct ==null)return;
        isStart = true;
        _DrinkProduct = _DrinkSlot.GetComponent<DrinkSlot>().Cup.GetComponent<DrinkProduct>();
        _DrinkProduct.ServerFilling(gameObject);
        _DrinkProduct.GetComponent<ItemInteract>().rb.isKinematic = true;
        _DrinkProduct.GetComponent<ItemInteract>().collision.enabled = false;
        canvasTick.SetActive(false);
    }
    public void DrinkStop()
    {
        RpcSliderValue(0, _DrinkProduct.isMaxfillDelay);
        isStart = false;
        canvasTick.SetActive(true);
    }
    public void TickCanvasClose()
    {
        canvasTick.SetActive(false);
    }
}

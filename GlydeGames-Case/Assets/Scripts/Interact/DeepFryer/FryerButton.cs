using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class FryerButton : NetworkBehaviour
{
    public ItemBox _ItemBox;
    public GameObject FryerObj;
    public DeepFryer _DeepFryer;

    public bool isStart;

    [SyncVar] public float isCookDelay;
    [SyncVar] public float isMaxCookDelay;

    public Transform Pos1;
    public Transform Pos2;

    public GameObject canvasTick;
    public Slider _Slider;

    void Update()
    {
        if (isServer)
        {
            ServerCookPatato();
        }
    }

    [Server]
    private void ServerCookPatato()
    {
        if (isStart)
        {
            if (isCookDelay > isMaxCookDelay)
            {
                ServerCook();
                _DeepFryer.Availability = true;
                RpcSliderValue(0, isMaxCookDelay);
            }
            else
            {
                isCookDelay += Time.deltaTime;
                _DeepFryer.Availability = false;
                RpcSliderValue(isCookDelay, isMaxCookDelay);
            }
        }
    }

    [ClientRpc]
    private void RpcSliderValue(float value, float maxValue)
    {
        _Slider.maxValue = maxValue;
        _Slider.minValue = 0;
        _Slider.value = value;
    }

    [Server]
    private void ServerCook()
    {
        RpcCook();
    }

    [ClientRpc]
    private void RpcCook()
    {
        ServerObjPos();
        isCookDelay = 0;
        isStart = false;
        canvasTick.SetActive(true);
    }

    public void ServerStartChange()
    {
        if (_DeepFryer._ItemBox._itemAmount < 1) return;

        isStart = !isStart;
        ServerObjPos();
    }

    [ClientRpc]
    private void ServerObjPos()
    {
        if (isStart)
        {
            FryerObj.transform.DOMove(Pos1.position, 1);
        }
        else
        {
            FryerObj.transform.DOMove(Pos2.position, 1);
        }
    }

    public void TickCanvasClose()
    {
        canvasTick.SetActive(false);
    }
}
using System;
using Mirror;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

public class LoadingScreen : NetworkBehaviour
{
    public static LoadingScreen instance;
    
    public GameObject LoadSystem;
    public GameObject LoadImage;
    
    [SyncVar] public bool isOpen;
    [SyncVar] public float delay1;
    
    // script içi değişkenler
    [SyncVar] private bool value1;
    [SyncVar] private bool value2;

    void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (isServer)
        {
            ServeLoadScreenSystem();
        }
    }

    [Server]
    private void ServeLoadScreenSystem()
    {
        RpcLoadScreenSystem();
        if (isOpen)
        {
            delay1 += Time.deltaTime;
        }
    }

    [ClientRpc]
    private void RpcLoadScreenSystem()
    {
        if (isOpen)
        {
            event1();
            if (delay1 > 8)
            {
                event2();
                if (delay1 > 10)
                {
                    LoadImage.SetActive(false);
                    LoadingScreenManager.instance.HideLoadingScreen();
                    if (delay1 > 11)
                    {
                        isOpen = false;
                    }
                }
            }
        }
    }

    private void event1()
    {
        if (!value1)
        {
            LoadSystem.SetActive(true);
            LoadImage.SetActive(true);
            LoadingScreenManager.instance.HideLoadingScreen();
            value1 = true;
        }
    }
    private void event2()
    {
        if (!value2)
        {
            LoadingScreenManager.instance.RevealLoadingScreen();
            value2 = true;
        }
    }

    public void LoadScreen()
    {
        LoadImage.SetActive(false);
        LoadingScreenManager.instance.RevealLoadingScreen();
    }
}
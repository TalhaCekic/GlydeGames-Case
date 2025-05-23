using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class BoxViewAndSound : NetworkBehaviour
{
    public ItemBox itemBox;
    public AudioSource source;
    [SyncVar,] public bool value;

    public GameObject inObject;
    public List<GameObject> _list = new List<GameObject>();
    public List<GameObject> objs;

    private int currentIndex = 0;

    void Start()
    {
        if (!isServer) return;
        // Başlangıçta tüm orijinal listedeki elemanları pasif yap
        SetActiveForOriginal(false);

        // Modifiable listedeki elemanları aktif yap
        SetActiveForModifiable(false);
    }

    private void Update()
    {
        if (!isServer) return;
        // if (itemBox._itemName == "Tray Box")
        // {
        //     if (!value)
        //     {
        //         // Başlangıçta tüm orijinal listedeki elemanları pasif yap
        //         SetActiveForOriginal(false);
        //
        //         // Modifiable listedeki elemanları aktif yap
        //         SetActiveForModifiable(false);
        //     }
        // }
    }

    public List<GameObject> TakeItems(int count)
    {
        List<GameObject> takenItems = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            if (objs.Count > 0)
            {
                currentIndex = objs.Count - 1;
                GameObject item = objs[currentIndex];
                objs.RemoveAt(currentIndex);
                takenItems.Add(item);
                SetActiveForOriginal(false);
                SetActiveForModifiable(true);
                source.Play();
            }
            else
            {
                //source.Stop();
                break;
            }
        }

        return takenItems;
    }

    public void AddItem(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (currentIndex < _list.Count)
            {
                GameObject item = _list[currentIndex];
                objs.Insert(currentIndex, item);
                currentIndex++;
                SetActiveForOriginal(false);
                SetActiveForModifiable(true);
                source.Play();
            }
            else
            {
                //source.Stop();
                break;
            }
        }
    }

    [Server]
    public void SetActiveForOriginal(bool isActive)
    {
        RpcSetActiveForOriginal(isActive);
    }

    [ClientRpc]
    private void RpcSetActiveForOriginal(bool isActive)
    { 
        value = true;
        foreach (GameObject obj in _list)
        {
            if (obj != null)
                obj.SetActive(isActive);
            obj.SetActive(false);
            switch (objs.Count)
            {
                case 0:
                    inObject.transform.DOScaleX(0, 0.5f);
                    inObject.SetActive(false);
                    break;
                case 1:
                    inObject.transform.DOScaleX(0.01f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 2:
                    inObject.transform.DOScaleX(0.02f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 3:
                    inObject.transform.DOScaleX(0.05f, 0.5f);
                    inObject.SetActive(true);
                    //inObject.transform.DOMoveY(-0.03f, 0.5f);
                    break;
                case 4:
                    inObject.transform.DOScaleX(0.08f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 5:
                    inObject.transform.DOScaleX(0.1f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 6:
                    inObject.transform.DOScaleX(0.15f, 0.5f);
                    inObject.SetActive(true);
                    //inObject.transform.DOMoveY(0, 0.5f);
                    break;
                case 7:
                case 8:
                    inObject.transform.DOScaleX(0.18f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 9:
                    inObject.transform.DOScaleX(0.2f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 10:
                    inObject.transform.DOScaleX(0.22f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 11:
                    inObject.transform.DOScaleX(0.25f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 12:
                    inObject.transform.DOScaleX(0.3f, 0.5f);
                    inObject.SetActive(true);
                    break;
            }
           // value = false;
        }
    }

    [Server]
    // Modifiable listedeki tüm objeleri pasif yap
    public void SetActiveForModifiable(bool isActive)
    {
        RpcSetActiveForModifiable(isActive);
    }

    [ClientRpc]
    private void RpcSetActiveForModifiable(bool isActive)
    {
        foreach (GameObject obj in objs)
        {
            if (obj != null)
                obj.SetActive(isActive);
            switch (objs.Count)
            {
                case 0:
                    inObject.transform.DOScaleX(0, 0.5f);
                    inObject.SetActive(false);
                    break;
                case 1:
                    inObject.transform.DOScaleX(0.01f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 2:
                    inObject.transform.DOScaleX(0.02f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 3:
                    inObject.transform.DOScaleX(0.05f, 0.5f);
                    inObject.SetActive(true);
                    //inObject.transform.DOMoveY(-0.03f, 0.5f);
                    break;
                case 4:
                    inObject.transform.DOScaleX(0.08f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 5:
                    inObject.transform.DOScaleX(0.1f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 6:
                    inObject.transform.DOScaleX(0.15f, 0.5f);
                    inObject.SetActive(true);
                    //inObject.transform.DOMoveY(0, 0.5f);
                    break;
                case 7:
                case 8:
                    inObject.transform.DOScaleX(0.18f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 9:
                    inObject.transform.DOScaleX(0.2f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 10:
                    inObject.transform.DOScaleX(0.22f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 11:
                    inObject.transform.DOScaleX(0.25f, 0.5f);
                    inObject.SetActive(true);
                    break;
                case 12:
                    inObject.transform.DOScaleX(0.3f, 0.5f);
                    inObject.SetActive(true);
                    break;
            }
        }
    }
}
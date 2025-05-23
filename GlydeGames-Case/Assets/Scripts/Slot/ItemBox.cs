using System;
using Mirror;
using Org.BouncyCastle.Crypto.Generators;
using TMPro;
using UnityEngine;
using System.Linq;
using Unity.Properties;

public class ItemBox : NetworkBehaviour
{
    public static ItemBox instance;
    public BoxViewAndSound boxViewAndSound;
    
    public string _itemName;
    [SyncVar] public int _maxItemAmount;
    [SyncVar] public int _itemAmount;

    public GameObject Prefab;

    [Header("UI")] public TMP_Text Amount;

    [SyncVar] private bool isChangeAddData;

    [SyncVar] private bool isChangeValue;

    private void Start()
    {
        instance = this;
        //&& _itemAmount <= 0
        if(!isServer)return;
        ServerStart();
    }

    [Server]
    public void ServerStart()
    {
        if (_itemName == "Tray Box")
        {
            _itemAmount = 20;
            RpcAddItemTrue();
            isChangeValue = true;
        }
        else
        {
            foreach (var obj in LobbyDatas.instance.objList)
            {
                if (obj.name == _itemName)
                {
                    _itemAmount = obj.currentAmount;
                    RpcAddItemTrue();
                    isChangeValue = true;
                }
            }
        }

        isChangeAddData = false;
    }

    [ClientRpc]
    private void RpcAddItemFalse()
    {
        boxViewAndSound.AddItem(_itemAmount);
        boxViewAndSound.SetActiveForOriginal(false);
        boxViewAndSound.SetActiveForModifiable(false);
    }
    [ClientRpc]
    private void RpcAddItemTrue()
    {
        boxViewAndSound.AddItem(_itemAmount);
        boxViewAndSound.SetActiveForOriginal(false);
        boxViewAndSound.SetActiveForModifiable(true);
    }

    private void Update()
    {
        if (isServer)
        {
            if (InGameHud.instance != null)
            {
                if (InGameHud.instance.isOkNextDay && !isChangeAddData && _itemAmount>0)
                {
                    ServerDataSaves();
                }
            }
            if (!isChangeValue && LobbyDatas.instance != null)
            {
                ServerStart();
            }
        }
    }

    [Server]
    public void ServerAddAmount()
    {
        RpcAddAmount();
    }
    [ClientRpc]
    private void RpcAddAmount()
    {
        Amount.text = _itemAmount.ToString();
    }

    public void GiveItem(int _amount, GameObject box)
    {
        if (box.GetComponent<ObjInteract>().itemName == _itemName)
        {
            int newAmount = _maxItemAmount - _itemAmount;
            if (newAmount > _amount)
            {
                box.GetComponent<ObjInteract>().GiveBox(_amount);
                _itemAmount += _amount;
                boxViewAndSound.AddItem(_amount);
            }
            else
            {
                box.GetComponent<ObjInteract>().GiveBox(newAmount);
                _itemAmount += newAmount;
                boxViewAndSound.AddItem(_amount);
            }
            Amount.text = _itemAmount.ToString();
        }
    }

    public void ServerAmountChange(int value)
    {
        _itemAmount -= value;
        Amount.text = _itemAmount.ToString();
        boxViewAndSound.TakeItems(value);
    }
    public void ServerAmountChangeAdd(int value)
    {
        _itemAmount += value;
        Amount.text = _itemAmount.ToString();
        boxViewAndSound.AddItem(value);
    }
    [Server]
    private void ServerDataSaves()
    {
        RpcDataSaves();
    }

    [ClientRpc]
    private void RpcDataSaves()
    {
        if (_itemName != "Tray Box")
        {
            ObjData objToRemove = null;

            foreach (ObjData obj in LobbyDatas.instance.objList)
            {
                if (obj.name == _itemName)
                {
                    objToRemove = obj;
                    break;
                }
            }

            if (objToRemove != null)
            {
                LobbyDatas.instance.objList.Remove(objToRemove);
            }

            LobbyDatas.instance.objList.Add(new ObjData(_itemName, gameObject, _itemAmount));
            isChangeAddData = true;
        }
    }
}
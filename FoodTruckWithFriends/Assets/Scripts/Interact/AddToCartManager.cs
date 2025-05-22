using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class Item
{
    public string name;
    public int saleAmount;
    public int currentAmount;
    public int TotalCardAmount;

    public Item(string name, int saleAmount, int currentAmount, int totalCardAmount)
    {
        this.name = name;
        this.saleAmount = saleAmount;
        this.currentAmount = currentAmount;
        this.TotalCardAmount = totalCardAmount;
    }
}

public class AddToCartManager : NetworkBehaviour
{
    public static AddToCartManager instance;
    public GameManager _gameManager;
    public ProductItem _productItem;
    public List<Item> ItemCardItemList = new List<Item>();

    public ProductItem[] itemsProductItem;

    //Buy item
    public Transform SpawnItemLocation;
    public List<VisualElement> BuyCardItemList = new List<VisualElement>();
    public List<GameObject> BuyProductItemList = new List<GameObject>();

    [Header("Buying")] public Button BuyButton;

    [SyncVar] public bool isActiveBuyinButton;

    //[SyncVar] public int CardCount;
    public TMP_Text BuyCartAmountText;
    [SyncVar] public int BuyCartAmount;
    [SyncVar] public bool isBuying;


    [Header("Item Spawn")] public BoxSpawnerManager SpawnBoxLocation;

    public GameObject KargoTruckPrefab;
    public Transform StartTruckPos;

    void Start()
    {
        instance = this;
        if (isServer)
        {
            ServerBuyProductItemList();
        }
    }

    void Update()
    {
        if (isServer)
        {
            ServerBuyingCartItemsText();
        }
    }

    [Server]
    private void ServerBuyingCartItemsText()
    {
        for (int i = 0; i < BuyCardItemList.Count; i++)
        {
            if (BuyCardItemList[i] == null)
            {
                BuyCardItemList.RemoveAt(i);
            }
        }
    }

    [Server]
    public void ServerBuyProductItemList()
    {
        // for (int i = 0; i < itemsProductItem.Length; i++)
        // {
        //     itemsProductItem[i].queue += 1;
        // }
    }

    public void BuyingCartItems(int Value)
    {
        _productItem._totalAmount += Value;
    }

    //[Server]
    public void DeleteCartItems(int Value)
    {
        _productItem._totalAmount -= Value;
    }

    //[Server]
    public void DeleteBuyProductItems(string Value)
    {
        for (int i = 0; i < BuyProductItemList.Count; i++)
        {
            if (BuyProductItemList[i].GetComponent<ObjInteract>().itemName == Value)
            {
                BuyProductItemList.RemoveAll(productItemList =>
                    productItemList.GetComponent<ObjInteract>().itemName == Value);
            }
        }
    }

    // buttonlar Buy Menusü
    public void LearnedBuyRecipe(ScrollView CartList, int buyRecipe)
    {
        // para eksilmesi ve siparişin gelmesi
        _gameManager.MoneyAddAndRemove(false, buyRecipe, false, CartList);
    }

    //[Server]
    public void ServerBuyBoxItemList(ScrollView CartList)
    {
        if (BuyProductItemList.Count <= 3)
        {
            // para eksilmesi ve siparişin gelmesi
            _gameManager.MoneyAddAndRemove(false, _productItem._totalAmount, true, CartList);
        }
    }

    //[Server]
    public void ServerBuyBoxSpawn(ScrollView CartList)
    {
        foreach (GameObject obj in BuyProductItemList)
        {
            GameObject BoxObj = Instantiate(obj);
            NetworkServer.Spawn(BoxObj);
            RpcBuyBoxItemList(BuyProductItemList, BoxObj);
        }

        BuyProductItemList.Clear();
        ItemCardItemList.Clear();
        BuyCardItemList.Clear();
        CartList.Clear();
        _productItem._totalAmount = 0;

        // truck  spawn Olur ve çalışmaya başlar
        GameObject spawnTruck = Instantiate(KargoTruckPrefab);
        NetworkServer.Spawn(spawnTruck);
        RpcTruckSpawn(spawnTruck);
    }

    [ClientRpc]
    private void RpcBuyBoxItemList(List<GameObject> BuyProductItemList, GameObject boxObj)
    {
        //SpawnBoxLocation.ItemListAdd(BuyProductItemList);
        SpawnBoxLocation.Items.Add(boxObj);
        SpawnBoxLocation.ServerItemListAdd();
    }

    [ClientRpc]
    private void RpcTruckSpawn(GameObject obj)
    {
        obj.transform.position = StartTruckPos.position;
        // BuyButton.gameObject.SetActive(false);
    }

    [Server]
    public void ServerBuyButtonSettings(bool value)
    {
        ClientBuyButtonSettings(value);
    }

    [ClientRpc]
    private void ClientBuyButtonSettings(bool value)
    {
        if (value)
        {
            BuyButton.enabled = false;
        }
        else
        {
            BuyButton.enabled = true;
        }
    }
}
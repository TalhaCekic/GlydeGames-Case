using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class RegisterItem
{
    public string name;
    public int saleAmount;
    public int currentAmount;
    public int TotalCardAmount;

    public RegisterItem(string name, int saleAmount,int currentAmount,int totalCardAmount)
    {
        this.name = name;
        this.saleAmount = saleAmount;
        this.currentAmount = currentAmount;
        this.TotalCardAmount = totalCardAmount;
    }
}

public class RegisterAddToCartManager : NetworkBehaviour
{
    public static RegisterAddToCartManager instance;
    public RegisterItemProduct _registerItemProduct;
    public CustomerManager _customerManager;
    public List<RegisterItem> ItemCardItemList = new List<RegisterItem>();

    public ProductItem[] itemsProductItem;

    //Buy item
    public Transform SpawnItemLocation;
    public List<VisualElement> BuyCardItemList = new List<VisualElement>();
    public List<GameObject> BuyProductItemList = new List<GameObject>();

    [Header("Buying")] public TMP_Text BuyCartAmountText;
    [SyncVar] public int BuyCartAmount;

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
        //BuyCartAmountText.text = BuyCartAmount.ToString();
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
        for (int i = 0; i < itemsProductItem.Length; i++)
        {
            //itemsProductItem[i].queue += 1;
        }
    }

    [Server]
    public void BuyingCartItems(int Value)
    {
        BuyCartAmount += Value;
    }

    [Server]
    public void DeleteCartItems(int Value)
    {
        BuyCartAmount -= Value;
    }
    
    // bazı parametrelerin değişimi
    [Server]
    public void buycardAmountChange(Label totalAmountText) // müşteri ürün onaylaması yapılınca müşteri içerisinde çağırlır
    {
        _customerManager.CashRegisterQueue.Remove(CustomerManager.instance.CashRegisterQueue[0]);
        //GameManager.instance.MoneyAddAndRemove(true, BuyCartAmount);
        _registerItemProduct._totalAmount = 0;
        totalAmountText.text = _registerItemProduct._totalAmount.ToString();
    }

    // buttonlar
    public void ServerBuyCustomerItemList(ScrollView cartView,Label totalAmountText)
    {
        if (_customerManager.CashRegisterQueue.Count > 0)
        {
            if (_customerManager.CashRegisterQueue[0]!=null)
            {
                _customerManager.CashRegisterQueue[0].GetComponent<Customer>().CashRegisterProduct(gameObject,cartView,totalAmountText); 
            }
            else
            {
                print("Musteri yok");
            }
        }
        else
        {
            print("hiç müşteri yok");
        }
    }
    

}
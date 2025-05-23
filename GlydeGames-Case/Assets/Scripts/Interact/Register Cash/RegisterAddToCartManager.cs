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
    public List<VisualElement> BuyCardItemList = new List<VisualElement>(); // 12
    public List<GameObject> BuyProductItemList = new List<GameObject>();

    [SyncVar] public bool isFood, isDrink, isSnack; 

    void Start()
    {
        instance = this;
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
    
    // bazı parametrelerin değişimi
    //[Server]
    public void buycardAmountChange(Label totalAmountText) // müşteri ürün onaylaması yapılınca müşteri içerisinde çağırlır
    {
        GameManager.instance.MoneyAddAndRemove(true, _registerItemProduct._totalAmount, false, new ScrollView());
        _registerItemProduct._totalAmount = 0;
        totalAmountText.text = _registerItemProduct._totalAmount.ToString();
    }

    // buttonlar
    public void ServerBuyCustomerItemList(ScrollView cartView,Label totalAmountText)
    {
        if (_customerManager.OrderStayQueue.Count > 0)
        {
            if (_customerManager.OrderStayQueue[0]!=null)
            {
                _customerManager.OrderStayQueue[0].GetComponent<Customer>().ServerOrderValueChange(_registerItemProduct._totalAmount); 
                _customerManager.OrderStayQueue[0].GetComponent<Customer>().CashRegisterProduct(gameObject,cartView,totalAmountText); 
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
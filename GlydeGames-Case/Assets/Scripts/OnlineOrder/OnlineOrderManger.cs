using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class OnlineOrderItem
{
    public string itemName;
    public string MealName;
    public string DrinkName;
    public string SnackName;
    public int OrderNo;
    public int SellValue;

    public OnlineOrderItem(string name, string mealName, string drinkName, string snackName, int orderNo, int sellValue)
    {
        itemName = name;
        MealName = mealName;
        DrinkName = drinkName;
        SnackName = snackName;
        OrderNo = orderNo;
        SellValue = sellValue;
    }

    public override bool Equals(object obj)
    {
        if (obj is OnlineOrderItem)
        {
            var other = (OnlineOrderItem)obj;
            return this.itemName == other.itemName;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return itemName.GetHashCode();
    }
}

public class OnlineOrderManger : NetworkBehaviour
{
    public OnlineOrderCanvas OnlineOrderCanvas;
    [SyncVar] public int OrderCount;

    public Datas RecipeData;

    public List<OnlineOrderItem> orderItems = new List<OnlineOrderItem>();
    public List<OnlineDoors> onlineDoors = new List<OnlineDoors>();

    [SyncVar] public float OrderSelecetDelay;

    public BurgerScreen _BurgerScreen;
    public SnackScreen _SnackScreen;



    private void Update()
    {
        if (!isServer) return;
        if (GameManager.instance.isOnlineUpdate && GameManager.instance.isShopOpen)
        {
            ServerSelecetOrderSystem();
        }
        if ( GameManager.instance.isShopOpen)
        {
            ServerSelecetOrderSystemtest();
        }

        //ServerSelecetOrderSystem();
    }

    [Server]
    private void ServerSelecetOrderSystemtest()
    {
        OrderSelecetDelay += Time.deltaTime;
        if (orderItems.Count < 1)
        {
            
            //Statistic
            DailyStatistics.instance.ServerCustomerCountAdd(1);
            ServerSelectOrder();
            OrderSelecetDelay = 0;
        }
        
    }
    [Server]
    private void ServerSelecetOrderSystem()
    {
        OrderSelecetDelay += Time.deltaTime;
        if (OrderSelecetDelay > 100)
        {
            //Statistic
            DailyStatistics.instance.ServerCustomerCountAdd(1);
            ServerSelectOrder();
            OrderSelecetDelay = 0;
        }
    }

    // Sipariş seç (Server)
    void ServerSelectOrder()
    {
        OrderCount = Random.Range(1, RecipeData._MenuDatas.Length + 1);
        for (int i = 0; i < 1; i++)
        {
            int OrderSelectName = Random.Range(0, RecipeData._MenuDatas.Length);
            int OrderOnlineNumber = Random.Range(0, onlineDoors.Count);
            int OrderSelectNumber = 1;
            AddOrderItem(RecipeData._MenuDatas[OrderSelectName]._name, RecipeData._MenuDatas[OrderSelectName]._MealName,
                RecipeData._MenuDatas[OrderSelectName]._DrinkName, RecipeData._MenuDatas[OrderSelectName]._SnackName,
                OrderOnlineNumber,OrderSelectName);
        }
    }

    [ClientRpc] // liste ekleme clientler için
    public void AddOrderItem(string itemName, string mealName, string drinkName, string snackName, int orderNo,int orderNumber)
    {
        var newItem = new OnlineOrderItem(itemName, mealName, drinkName, snackName, onlineDoors[orderNo].DoorNo,RecipeData._MenuDatas[orderNumber]._SellValue);
        onlineDoors[orderNo].SelectOrders.Add(new OnlineOrderItem(itemName, mealName, drinkName, snackName, onlineDoors[orderNo].DoorNo,RecipeData._MenuDatas[orderNumber]._SellValue));

        OnlineOrderCanvas.AddViewElement(itemName, onlineDoors[orderNo].DoorNo);

        _BurgerScreen.AddCart(mealName,true,onlineDoors[orderNo].DoorNo);
        _SnackScreen.AddCart(drinkName,true,onlineDoors[orderNo].DoorNo);
        _SnackScreen.AddCart(snackName,true,onlineDoors[orderNo].DoorNo);
        
        orderItems.RemoveAll(item => item.Equals(newItem));

        orderItems.Add(newItem);
    }
}
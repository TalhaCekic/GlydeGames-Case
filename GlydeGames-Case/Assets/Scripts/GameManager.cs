using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;


public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public AddToCartManager addToCartManager;
    public DailyStatistics dailyStatistics;

    [SyncVar] public bool isShopOpen;
    [SyncVar] public bool isDayOn;

    //Parasal işlemler
    [Header("Moneys System")] [SyncVar] public int Money;
    public TMP_Text MoneyText;
    [SyncVar] public int FirstMoney;


    void Start()
    {
        instance = this;

        if (isServer)
        {
            ServerStart();
        }
    }

    [Server]
    private void ServerStart()
    {
        Money = FirstMoney;
        RpcStart();
    }

    [ClientRpc]
    private void RpcStart()
    {
        MoneyText.text = Money.ToString();
    }

    private void Update()
    {
        if (isServer)
        {
            ServerUpdate();
        }
    }

    [Server]
    public void ServerUpdate()
    {
        RpcUpdate();
        RpcMoneyText();
    }

    [ClientRpc]
    public void RpcUpdate()
    {
        //RpcDayPanel();
    }

    public void RpcDayPanel(bool isDayOn)
    {
        dailyStatistics.DayPanel(isDayOn);
        if (isDayOn)
        {
            dailyStatistics.DayPanelAddText();
        }
    }


    // [Server]
    public void MoneyAddAndRemove(bool value, int Value1, bool isSpawn, ScrollView CartList)
    {
        if (value)
        {
            Money += Value1;
            print(Value1);
            // statistic
            DailyStatistics.instance.ServerMoneyEarnedCountAdd(Value1);
        }

        if (!value && Money - Value1 >= 0)
        {
            Money -= Value1;
            // statistic
            DailyStatistics.instance.ServerMoneyEarnedCountAdd(-Value1);

            if (isSpawn)
            {
                // spawnlama işlemi
                addToCartManager.ServerBuyBoxSpawn(CartList);
            }
        }
    }

    [Server]
    public void ShopState(Button StateButton)
    {
        isShopOpen = !isShopOpen;
        StateChange(StateButton);
    }

    [Server]
    public void ServerDayState(bool value)
    {
        isDayOn = value;
    }

    public void StateChange(Button StateButton)
    {
        if (isShopOpen)
        {
            StateButton.text = "Shop Open";
            StateButton.style.backgroundColor = Color.green;
        }
        else
        {
            StateButton.text = "Shop Close";
            StateButton.style.backgroundColor = Color.red;
        }
    }

    [ClientRpc]
    public void RpcMoneyText()
    {
        MoneyText.text = Money.ToString();
    }
}
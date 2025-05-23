using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines.Primitives;
using Mirror;
using TMPro;
using UnityEngine;

public class DailyStatistics : NetworkBehaviour
{
    public static DailyStatistics instance;

    [Header("Day of Obj")] public GameObject DayOffManager;
    public bool isDayPanel;

    [Header("Incoming Customers")] public TMP_Text IncomingCustomersCountText;
    [SyncVar] public int IncomingCustomersCount;

    [Header("Money Earned")] public TMP_Text MoneyEarnedText;
    [SyncVar] public float MoneyEarnedCount;

    [Header("Customer Satisfaction")] 
    public TMP_Text CustomerSatisfactionHappyText;
    [SyncVar] public int CustomerSatisfactionHappyCount;
    public TMP_Text CustomerSatisfactionNormalText;
    [SyncVar] public int CustomerSatisfactionNormalCount;
    public TMP_Text CustomerSatisfactionSadText;
    [SyncVar] public int CustomerSatisfactionSadCount;

    private void Start()
    {
        instance = this;
    }

    [Server]
    public void ServerCustomerCountAdd(int Value)
    {
        IncomingCustomersCount += Value;
    }

    [Server]
    public void ServerMoneyEarnedCountAdd(float Value)
    {
        MoneyEarnedCount += Value;
    }
    [Server]
    public void ServerMoneyEarnedCountRemove(float Value)
    {
        MoneyEarnedCount -= Value;
    }

    [Server]
    public void ServerCustomerSatisfactionCountAdd(int HappyValue, int NormalValue, int SadValue)
    {
        CustomerSatisfactionHappyCount += HappyValue;
        CustomerSatisfactionNormalCount += NormalValue;
        CustomerSatisfactionSadCount += SadValue;
    }

    public void DayPanel(bool value)
    {
        isDayPanel = value;
        DayOffManager.SetActive(isDayPanel);
    }

    [ClientRpc]
    public void DayPanelAddText()
    {
        IncomingCustomersCountText.text = IncomingCustomersCount.ToString();
        MoneyEarnedText.text = MoneyEarnedCount.ToString();
        CustomerSatisfactionHappyText.text = CustomerSatisfactionHappyCount.ToString();
        CustomerSatisfactionNormalText.text = CustomerSatisfactionNormalCount.ToString();
        CustomerSatisfactionSadText.text = CustomerSatisfactionSadCount.ToString();
    }

    public void IsdayPanelFalse()
    {
        isDayPanel = false;
    }
}
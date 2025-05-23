using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameHud : NetworkBehaviour
{
    public static InGameHud instance;
    public DailyStatistics _DailyStatistics;
    public GameManager _GameManager;
    public dayManager _DayManager;
    [Header("Doc")]
    public UIDocument _document;

    [Header("Active Scene")]
    private Label TimerHour;
    private Label TimerMinute;
    private Label Money;
    private Label MinimumMoney;
    private Label Day;

    [Header("Statistic Used")]
    private VisualElement StatisticBg;
    private Label DayLabel;
    private Label InComingCustomersLabel;
    private Label MoneyEarnedLabel;

    private Label CustomerSatisfactionHappyText;
    private Label CustomerSatisfactionNormalText;
    private Label CustomerSatisfactionSadText;
    
    [Header("Statistic Don't Use")]
    private Label DayLabel_DNT;
    private Label InComingCustomersLabel_DNT;
    private Label MoneyEarnedLabel_DNT;
    private Label CustomerStatistic_DNT;

    private VisualElement CustomerSatisfactionHappy_DNT;
    private VisualElement CustomerSatisfactionNormal_DNT;
    private VisualElement CustomerSatisfactionSad_DNT;

    private Button NextDayButton;
    
    [Header("Last Day")]
    private VisualElement LastDayBg;
    private VisualElement LastDayColorBg;
    private Label LastDayLabel;
    
    [Header("Crosshair")]
    private VisualElement Crosshair;
    
    [Header("Values")][SyncVar] public bool isStatistic;
    [SyncVar] public bool isOkNextDay;
    

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        TimerHour = _document.rootVisualElement.Q("TimerHour_Label") as Label;
        TimerMinute = _document.rootVisualElement.Q("TimerMinute_Label") as Label;
        Money = _document.rootVisualElement.Q("Money_Label") as Label;
        MinimumMoney = _document.rootVisualElement.Q("MinimumMoney_Label") as Label;
        Day = _document.rootVisualElement.Q("Day") as Label;
        
        Crosshair = _document.rootVisualElement.Q("Cross");
    }

    private void OnEnable()
    {
        NextDayButton = _document.rootVisualElement.Q("NextDay_Button") as Button;

        NextDayButton.clicked += NextDayButtonClicked;

    }

    private void OnDisable()
    {
        NextDayButton.clicked -= NextDayButtonClicked;
    }

    private void Start()
    {
        instance = this;
        _document = GetComponent<UIDocument>();

        ServerUpdate();
    }

    private void Update()
    {
        if (isServer)
        {
            if (!isStatistic)
            {
                ServerStatisticCheck(false);
            }
            else
            {
                ServerStatisticCheck(true);
            }
        }
    }

    [Server]
    private void ServerUpdate()
    {
        isStatistic = false;
    }

    private void NextDayButtonClicked()
    {
        ServerNexDayButton();
    }
    
    // [Server]
    // public void ServerCross(bool value)
    // {
    //     RpcCross(value);
    // }
    //[TargetRpc]
    public void RpcCross(bool value)
    {
        Crosshair = _document.rootVisualElement.Q("Cross");
        if (value)
        {
            Crosshair.style.display = DisplayStyle.None;
        }
        else
        {
            Crosshair.style.display = DisplayStyle.Flex;
        }
    }
    [Server]
    private void ServerNexDayButton()
    {
        RpcNexDayButton();
    }
    [ClientRpc]
    private void RpcNexDayButton()
    {
        if (isOkNextDay)
        {
            CustomNetworkManager.instance.LastDay++;
            CustomNetworkManager.instance.LastMoney = _GameManager.Money;
            SteamLobby.instance.Restart();
            isOkNextDay = false;
        }
        else
        {
            CustomNetworkManager.instance.LastDay = 0;
            CustomNetworkManager.instance.LastMoney = 0;
            SteamLobby.instance.leaving();
        }
    }

    [Server]
    public void ServerMoneyWrite(float value)
    {
        RpcMoneyWrite(value);
    }
    [ClientRpc]
    private void RpcMoneyWrite(float value)
    {
        Money = _document.rootVisualElement.Q("Money_Label") as Label;
        Money.text = value.ToString();
    }
    [Server]
    public void ServerMoneyColorWrite(float money, float minMoney)
    {
        RpcMoneyColorWrite(money, minMoney);
    }
    [ClientRpc]
    private void RpcMoneyColorWrite(float money, float minMoney)
    {
        Money = _document.rootVisualElement.Q("Money_Label") as Label;
        MinimumMoney = _document.rootVisualElement.Q("MinimumMoney_Label") as Label;

        if (minMoney > money)
        {
            Money.style.color = Color.red;
        }
        else
        {
            Money.style.color = Color.green;
        }
    }

    [Server]
    public void ServerMinimumMoneyWrite(float value)
    {
        RpcMinimumMoneyWrite(value);
    }
    [ClientRpc]
    private void RpcMinimumMoneyWrite(float value)
    {
        MinimumMoney = _document.rootVisualElement.Q("MinimumMoney_Label") as Label;
        MinimumMoney.text = value.ToString();
        MinimumMoney.style.color = Color.green;
    }
    [Server]
    public void ServerTimeHourWrite(string HourValue)
    {
        RpcTimeHourWrite(HourValue);
    }
    [ClientRpc]
    private void RpcTimeHourWrite(string HourValue)
    {
        TimerHour = _document.rootVisualElement.Q("TimerHour_Label") as Label;
        TimerHour.text = HourValue;
    }
    [Server]
    public void ServerTimeMinuteWrite(string MinuteValue)
    {
        RpcTimeMinuteWrite(MinuteValue);
    }
    [ClientRpc]
    private void RpcTimeMinuteWrite(string MinuteValue)
    {
        TimerMinute = _document.rootVisualElement.Q("TimerMinute_Label") as Label;
        TimerMinute.text = MinuteValue;
    }

    [Server]
    public void ServerTimeDayWrite(int value,int nextValue)
    {
        RpcTimeDayWrite(value, nextValue);
    }
    [ClientRpc]
    private void RpcTimeDayWrite(int value,int nextValue)
    {
        Day = _document.rootVisualElement.Q("Day") as Label;
        Day.text = value.ToString() + "/" + nextValue.ToString();
    }

    // Statistic
    [Server]
    public void ServerStatisticCheck(bool value)
    {
        RpcStatisticCheck(value);
    }
    [ClientRpc]
    private void RpcStatisticCheck(bool value)
    {
        _document = GetComponent<UIDocument>();

        StatisticBg = _document.rootVisualElement.Q("StatisticElement");
        LastDayBg = _document.rootVisualElement.Q("WinAndLoseElement");
        if (value)
        {
            StatisticBg.style.display = DisplayStyle.Flex;
            StatisticData();
        }
        else
        {
            StatisticBg.style.display = DisplayStyle.None;
            LastDayBg.style.display = DisplayStyle.None;
        }
    }
    private void StatisticData()
    {
        // USED
        DayLabel = _document.rootVisualElement.Q("Day_Label") as Label;
        InComingCustomersLabel = _document.rootVisualElement.Q("InComingCustomers_Label") as Label;
        MoneyEarnedLabel = _document.rootVisualElement.Q("MoneyEarned_Label") as Label;

        CustomerSatisfactionHappyText = _document.rootVisualElement.Q("HappyAmount_Label") as Label;
        CustomerSatisfactionNormalText = _document.rootVisualElement.Q("NormalAmount_Label") as Label;
        CustomerSatisfactionSadText = _document.rootVisualElement.Q("SadAmount_Label") as Label;

        DayLabel.text = _DayManager.day.ToString();
        DayLabel.AddToClassList("textRightToLeftReset");
        DayLabel.RemoveFromClassList("textRightToLeft");
        
        InComingCustomersLabel.text = _DailyStatistics.IncomingCustomersCount.ToString();
        InComingCustomersLabel.AddToClassList("textRightToLeftReset");
        InComingCustomersLabel.RemoveFromClassList("textRightToLeft");
        
        MoneyEarnedLabel.text = _DailyStatistics.MoneyEarnedCount.ToString();
        MoneyEarnedLabel.AddToClassList("textRightToLeftReset");
        MoneyEarnedLabel.RemoveFromClassList("textRightToLeft");

        CustomerSatisfactionHappyText.text = _DailyStatistics.CustomerSatisfactionHappyCount.ToString();
        CustomerSatisfactionHappyText.AddToClassList("Scale-OpacityReset");
        CustomerSatisfactionHappyText.RemoveFromClassList("Scale-Opacity");
        
        CustomerSatisfactionNormalText.text = _DailyStatistics.CustomerSatisfactionNormalCount.ToString();
        CustomerSatisfactionNormalText.AddToClassList("Scale-OpacityReset");
        CustomerSatisfactionNormalText.RemoveFromClassList("Scale-Opacity");
        
        CustomerSatisfactionSadText.text = _DailyStatistics.CustomerSatisfactionSadCount.ToString();
        CustomerSatisfactionSadText.AddToClassList("Scale-OpacityReset");
        CustomerSatisfactionSadText.RemoveFromClassList("Scale-Opacity");
        
        // DON'T USED
        DayLabel_DNT = _document.rootVisualElement.Q("Day-") as Label;
        DayLabel_DNT.AddToClassList("textLeftToRightReset");
        DayLabel_DNT.RemoveFromClassList("textLeftToRight");
        
        InComingCustomersLabel_DNT = _document.rootVisualElement.Q("InCominCustomers-") as Label;
        InComingCustomersLabel_DNT.AddToClassList("textLeftToRightReset");
        InComingCustomersLabel_DNT.RemoveFromClassList("textLeftToRight");
        
        MoneyEarnedLabel_DNT = _document.rootVisualElement.Q("MoneyEarned-") as Label;
        MoneyEarnedLabel_DNT.AddToClassList("textLeftToRightReset");
        MoneyEarnedLabel_DNT.RemoveFromClassList("textLeftToRight");
        
        CustomerStatistic_DNT = _document.rootVisualElement.Q("CustomerStatistic-") as Label;
        CustomerStatistic_DNT.AddToClassList("Scale-OpacityReset");
        CustomerStatistic_DNT.RemoveFromClassList("Scale-Opacity");
        
        CustomerSatisfactionHappy_DNT = _document.rootVisualElement.Q("HappyEmojiBg");
        CustomerSatisfactionHappy_DNT.AddToClassList("HappySystemDownReset");
        CustomerSatisfactionHappy_DNT.RemoveFromClassList("HappySystemDown");
        
        CustomerSatisfactionNormal_DNT = _document.rootVisualElement.Q("NormalEmojiBg");
        CustomerSatisfactionNormal_DNT.AddToClassList("HappySystemDownReset");
        CustomerSatisfactionNormal_DNT.RemoveFromClassList("HappySystemDown");
        
        CustomerSatisfactionSad_DNT = _document.rootVisualElement.Q("SadEmojiBg");
        CustomerSatisfactionSad_DNT.AddToClassList("HappySystemDownReset");
        CustomerSatisfactionSad_DNT.RemoveFromClassList("HappySystemDown");       
        
        NextDayButton.AddToClassList("NextLeftToRightReset");
        NextDayButton.RemoveFromClassList("NextLeftToRight");
        
        LastDayBg = _document.rootVisualElement.Q("WinAndLoseElement");
        LastDayColorBg = _document.rootVisualElement.Q("BgColor");
        LastDayLabel = _document.rootVisualElement.Q("LastDay_Label") as Label;
        
        switch (_GameManager._dayManager.day)
        {
            case 3:
                LastDayBg.style.display = DisplayStyle.Flex;
                LastDayColorBg.style.display = DisplayStyle.Flex;
                LastDayColorBg.AddToClassList("WinAndLoseUpReset");
                if (_GameManager.Money > _GameManager.minMoney)
                {
                    LastDayLabel.text = "You can move on to the next day because you have met the company's request.";
                    LastDayColorBg.style.unityBackgroundImageTintColor = Color.green;
                    isOkNextDay = true;
                }
                else
                {
                    LastDayColorBg.style.unityBackgroundImageTintColor = Color.red;
                    LastDayLabel.text = "Since you can't meet the company's demand, you will start over from the beginning.";
                }
                break;
            case 6:
                if (_GameManager.Money > _GameManager.minMoney) isOkNextDay = true;
                break;
            case 9:
                if (_GameManager.Money > _GameManager.minMoney) isOkNextDay = true;
                break;
            case 12:
                if (_GameManager.Money > _GameManager.minMoney) isOkNextDay = true;
                break;
            case 15:
                if (_GameManager.Money > _GameManager.minMoney) isOkNextDay = true;
                break;
            default:
                isOkNextDay = true;
                break;
        }

    }

}

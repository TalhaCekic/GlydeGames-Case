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
    public dayManager _dayManager;
    public InGameHud _inGameHud;

    [SyncVar] public bool isShopOpen;
    [SyncVar] public bool isDayOn;

    //Parasal işlemler
    [Header("Moneys System")] [SyncVar] public float Money;
    public TMP_Text MoneyText;
    [SyncVar] public int FirstMoney;

    [SyncVar] public float minMoney;

    [Header("Update Value")] 
    [Header("Online Service")]
    [SyncVar] public bool isOnlineUpdate;
    public GameObject CarObj;
    public Transform SpawnCarObjTrans;
    private GameObject spawnCar;


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
       // if (_dayManager.day == 0)
       // {
       //     Money = FirstMoney;
       //     print(Money);
       // }
       // else
      //  {
         //   Money = CustomNetworkManager.instance.LastMoney;
         //   print(CustomNetworkManager.instance.LastMoney);
       // }
        Money = CustomNetworkManager.instance.LastMoney;

        RpcStart();
        ServerSpawnCar();
    }

    //[Server]
    private void ServerSpawnCar()
    {
        if (LobbyDatas.instance != null)
        {
            if (LobbyDatas.instance.data.updateData[0]._active)
            {
                isOnlineUpdate = true;
                spawnCar = Instantiate(CarObj,SpawnCarObjTrans);
                NetworkServer.Spawn(spawnCar);
                RpcAddCar(spawnCar);
            }
        }
    }
    [ClientRpc]
    private void RpcStart()
    {
        _inGameHud.ServerMoneyWrite(Money);
        if (LobbyDatas.instance != null)
        {
            if (LobbyDatas.instance.data.updateData[0]._active)
            {
                isOnlineUpdate = true;
            }
        }
    }
    [Server]
    public void SpawnStartCar()
    {
        spawnCar = Instantiate(CarObj,SpawnCarObjTrans);
        NetworkServer.Spawn(spawnCar);
        RpcAddCar(spawnCar);
    }

    [ClientRpc]
    private void RpcAddCar(GameObject obj)
    {
        spawnCar = obj;
        //obj.transform.SetParent(SpawnCarObjTrans);
        //obj.transform.localPosition = SpawnCarObjTrans.position;
        obj.GetComponent<carMovement>().CarStartFunction(SpawnCarObjTrans);
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
        
        if (!isShopOpen)
        {
            if(!isDayOn && _dayManager.hour==23) _inGameHud.isStatistic = true;
        }
    }

    [ClientRpc]
    public void RpcUpdate()
    {
        //RpcDayPanel();
        //////
        // if (LobbyDatas.instance != null)
        // {
        //     if (LobbyDatas.instance.data.updateData[0]._active)
        //     {
        //         isOnlineUpdate = true;
        //     }
        // }
    }

    public void RpcDayPanel(bool isDayOn)
    {
        //dailyStatistics.DayPanel(isDayOn);
       // _inGameHud.ServerStatisticCheck(isDayOn);
        // if (isDayOn)
        // {
        //     dailyStatistics.DayPanelAddText();
        // }
    }
    //[Server]
    public void MoneyAddAndRemove(bool value, float Value1, bool isSpawn, ScrollView CartList)
    {
        if (value)
        {
            Money += Value1;
            // statistic
            DailyStatistics.instance.ServerMoneyEarnedCountAdd(Value1);
        }

        if (!value && Money - Value1 >= 0)
        {
            Money -= Value1;
            // statistic
            DailyStatistics.instance.ServerMoneyEarnedCountRemove(Value1);

            if (isSpawn)
            {
                // spawnlama işlemi
                if(CartList==null)return;
                addToCartManager.ServerBuyBoxSpawn(CartList);
            }
        }
    }

    //[Server]
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

   // [ClientRpc]
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
            if(!isDayOn && _dayManager.hour==23) _inGameHud.isStatistic = true;
        }
    }

    [ClientRpc]
    public void RpcMoneyText()
    {
        _inGameHud.ServerMoneyWrite(Money);
        switch (_dayManager.day)
        {
            case 1:
            case 2:
            case 3:
                minMoney = 1000;
                _inGameHud.ServerMinimumMoneyWrite(minMoney);
                break;
            case 4:
            case 5:
            case 6:
                minMoney = 3000;
                _inGameHud.ServerMinimumMoneyWrite(minMoney);
                break;
            case 7: 
            case 8:
            case 9:
                minMoney = 7000;
                _inGameHud.ServerMinimumMoneyWrite(minMoney);
                break;
            case 10: 
            case 11:
            case 12:
                minMoney = 10000;
                _inGameHud.ServerMinimumMoneyWrite(minMoney);
                break;
            case 13: 
            case 14:
            case 15:
                minMoney = 15000;
                _inGameHud.ServerMinimumMoneyWrite(minMoney);
                break;
        }
        
        // renk belirler
        _inGameHud.ServerMoneyColorWrite(Money, minMoney);
    }
}
using System;
using System.Collections.Generic;
using System.Xml.Schema;
using DG.Tweening;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[System.Serializable]
public class OrderItem
{
    public string itemName;
    public string MealName;
    public string DrinkName;
    public string SnackName;

    public OrderItem(string name, string mealName, string drinkName, string snackName)
    {
        itemName = name;
        MealName = mealName;
        DrinkName = drinkName;
        SnackName = snackName;
    }

    public override bool Equals(object obj)
    {
        if (obj is OrderItem)
        {
            var other = (OrderItem)obj;
            return this.itemName == other.itemName;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return itemName.GetHashCode();
    }
}

public class Customer : NetworkBehaviour
{
    public CustomerOrderPanel orderPanel;
    [Header("View")] [SyncVar] public int AppearanceValue;
    public List<GameObject> Appearance = new List<GameObject>();

    [Header("Orders")] [SyncVar] public bool WaitingForOrder;
    [SyncVar] public int OrderCount;
    public Datas RecipeData;
    public List<OrderItem> orderItems = new List<OrderItem>();

    [Header("Transforms")] public Transform OrderStayPos;
    public Transform Hand;

    [Header("Agent & Anim")] public Animator anims;
    [SyncVar] public bool isStop;
    [SerializeField] public NavMeshAgent CustomerAgent;
    [SyncVar] public bool isFinish;

    public Vector3 TargetPos;
    [SyncVar] public int customerQueue;
    [SyncVar] public bool isEat;
    [SyncVar] public bool isChair;
    [SyncVar] public bool isToilet;
    [SyncVar] public bool GoToToilet;
    public GameObject ChairSelected;
    public GameObject ToiletSelected;
    public Transform LeaveSelected;

    // oturup bekleme süresi
    [SyncVar] public float SitTime;
    [SyncVar] public float MaxSitTime;

    // tuvalet bekleme süresi
    [SyncVar] public float ToiletTime;
    [SyncVar] public float MaxToiletTime;

    // yeme süresi belirleme
    [SyncVar] public float eatTime;
    [SyncVar] public float MaxeatTime;

    [Header("Wait Time")] [SyncVar] public float OrderWaitTimer;

    [SyncVar] public float OrderWaitTimeHappy = 0;
    [SyncVar] public float OrderWaitNormal = 6;
    [SyncVar] public float OrderWaitSad = 11;

    [SyncVar] public float OrderWaitMax;
    [SyncVar] public float OrderWaitMin;

    [Header("Data")] [SerializeField] public DailyStatistics dailyStatistics;

    [SyncVar] public int HappySystem;

    public TrayInteract _TrayInteract;
    public Tray _Tray;

    public BurgerScreen _BurgerScreen;
    public SnackScreen _SnackScreen;

    [SyncVar] private bool Change1;
    [SyncVar] private bool Change2;
    [SyncVar] private bool Change3;

    [SyncVar] private float OrderValue;

    void Start()
    {
        dailyStatistics = DailyStatistics.instance;
        if (isServer)
        {
            ServerStart();
            ServerSelectGoPos();
            ServerGoToPos();
        }
    }

    [Server]
    private void ServerStart()
    {
        RpcStart();
        ServerAppiranceValue(); // görünüm ayarlama
        ServerSelectOrder(); // sipariş Seçimi
        //randomToilet(); // tuvalet durumu olacak mı
    }

    private void randomToilet()
    {
        if (CustomerManager.instance.Toilet.Count > 0)
        {
            int randomInt = Random.Range(5, 5);
            if (randomInt == 5)
            {
                isToilet = true;
                RandomToiletPosSelect();
            }
        }
        else
        {
            isToilet = false;
        }
    }

    [ClientRpc]
    private void RandomToiletPosSelect()
    {
        int randomToilet = Random.Range(0, CustomerManager.instance.Toilet.Count);
        ToiletSelected = CustomerManager.instance.Toilet[randomToilet];
        CustomerManager.instance.RemoveToilet(ToiletSelected);
    }

    [ClientRpc]
    private void RpcStart()
    {
        CustomerAgent = GetComponent<NavMeshAgent>();

        OrderStayPos = GameObject.FindWithTag("OrderStayPos").transform;
        TargetPos = OrderStayPos.position;
        isStop = true;

        GameObject BurgermonitorObj = GameObject.Find("BurgerMonitor");
        _BurgerScreen = BurgermonitorObj.GetComponent<BurgerScreen>();

        GameObject SnackmonitorObj = GameObject.Find("DrinkAndSnackMonitor");
        _SnackScreen = SnackmonitorObj.GetComponent<SnackScreen>();

        this.transform.SetParent(null);
    }

    // görünümü ata (Server)
    void ServerAppiranceValue()
    {
        AppearanceValue = Random.Range(0, Appearance.Count);
        RpcAppiranceValue(AppearanceValue);
    }

    [ClientRpc] //  (Rpc)
    void RpcAppiranceValue(int index)
    {
        Appearance[index].SetActive(true);
    }

    // Sipariş seç (Server)
    void ServerSelectOrder()
    {
        OrderCount = Random.Range(1, RecipeData._MenuDatas.Length + 1);
        for (int i = 0; i < 1; i++)
        {
            int OrderSelectName = Random.Range(0, RecipeData._MenuDatas.Length);
            //int OrderSelectNumber = Random.Range(1, 3);
            int OrderSelectNumber = 1;
            AddOrderItem(RecipeData._MenuDatas[OrderSelectName]._name, RecipeData._MenuDatas[OrderSelectName]._MealName,
                RecipeData._MenuDatas[OrderSelectName]._DrinkName, RecipeData._MenuDatas[OrderSelectName]._SnackName);
        }
    }

    [ClientRpc] // liste ekleme clientler için
    public void AddOrderItem(string itemName, string mealName, string drinkName, string snackName)
    {
        var newItem = new OrderItem(itemName, mealName, drinkName, snackName);

        orderItems.RemoveAll(item => item.Equals(newItem));

        orderItems.Add(newItem);
    }

    void Update()
    {
        if (isServer)
        {
            ServerGoToPos();
            isChairValue();
        }
    }

    [Server]
    private void ServerGoToPos()
    {
        if (TargetPos != null)
        {
            float stoppingDistance = Vector3.Distance(TargetPos, transform.position);
            if (stoppingDistance <= 0.5f)
            {
                if (isChair)
                {
                    CustomerAgent.isStopped = true;
                    anims.SetBool("isWalk", false);
                    isStop = true;
                }
            }
            else if (stoppingDistance <= 1.5f)
            {
                if (!isStop)
                {
                    CustomerAgent.isStopped = true;
                    anims.SetBool("isWalk", false);
                    isStop = true;
                }
            }
            else
            {
                if (isStop)
                {
                    WaitingForOrder = false;
                    CustomerAgent.isStopped = false;
                    CustomerAgent.SetDestination(TargetPos);
                    anims.SetBool("isSit", false);
                    anims.SetBool("isWalk", true);
                    isStop = false;
                }
            }

            // bitiş ve yok oluş
            if (isFinish)
            {
                if (stoppingDistance < 3f)
                {
                    CustomerManager.instance.CustomersList.Remove(gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }

    [Server]
    private void isChairValue()
    {
        if (isStop)
        {
            if (isChair || isEat)
            {
                OneFunction();
                //  yeme süresi belirleme
                if (isEat)
                {
                    if (eatTime > MaxeatTime)
                    {
                        OneFunction2();
                    }
                    else
                    {
                        eatTime += Time.deltaTime;
                    }
                }
                else
                {
                    //  bekleme süresi
                    if (SitTime > MaxSitTime)
                    {
                        OneFunction3();
                    }
                    else
                    {
                        SitTime += Time.deltaTime;
                        orderPanel.Slider.maxValue = MaxSitTime;
                        orderPanel.Slider.value = SitTime;
                    }
                }
            }
            else
            {
                orderPanel.Slider.maxValue = OrderWaitMin;
                orderPanel.Slider.value = OrderWaitTimer;
                if (OrderWaitTimer >= OrderWaitMin)
                {
                    RandomPosSelectto();
                }
            }
        }
    }

    private void OneFunction()
    {
        if (!Change1)
        {
            CustomerAgent.isStopped = true;
            RpcCustomPos();
            RpcRot();
            anims.SetBool("isSit", true);
            WaitingForOrder = true;
            Change1 = true;
        }
    }

    [ClientRpc]
    private void RpcCustomPos()
    {
        this.transform.position = ChairSelected.transform.position;
    }

    private void OneFunction2()
    {
        if (!Change2)
        {
            anims.SetBool("isEating", false); // yeme animasyonu olacak
            RandomPosSelectto();
            _TrayInteract.ServerStateChange(false);
            _TrayInteract.RpcChairAdd(ChairSelected);
            _Tray._name = null;
            isChair = false;
            isEat = false;
            //RpcName();
            isStop = true;
            Change2 = true;
        }
    }

    private void OneFunction3()
    {
        if (!Change3)
        {
            RpcName();
            RandomPosSelectto();
            GameManager.instance.MoneyAddAndRemove(false, OrderValue * 2, false, new ScrollView());
            CustomerManager.instance.AddChairs(ChairSelected);
            anims.SetBool("isSit", false);
            anims.SetBool("isWalk", true);
            isStop = true;
            Change3 = true;
        }
    }


    [ClientRpc]
    private void RpcName()
    {
        foreach (var names in orderItems)
        {
                _BurgerScreen.ListRemoveOnlineOff(names.MealName);
                _SnackScreen.ListRemoveOnlineOff(names.DrinkName);
                _SnackScreen.ListRemoveOnlineOff(names.SnackName);
        }
    }

    [ClientRpc]
    private void RpcRot()
    {
        transform.rotation = ChairSelected.transform.rotation;
    }

    [Server]
    public void ServerSelectGoPos() // start da çalışıyor
    {
        RpcSelectOrderStayGoPos();
    }

    [ClientRpc]
    public void RpcSelectOrderStayGoPos()
    {
        if (orderItems.Count > 0)
        {
            TargetPos = OrderStayPos.position + new Vector3(customerQueue, 0, 0);
        }
    }

    public void PlayerGiveProduct(GameObject PlayerHandObj, GameObject player) // sipariş teslimi
    {
        if (WaitingForOrder)
        {
            Tray packet = PlayerHandObj.GetComponent<Tray>();
            _TrayInteract = PlayerHandObj.GetComponent<TrayInteract>();
            _Tray = packet;
            foreach (var orderItem in orderItems)
            {
                if (packet._name == orderItem.itemName)
                {
                    switch (HappySystem)
                    {
                        case 3:
                            dailyStatistics.ServerCustomerSatisfactionCountAdd(0, 0, 1);
                            break;
                        case 2:
                            dailyStatistics.ServerCustomerSatisfactionCountAdd(0, 1, 0);
                            break;
                        case 1:
                            dailyStatistics.ServerCustomerSatisfactionCountAdd(1, 0, 0);
                            break;
                    }

                    // teslim sonrası player işlemi
                    player.GetComponent<PlayerInteract>().HandFullTest(false);
                    player.GetComponent<PlayerInteract>().isHandRightTrue = false;
                    player.GetComponent<PlayerInteract>().InteractObj = null;

                    // teslim sonrası player işlemi
                    PlayerHandObj.transform.SetParent(null);
                    PlayerHandObj.transform.localRotation = Quaternion.identity;
                    PlayerHandObj.GetComponent<TrayInteract>().isInteract = false;
                    PlayerHandObj.GetComponent<TrayInteract>().collider.enabled = false;
                    PlayerHandObj.GetComponent<Tray>().Trash();
                    PlayerHandObj.transform.DOMove(Hand.position, 0.6f)
                        .OnComplete(() => ComplitePlayerGiveProduct());
                }
            }
        }
    }

    private void ComplitePlayerGiveProduct()
    {
        RpcName();
        anims.SetBool("isEating", true); // yeme animasyonu olacak
        isEat = true;
    }

    // Cash System
    //[Server]
    public void CashRegisterProduct(GameObject CashObj, ScrollView BuyCardItemList, Label TotalAmountText)
    {
        RegisterAddToCartManager Cash = CashObj.GetComponent<RegisterAddToCartManager>();

        if (Cash.isFood && Cash.isDrink && Cash.isSnack)
        {
            RegisterAddToCartManager.instance.buycardAmountChange(TotalAmountText);

            Cash.ItemCardItemList.Clear();
            Cash.BuyCardItemList.Clear();
            Cash.BuyProductItemList.Clear();
            BuyCardItemList.Clear();
            Cash.isFood = false;
            Cash.isDrink = false;
            Cash.isSnack = false;

            CustomerManager.instance.OrderStayQueue.Remove(gameObject);
            int randomChair = Random.Range(0, CustomerManager.instance.Chairs.Count);
            CustomGoToChair(randomChair);
            // sipariş onaylanınca kartların silinmesi
            CustomerManager.instance.ServerCashQueueMove();
            CustomerManager.instance.ServerQueueMove();
        }
    }

    // [ClientRpc]
    private void CustomGoToChair(int randomChair)
    {
        ChairSelected = CustomerManager.instance.Chairs[randomChair];
        CustomerManager.instance.RemoveChairs(ChairSelected);

        TargetPos = ChairSelected.transform.position;

        isChair = true;
        isStop = true;
    }

    [Server]
    private void RandomPosSelectto()
    {
        RpcPanel();
        CustomerAgent.isStopped = false;

        TargetPos = LeaveSelected.position;
        isFinish = true;
        if (!isChair)
        {
            CustomerManager.instance.OrderStayQueue.Remove(gameObject);
            CustomerManager.instance.ServerQueueMoveRemove();
        }
    }

    [ClientRpc]
    private void RpcPanel()
    {
        orderPanel.Canvas.SetActive(false);
    }

    [Server]
    public void ServerCustomerTime()
    {
        OrderWaitTimer += Time.deltaTime;
        if (OrderWaitTimer < OrderWaitMin)
        {
            HappySystem = 1;
        }
        else if (OrderWaitTimer > OrderWaitMin && OrderWaitTimer < OrderWaitMax)
        {
            HappySystem = 2;
        }
        else if (OrderWaitTimer > OrderWaitMin && OrderWaitTimer > OrderWaitMax)
        {
            HappySystem = 3;
        }
    }

    [Server]
    public void ServerOrderValueChange(float valye)
    {
        OrderValue += valye;
    }
}
using System;
using System.Collections.Generic;
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
    public int quantity;

    public OrderItem(string name, int qty)
    {
        itemName = name;
        quantity = qty;
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
    [Header("View")] [SyncVar] public int AppearanceValue;
    public List<GameObject> Appearance = new List<GameObject>();

    [Header("Orders")] [SyncVar] public bool WaitingForOrder;
    [SyncVar] public int OrderCount;
    public List<OrderItem> orderItems = new List<OrderItem>();
    public ScribtableOrderItem scribtableOrderItem;

    [Header("Transforms")] public Transform OrderStayPos;
    public Transform CashRegisterStayPos;
    public Transform Hand;

    [Header("Agent & Anim")] public Animator anims;
    [SyncVar] public bool isStop;
    [SerializeField] public NavMeshAgent CustomerAgent;
    [SyncVar] public bool isFinish;

    public Vector3 TargetPos;
    [SyncVar] public int customerQueue;

    [Header("Random Pos")] public float minX = -5;
    public float maxX = 5f;
    public float minZ = -5f;
    public float maxZ = 5f;

    [Header("Wait Time")] [SyncVar] public float OrderWaitTimer;

    [SyncVar] public float OrderWaitTimeHappy = 0;
    [SyncVar] public float OrderWaitNormal = 6;
    [SyncVar] public float OrderWaitSad = 11;

    [SyncVar] public float OrderWaitMax;
    [SyncVar] public float OrderWaitMin;

    [Header("Data")] [SerializeField] public DailyStatistics dailyStatistics;

    [SyncVar] public int HappySystem;

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
        WaitingForOrder = true;
        RpcStart();
        ServerAppiranceValue(); // görünüm ayarlama
        ServerSelectOrder(); // sipariş Seçimi
    }

    [ClientRpc]
    private void RpcStart()
    {
        CustomerAgent = GetComponent<NavMeshAgent>();

        OrderStayPos = GameObject.FindWithTag("OrderStayPos").transform;
        CashRegisterStayPos = GameObject.FindWithTag("CashRegisterrStayPos").transform;
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
        OrderCount = Random.Range(1, scribtableOrderItem.OrderItemName.Length + 1);
        for (int i = 0; i < OrderCount; i++)
        {
            int OrderSelectName = Random.Range(0, scribtableOrderItem.OrderItemName.Length);
            int OrderSelectNumber = Random.Range(1, 3);
            AddOrderItem(scribtableOrderItem.OrderItemName[OrderSelectName], OrderSelectNumber);
        }
    }

    [ClientRpc] // liste ekleme clientler için
    public void AddOrderItem(string itemName, int quantity)
    {
        var newItem = new OrderItem(itemName, quantity);

        orderItems.RemoveAll(item => item.Equals(newItem));

        orderItems.Add(newItem);
    }

    void Update()
    {
        if (isServer)
        {
            ServerGoToPos();
        }
    }

    [Server]
    private void ServerGoToPos()
    {
        if (TargetPos != null)
        {
            float stoppingDistance = Vector3.Distance(TargetPos, transform.position);
            if (stoppingDistance <= 0.3f)
            {
                RpcTakeProduct(TargetPos);
                CustomerAgent.isStopped = true;
                anims.SetBool("isWalk", false);
                isStop = true;
            }
            else
            {
                CustomerAgent.isStopped = false;
                CustomerAgent.SetDestination(TargetPos);
                anims.SetBool("isWalk", true);
                isStop = false;
            }

            // bitiş ve yok oluş
            if (isFinish)
            {
                if (stoppingDistance < 0.3f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    [ClientRpc]
    private void RpcTakeProduct(Vector3 Pos)
    {
        if (orderItems.Count == 0) return;

        Quaternion targetRotation = Quaternion.LookRotation(Pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
    }

    [Server]
    private void ServerSelectGoPos() // start da çalışıyor
    {
        RpcSelectOrderStayGoPos();
    }

    [ClientRpc]
    public void RpcSelectOrderStayGoPos()
    {
        if (orderItems.Count > 0)
        {
            //TargetPos = OrderStayPos.position + new Vector3(-CustomerManager.instance.OrderStayQueue.Count, 0, 0);
            TargetPos = OrderStayPos.position + new Vector3(-customerQueue, 0, 0);
        }
    }

    [ClientRpc]
    public void RpcSelectCashRegisterGoPos()
    {
        if (orderItems.Count > 0)
        {
            //TargetPos = OrderStayPos.position + new Vector3(-CustomerManager.instance.OrderStayQueue.Count, 0, 0);
            TargetPos = CashRegisterStayPos.position + new Vector3(-customerQueue, 0, 0);
        }
    }

    public void
        PlayerGiveProduct(GameObject PlayerHandObj, GameObject player) // player Interact Çağırılır sipariş teslimi
    {
        if (WaitingForOrder)
        {
            ItemPacket packet = PlayerHandObj.GetComponent<ItemPacket>();
            foreach (var orderItem in orderItems)
            {
                foreach (var packetItem in packet.orderItems)
                {
                    if (packetItem.itemName == orderItem.itemName)
                    {
                        if (packetItem.quantity == orderItem.quantity)
                        {
                            CustomerManager.instance.ServerQueueMove();

                            // teslim sonrası player işlemi
                            player.GetComponent<PlayerInteract>().HandFullTest(false);
                            player.GetComponent<PlayerInteract>().isHandRightTrue = false;
                            player.GetComponent<PlayerInteract>().HandObj = null;

                            // teslim sonrası player işlemi
                            PlayerHandObj.transform.SetParent(null);
                            PlayerHandObj.transform.DOMove(Hand.position, 0.6f)
                                .OnComplete(() => ComplitePlayerGiveProduct(PlayerHandObj));
                            WaitingForOrder = false;
                        }
                    }
                }
            }
        }
    }

    private void ComplitePlayerGiveProduct(GameObject PlayerHandObj)
    {
        CustomerManager.instance.CashRegisterQueue.Add(gameObject);
        CustomerManager.instance.OrderStayQueue.Remove(gameObject);


        TargetPos = CashRegisterStayPos.position + new Vector3(-CustomerManager.instance.CashRegisterQueue.Count, 0, 0);
        Destroy(PlayerHandObj);
        CustomerManager.instance.ServerCashQueue(gameObject);
        WaitingForOrder = false;
    }

    // Cash System
    [Server]
    public void CashRegisterProduct(GameObject CashObj, ScrollView BuyCardItemList,Label TotalAmountText)
    {
        RegisterAddToCartManager Cash = CashObj.GetComponent<RegisterAddToCartManager>();
        print(Cash.ItemCardItemList.Count);
        foreach (var orderItem in orderItems)
        {
            foreach (var RegisterItem in Cash.ItemCardItemList)
            {
                if (RegisterItem.name == orderItem.itemName)
                {
                    if (RegisterItem.currentAmount == orderItem.quantity)
                    {
                        RegisterAddToCartManager.instance.buycardAmountChange(TotalAmountText);
                        RandomPosSelectto();
                        
                        // sipariş onaylanınca kartların silinmesi
                        CustomerManager.instance.ServerCashQueueMove();
                        
                        Cash.ItemCardItemList.Clear();
                        Cash.BuyCardItemList.Clear();
                        Cash.BuyProductItemList.Clear();
                        BuyCardItemList.Clear();
                        
                        print(HappySystem);
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
                        break;
                        
                    }
                }
                else
                {
                    print("sipariş yanlış");
                }
            }
        }
    }

    [Server]
    private void RandomPosSelectto()
    {
        CustomerAgent.isStopped = false;
        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);
        TargetPos = new Vector3(randomX, TargetPos.y, randomZ);
        if (TargetPos != null)
        {
            CustomerAgent.SetDestination(TargetPos);
            isFinish = true;
        }
    }

    [Server]
    public void ServerCustomerTime()
    {
        OrderWaitTimer += Time.deltaTime;
        if (OrderWaitTimer < OrderWaitMin)
        {
            HappySystem = 3;
        }
        else if (OrderWaitTimer > OrderWaitMin && OrderWaitTimer < OrderWaitMax)
        {
            HappySystem = 2;
        }
        else if (OrderWaitTimer > OrderWaitMin && OrderWaitTimer > OrderWaitMax)
        {
            HappySystem = 1;
        }

        // if (OrderWaitTimer > OrderWaitTimeHappy)
        // {
        //     HappySystem = 3;
        //     print("NİCEEE");
        // }
        // else if (OrderWaitTimer > OrderWaitNormal)
        // {
        //     HappySystem = 2;
        //     print("EH İŞTE YAPACAK BİŞEY YOK");
        // }
        // else if(OrderWaitTimer > OrderWaitSad)
        // {
        //     HappySystem = 1;
        //     print("YARRAK GİBİ SİPARİŞ AQ YA");
        // }
    }
}
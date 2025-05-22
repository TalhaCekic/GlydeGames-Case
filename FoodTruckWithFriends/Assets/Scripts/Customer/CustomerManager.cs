using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerManager : NetworkBehaviour
{
    public static CustomerManager instance;
    public GameManager gameManager;

    [Header("Per Day Spawn")] [SyncVar] public int customerPerDay;
    [SyncVar] public bool isSpawnCustomer;
    [SyncVar] public bool currentSpawn;
    [SyncVar] public float SpawnDelay;
    [SyncVar] public int SpawnIndex;

    [Header("Per Day Spawn")] public GameObject CustomerPrefab;
    public GameObject SpawnObj;
    public List<GameObject> CustomersList = new List<GameObject>();
    [Header("SpawnPos")] public List<Transform> CustomersSpawnPos = new List<Transform>();

    [Header("Queue")]
    //[SyncVar] public bool CustomerOnStage;
    public List<GameObject> OrderStayQueue;
    public List<GameObject> CashRegisterQueue;

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
        RpcStart();
    }

    [ClientRpc]
    private void RpcStart()
    {
        gameManager = GameManager.instance.GetComponent<GameManager>();
    }

    void Update()
    {
        if (isServer)
        {
            ServerCustomerSpawnState();
        }
    }

    // müşteriler gidince gün bitirişi devreye girer
    [Server]
    public void ServerListQueueState()
    {
        if (OrderStayQueue.Count == 0 && CashRegisterQueue.Count == 0)
        {
            GameManager.instance.RpcDayPanel(true);
        }
    }

    [Server]
    public void ServerCustomerSpawnState()
    {
        if (gameManager == null) return;

        if (gameManager.isDayOn && !isSpawnCustomer)
        {
            customerPerDay = Random.Range(6, 7);
            isSpawnCustomer = true;
        }

        if (gameManager.isDayOn && gameManager.isShopOpen && !currentSpawn)
        {
            if (customerPerDay != CustomersList.Count)
            {
                SpawnDelaySystem();
            }
            else
            {
                isSpawnCustomer = false;
            }
        }
    }

    [Server]
    private void SpawnDelaySystem()
    {
        SpawnDelay += Time.deltaTime;
        if (SpawnDelay > 10)
        {
            if (CustomersList.Count != customerPerDay)
            {
                for (int i = 0; i < customerPerDay; i++)
                {
                    SpawnObj = Instantiate(CustomerPrefab, this.gameObject.transform);
                    NetworkServer.Spawn(SpawnObj);
                    ServerCustomerSpawnIndex(SpawnObj);

                    //Statistic
                    DailyStatistics.instance.ServerCustomerCountAdd(1);

                    SpawnDelay = 0;
                    break;
                }
            }
        }
    }

    [Server]
    void ServerCustomerSpawnIndex(GameObject obj)
    {
        obj.GetComponent<Customer>().customerQueue = OrderStayQueue.Count + 1;
        SpawnIndex = Random.Range(0, CustomersSpawnPos.Count);
        RpcCustomerSpawn(SpawnIndex, obj);
    }

    [ClientRpc]
    private void RpcCustomerSpawn(int index, GameObject obj)
    {
        obj.transform.SetParent(this.gameObject.transform);
        obj.transform.position = CustomersSpawnPos[index].position;
        obj.SetActive(true);
        CustomersList.Add(obj);
        OrderStayQueue.Add(obj);
    }
    [ClientRpc]
    public void ServerCustomerQueueRemove(GameObject obj)
    {
        OrderStayQueue.Remove(obj);
    }

    public void ServerQueueMove()
    {
        foreach (var orderQueue in OrderStayQueue)
        {
            orderQueue.GetComponent<Customer>().customerQueue -= 1;
            orderQueue.GetComponent<Customer>().RpcSelectOrderStayGoPos();
        }
    }

    public void ServerCashQueue(GameObject Obj)
    {
        Obj.GetComponent<Customer>().customerQueue = CashRegisterQueue.Count;
    }

    public void ServerCashQueueMove()
    {
        foreach (var orderQueue in CashRegisterQueue)
        {
            orderQueue.GetComponent<Customer>().customerQueue -= 1;
            orderQueue.GetComponent<Customer>().RpcSelectCashRegisterGoPos();
        }
    }
}
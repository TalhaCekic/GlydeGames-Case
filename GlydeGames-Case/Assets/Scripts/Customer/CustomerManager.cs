using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerManager : NetworkBehaviour
{
    public static CustomerManager instance;
    public GameManager gameManager;
    [Header("ChairManager")] public List<GameObject> Chairs;
    [Header("Toilet")] public List<GameObject> Toilet;

    [Header("Per Day Spawn")] [SyncVar] public int customerPerDay;
    [SyncVar] public bool isSpawnCustomer;
    [SyncVar] public bool currentSpawn;
    [SyncVar] public bool finishSpawn;
    [SyncVar] public float SpawnDelay;
    [SyncVar] public int SpawnIndex;

    [Header("Per Day Spawn")] public GameObject CustomerPrefab;
    public GameObject SpawnObj;
    public List<GameObject> CustomersList = new List<GameObject>();
    [Header("SpawnPos")] public List<Transform> CustomersSpawnPos = new List<Transform>();

    [Header("Queue")] public List<GameObject> OrderStayQueue;

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

    public void RemoveChairs(GameObject chair)
    {
        Chairs.Remove(chair);
    }

    public void AddChairs(GameObject chair)
    {
        Chairs.Add(chair);
    }  
    public void RemoveToilet(GameObject chair)
    {
        Toilet.Remove(chair);
    }

    public void AddToilet(GameObject chair)
    {
        Toilet.Add(chair);
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
        if (OrderStayQueue.Count == 0 && Chairs.Count == 0)
        {
            GameManager.instance.RpcDayPanel(true);
        }
    }

    [Server]
    public void ServerCustomerSpawnState()
    {
        if (gameManager == null) return;
        if (finishSpawn) return;

        if (gameManager.isDayOn && !isSpawnCustomer)
        {
            //customerPerDay = Random.Range(6, 7);
            customerPerDay = 2;
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
            else
            {
                finishSpawn = false;
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
        obj.transform.position = CustomersSpawnPos[index].position;
        obj.GetComponent<Customer>().LeaveSelected = CustomersSpawnPos[index];
        obj.SetActive(true);
        CustomersList.Add(obj);
        OrderStayQueue.Add(obj);
    }

    public void ServerQueueMove()
    {
        foreach (var orderQueue in OrderStayQueue)
        {
            //orderQueue.GetComponent<Customer>().customerQueue -= 1;
            //orderQueue.GetComponent<Customer>().RpcSelectOrderStayGoPos();
            orderQueue.GetComponent<Customer>().ServerSelectGoPos();
        }
    }
    public void ServerQueueMoveRemove()
    {
        foreach (var orderQueue in OrderStayQueue)
        {
            orderQueue.GetComponent<Customer>().customerQueue -= 1;
            orderQueue.GetComponent<Customer>().ServerSelectGoPos();
        }
    }

    public void ServerCashQueueMove()
    {
        foreach (var orderStayQueue in OrderStayQueue)
        {
            orderStayQueue.GetComponent<Customer>().customerQueue -= 1;
            orderStayQueue.GetComponent<Customer>().ServerSelectGoPos();
        }
    }
}
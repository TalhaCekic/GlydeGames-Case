using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NpcPeopleSpawner : NetworkBehaviour
{
    public GameObject NpcPeoplePrefab;

    [SyncVar] public int customerPerDay;

    [SyncVar] public bool currentSpawn;

    public List<GameObject> CustomersList = new List<GameObject>();
    public List<Transform> CustomersSpawnPos = new List<Transform>();
    //public GameObject SpawnObj;

    [SyncVar] public float delay1;
    [SyncVar] public float delay2;

    void Start()
    {
        if (isServer)
        {
            ServerStartNpcSpawnState();
        }
    }

    void Update()
    {
        if (isServer)
        {
            ServerNpcSpawnState();
        }
    }

    [Server]
    public void ServerStartNpcSpawnState()
    {
        customerPerDay = Random.Range(15, 20);
    }

    [Server]
    private void ServerNpcSpawnState()
    {
        if (customerPerDay != CustomersList.Count)
        {
            ServerSpawnSystem();
        }
    }

    [Server]
    private void ServerSpawnSystem()
    {
        delay1 += Time.deltaTime;
        if (delay1 > 2)
        {
            if (CustomersList.Count != customerPerDay)
            {
                for (int i = 0; i < customerPerDay; i++)
                {
                    int index = Random.Range(0, CustomersSpawnPos.Count);
                    GameObject SpawnObj = Instantiate(NpcPeoplePrefab);
                    NetworkServer.Spawn(SpawnObj);
                    SpawnObj.GetComponent<NpcPeopleMovement>().ServerCustomerSpawn(SpawnObj, CustomersSpawnPos);
                    RpcSpawnSystem(SpawnObj,index);
                    currentSpawn = true;
                    delay1 = 0;
                    break;
                }
            }
        }
    }

    [ClientRpc]
    private void RpcSpawnSystem(GameObject obj,int index)
    {
        CustomersList.Add(obj);
        obj.SetActive(true);
        obj.transform.position = CustomersSpawnPos[index].position;
    }
}
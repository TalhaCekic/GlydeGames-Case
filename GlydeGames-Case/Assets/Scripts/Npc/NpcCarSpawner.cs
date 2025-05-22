using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NpcCarSpawner : NetworkBehaviour
{
    [SyncVar] public string SelectPathStringSpawner;
    [SyncVar] public int customerPerDay;

    [SyncVar] public bool currentSpawn;
    
    public GameObject SpawnObj;
    
    [SyncVar] public int SelectCarIndex;
    public List<GameObject> CarsPrefab = new List<GameObject>();
    
    public List<GameObject> CarList = new List<GameObject>();
    public List<Transform> CustomersSpawnPos = new List<Transform>();
    
    void Start()
    {
        if (isServer)
        {
            ServerStartNpcSpawnState();
        }
    }
    [Server]
    public void ServerStartNpcSpawnState() {
        customerPerDay = Random.Range(3, 5);
    }
    void Update()
    {
        if (isServer)
        {
            ServerNpcSpawnState();
        }
    }
    [Server]
    public void ServerNpcSpawnState() {
        if (!currentSpawn)
        {
            StartCoroutine(ScaleCoroutine());
        }
    }
    [Server]
    IEnumerator ScaleCoroutine() {
        currentSpawn = true;
        yield return new WaitForSeconds(1);
        if (CarList.Count != customerPerDay)
        {
            for (int i = 0; i < customerPerDay; i++)
            {
                SelectCarIndex = Random.Range(0, CarsPrefab.Count);
                SpawnObj = Instantiate(CarsPrefab[SelectCarIndex]);
                
                NetworkServer.Spawn(SpawnObj);
                SpawnObj.SetActive(true);
                SpawnObj.GetComponent<NpcCarMovement>().SelectPathString = SelectPathStringSpawner;
                SpawnObj.GetComponent<NpcCarMovement>().ServerCustomerSpawn(SpawnObj,CustomersSpawnPos);

                CarList.Add(SpawnObj);
				
                currentSpawn = true;
                break;
            }
        }
        yield return new WaitForSeconds(10);
        currentSpawn = false;
    }
}

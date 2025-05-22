using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class LocalPlayerSpawn : NetworkBehaviour
{
    Transform SelectSpawnPos;
    public List<GameObject> spawnPoints = new List<GameObject>();

    void Start()
    {
        if (isLocalPlayer && isServer)
        {
            ServerStart();
        }
    }

    [Server]
    public void ServerStart()
    {
        RpcStart();
    }

    [ClientRpc]
    private void RpcStart()
    {
        GameObject[] spawnPos = GameObject.FindGameObjectsWithTag("SpawnPos");
        for (int i = 0; i < spawnPos.Length; i++)
        {
            spawnPoints.Add(spawnPos[i]);
        }

        if (spawnPoints != null)
        {
          //  Transform spawnPoint = spawnPoints[spawnPoints.Count].transform;
        
            this.transform.position = spawnPoints[1].transform.position;
        
            this.transform.rotation = spawnPoints[1].transform.rotation;
        }
    }

    void Update()
    {
    }
}
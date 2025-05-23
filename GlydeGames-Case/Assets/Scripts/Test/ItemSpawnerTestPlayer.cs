using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawnerTestPlayer : NetworkBehaviour
{
    public Datas data;
    public Transform spawnPos;
    
    void Start()
    {
        
    }

    void Update()
    {
        CmdSpawnItem();
    }

    [Command]
    private void CmdSpawnItem()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GameObject obj = Instantiate(data._categoryData[0].ObjPrefab);
            NetworkServer.Spawn(obj);
            RpcSpawnItem(obj);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameObject obj = Instantiate(data._categoryData[1].ObjPrefab);
            NetworkServer.Spawn(obj);
            RpcSpawnItem(obj);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameObject obj = Instantiate(data._categoryData[5].ObjPrefab);
            NetworkServer.Spawn(obj);
            RpcSpawnItem(obj);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            GameObject obj = Instantiate(data._categoryData[6].ObjPrefab);
            NetworkServer.Spawn(obj);
            RpcSpawnItem(obj);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameObject obj = Instantiate(data._categoryData[7].ObjPrefab);
            NetworkServer.Spawn(obj);
            RpcSpawnItem(obj);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            GameObject obj = Instantiate(data._categoryData[8].ObjPrefab);
            NetworkServer.Spawn(obj);
            RpcSpawnItem(obj);
        }
    }

    [ClientRpc]
    private void RpcSpawnItem(GameObject obj)
    {
        obj.transform.position = spawnPos.position;
    }
}

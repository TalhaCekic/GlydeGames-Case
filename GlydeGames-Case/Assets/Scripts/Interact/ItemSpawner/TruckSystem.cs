using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class TruckSystem : NetworkBehaviour
{
    [Header("Spline System")]
    public SplineFollower splineFollover;
    public SplineComputer splineComputer;

    [Header("Duraksama Ayarı")]
    [SyncVar] public bool isStop;
    [SyncVar] public float timeDelay;

    [Header("Box Spawn System")] 
    public BoxSpawnerManager boxSpawnerManager;

    public Transform InParentBoxSpawnTransform;
    void Start() {
        if (isServer)
        {
            ServerTest();
        }
    }
    [Server]
    private void ServerTest()
    {
        RpcTest();
        AddToCartManager.instance.ServerBuyButtonSettings(true);
    }
    
    [ClientRpc]
    private void RpcTest()
    {
        GameObject splineObject = GameObject.Find("WayKargoTruckTest");
        splineComputer = splineObject.GetComponent<SplineComputer>();
        
        GameObject BoxSpawnObj = GameObject.Find("BoxSpawnPos");
        boxSpawnerManager = BoxSpawnObj.GetComponent<BoxSpawnerManager>();
        
        if (splineComputer != null)
        {
            splineFollover.spline = splineComputer; // yolun atamasını yap
            splineFollover.followSpeed = 13;
        }

        if (boxSpawnerManager == null)
        {
            // BoxSpawnObj.transform.parent = InParentBoxSpawnTransform; // bu objeye başlangıçta parent yap
            // BoxSpawnObj.transform.position = Vector3.zero; // bu objeye başlangıçta parent yap
            // print("aaaaa");
        }
    }

    void Update()
    {
        if (isServer)
        {
            ServerUpdate();
            Timer();
        }
    }
    [Server]
    private void ServerUpdate()
    {
        RpcUpdate();
    }
    
    [ClientRpc]
    private void RpcUpdate()
    {
        if (splineFollover.result.percent > 0.36f)
        {
            if (timeDelay > 20)
            {
                splineFollover.follow = true;
            }
            else
            {
                splineFollover.follow = false;
                isStop = true;
                
                boxSpawnerManager.transform.position = InParentBoxSpawnTransform.position;
                boxSpawnerManager.ServerItemListRemove();
            }
        }

        if (splineFollover.result.percent >= 0.95f)
        {
            AddToCartManager.instance.ServerBuyButtonSettings(false);
            Destroy(gameObject);
        }
    }

    [Server]
    private void Timer()
    {
        if (isStop)
        {
            timeDelay += Time.deltaTime;
        }
    }

}

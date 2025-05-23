using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class TruckSystem : NetworkBehaviour
{
    [Header("Spline System")] public SplineFollower splineFollover;
    public SplineComputer splineComputer;

    [Header("Duraksama Ayarı")] [SyncVar] public bool isStop;
    [SyncVar] public float timeDelay;

    [Header("Box Spawn System")] public BoxSpawnerManager boxSpawnerManager;

    public Transform InParentBoxSpawnTransform;

    public Transform LeftDoor, RightDoor;

    void Start()
    {
        if (isServer)
        {
            ServerTest();
        }
    }

    [Server]
    private void ServerTest()
    {
        RpcTest();
        ProductItem.instance.ServerBuyButtonSettings(true);
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

        boxSpawnerManager.transform.parent = InParentBoxSpawnTransform;
        boxSpawnerManager.transform.position = InParentBoxSpawnTransform.position;
        boxSpawnerManager.transform.rotation = InParentBoxSpawnTransform.rotation;
    }

    void Update()
    {
        if (isServer)
        {
            ServerUpdate();
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
        if (splineFollover.result.percent > 0.5f)
        {
            if (boxSpawnerManager.Items.Count==0)
            {
                splineFollover.follow = true;
                boxSpawnerManager.transform.parent = null;
                LeftDoor.transform.localRotation = Quaternion.Euler(0, 0, 0);
                RightDoor.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                splineFollover.follow = false;
                isStop = true;
    
                boxSpawnerManager.transform.parent = InParentBoxSpawnTransform;
                boxSpawnerManager.transform.position = InParentBoxSpawnTransform.position;
                boxSpawnerManager.transform.rotation = InParentBoxSpawnTransform.rotation;
                boxSpawnerManager.ServerItemListRemove();
    
                LeftDoor.transform.localRotation = Quaternion.Euler(0, 90, 0);
                RightDoor.transform.localRotation = Quaternion.Euler(0, -90, 0);
            }
        }
    
        if (splineFollover.result.percent >= 0.95f)
        {
            ProductItem.instance.ServerBuyButtonSettings(false);
            Destroy(gameObject);
        }
    }
}
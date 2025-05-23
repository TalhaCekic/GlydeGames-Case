using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CarSlotPos : NetworkBehaviour
{
    public Transform slotPos;
    void Update()
    {
        if(!isServer)return;
        ServerCarSlotPos();
    }

    [Server]
    private void ServerCarSlotPos()
    {
        RpcCarSlotPos();
    }

    [ClientRpc]
    private void RpcCarSlotPos()
    {
        GameObject slot = GameObject.Find("CarSlotPosTransform");
        slot.transform.SetParent(gameObject.transform);
        //slot.transform.position = slotPos.transform.position;
        slot.transform.position = slotPos.transform.position;
    }


}

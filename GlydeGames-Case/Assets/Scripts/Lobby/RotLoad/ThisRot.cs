using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ThisRot : NetworkBehaviour
{

    void Update()
    {
        if (isServer)
        {
            ServerRot();
        }
    }

    [Server]
    private void ServerRot()
    {
        RpcRot();
    }

    [ClientRpc]
    private void RpcRot()
    {
        this.transform.Rotate(new Vector3(0,0,-1f));
    }
}

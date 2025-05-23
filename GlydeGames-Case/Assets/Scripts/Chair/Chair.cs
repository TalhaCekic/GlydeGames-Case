using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Chair : NetworkBehaviour
{
    [SyncVar] public bool isStateNull;

    [Server]
    public void ServerStateChange(bool value)
    {
        isStateNull = value;
    }
}

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PacketItem : NetworkBehaviour
{
    [SyncVar] public bool isPacketItem;

    public void ServerPacketItemChange()
    {
        isPacketItem = true;
    }
}

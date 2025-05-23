using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class DeepFryer : NetworkBehaviour
{
    public ItemBox _ItemBox;
    [SyncVar] public bool Availability;
    public FryerButton _FryerButton;
    

    [Server]
    public void avalibleChack()
    {
        if (_ItemBox._itemAmount == 0)
        {
            Availability = false;
            RpcTickClose();
        }
    }

    [ClientRpc]
    private void RpcTickClose()
    {
        _FryerButton.TickCanvasClose();
    }
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;

public class Door : NetworkBehaviour
{
    [SyncVar] public bool isDoor;
    public void ServerDoorOpen()
    {
        isDoor = !isDoor;
        if (isDoor)
        {
            gameObject.transform.DORotate(new Vector3(0,-75,0), 1);
        }
        else
        {
            gameObject.transform.DORotate(new Vector3(0,-180,0), 1);
        }
    }

    [ClientRpc]
    public void isDoorChange(bool value)
    {
        isDoor = value;
        if (isDoor)
        {
            gameObject.transform.DORotate(new Vector3(0,-75,0), 1);
        }
        else
        {
            gameObject.transform.DORotate(new Vector3(0,-180,0), 1);
        }
    }
}

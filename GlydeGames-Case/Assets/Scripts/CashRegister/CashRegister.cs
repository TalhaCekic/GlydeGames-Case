using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class CashRegister : NetworkBehaviour
{
    [SyncVar] public bool isInteractCR;
    public Transform CamPos;
    
    public void CmdSetInteractCR() {
        isInteractCR = !isInteractCR;
    }

    // oyuncunun ekrana odaklanmasını sağlar
    public void SetPlayerPosition()
    {
        Camera.main.transform.DOMove(CamPos.transform.position, 1);
        Camera.main.transform.rotation = CamPos.transform.rotation;
    }
    
}
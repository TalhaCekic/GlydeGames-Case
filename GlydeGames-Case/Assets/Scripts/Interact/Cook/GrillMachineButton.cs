using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GrillMachineButton : NetworkBehaviour
{
    [SyncVar] public bool isStart;


    public void GrillStartAndStop()
    {
        isStart = !isStart;
    }
}

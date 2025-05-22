using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class wheel : NetworkBehaviour
{
    public WheelCollider targetWheel;
    private Vector3 wheelPosition = new Vector3();
    private Quaternion wheelRotation = new Quaternion();

    void Update()
    {
        if (isServer)
        {
            ServerWheel();
        }
    }
    [Server]
    private void ServerWheel() {
        RpcWheel();
    }
    [ClientRpc]
    private void RpcWheel() {
        targetWheel.GetWorldPose(out wheelPosition,out wheelRotation);
        transform.position = wheelPosition;
        transform.rotation = wheelRotation;
    }
}

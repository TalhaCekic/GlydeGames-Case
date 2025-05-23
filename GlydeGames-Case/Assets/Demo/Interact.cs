using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening; // DOTween kütüphanesi için gerekli

public class Interact : NetworkBehaviour
{
    public LayerMask interactableLayer;
    public Transform holdPoint;
    public float moveSpeed = 10f;
    public float throwForce = 500f;

    [SyncVar] private uint selectedNetId = 0; // Aðda seçili objenin netId'si
    private Rigidbody selectedRigidbody;

    private void Update()
    {
        if (!isLocalPlayer) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
            {
                if (hit.rigidbody != null)
                {
                    NetworkIdentity netId = hit.rigidbody.GetComponent<NetworkIdentity>();
                    if (netId != null)
                    {
                        CmdSelectObject(netId.netId);
                    }
                }
            }
        }

        if (Input.GetMouseButton(0) && selectedRigidbody != null)
        {
            print("Selected Rigidbody: " + selectedRigidbody.name);
            selectedRigidbody.MovePosition(Vector3.Lerp(
                selectedRigidbody.position,
                holdPoint.position,
                moveSpeed * Time.deltaTime
            ));
        }

        if (Input.GetMouseButtonUp(0) && selectedRigidbody != null)
        {
            CmdReleaseObject();
        }

        if (Input.GetMouseButtonDown(1) && selectedRigidbody != null)
        {
            CmdThrowObject(Camera.main.transform.forward);
        }
    }

    [Command]
    private void CmdSelectObject(uint netId)
    {
        RpcSelectObject(netId);
    }

    [ClientRpc]
    private void RpcSelectObject(uint netId)
    {
        NetworkIdentity objIdentity = null;
        foreach (var identity in FindObjectsOfType<NetworkIdentity>())
        {
            if (identity.netId == netId)
            {
                objIdentity = identity;
                break;
            }
        }
        if (objIdentity != null)
        {
            selectedRigidbody = objIdentity.GetComponent<Rigidbody>();
            if (selectedRigidbody != null)
            {
                selectedRigidbody.isKinematic = true;
            }
        }
    }

    [Command]
    private void CmdReleaseObject()
    {
        RpcReleaseObject();
    }

    [ClientRpc]
    private void RpcReleaseObject()
    {
        if (selectedRigidbody != null)
        {
            selectedRigidbody.isKinematic = false;
            selectedRigidbody = null;
        }
    }

    [Command]
    private void CmdThrowObject(Vector3 direction)
    {
        RpcThrowObject(direction);
    }

    [ClientRpc]
    private void RpcThrowObject(Vector3 direction)
    {
        if (selectedRigidbody != null)
        {
            selectedRigidbody.isKinematic = false;
            selectedRigidbody.AddForce(direction * throwForce);
            selectedRigidbody = null;
        }
    }
}

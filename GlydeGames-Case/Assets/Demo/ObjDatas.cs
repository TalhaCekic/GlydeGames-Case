using Mirror;
using UnityEngine;

public class ObjDatas : NetworkBehaviour
{
    public int maxValue = 100;
    public int currentValue;
    public float playerPushForce = 500f;
    public float impactThreshold = 10f;

    private void Start()
    {
        ServerStart();
    }

    [Server]
    private void ServerStart()
    {
        ClientStart();
    }

    [ClientRpc]
    private void ClientStart()
    {
        currentValue = maxValue;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return;

        float impactForce = collision.relativeVelocity.magnitude;
        print($"Çarpma hýzý: {impactForce}");

        if (impactForce >= impactThreshold)
        {
            int damage = Mathf.RoundToInt(impactForce);
            currentValue -= damage;
            currentValue = Mathf.Clamp(currentValue, 0, maxValue);
            Debug.Log($"Objeye çarpma oldu! Hasar: {damage}, Kalan Deðer: {currentValue}");

            if (collision.collider.CompareTag("Player"))
            {
                NetworkIdentity playerNetId = collision.collider.GetComponent<NetworkIdentity>();
                if (playerNetId != null)
                {
                    Vector3 pushDirection = collision.contacts[0].normal;
                    RpcPushPlayer(playerNetId.netId, -pushDirection * playerPushForce);
                    Debug.Log("Oyuncu geri itildi (RPC ile)!");
                }
            }
        }
        else
        {
            Debug.Log($"Çarpma hýzý ({impactForce}) eþik deðerin altýnda, iþlem yapýlmadý.");
        }
    }

    [ClientRpc]
    private void RpcPushPlayer(uint playerNetId, Vector3 force)
    {
        // Sadece ilgili oyuncunun Rigidbody'sine kuvvet uygula
        foreach (var identity in FindObjectsOfType<NetworkIdentity>())
        {
            if (identity.netId == playerNetId)
            {
                Rigidbody playerRigidbody = identity.GetComponent<Rigidbody>();
                if (playerRigidbody != null)
                {
                    playerRigidbody.AddForce(force, ForceMode.Impulse);
                }
                break;
            }
        }
    }
}

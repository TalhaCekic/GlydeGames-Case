using Mirror;
using TMPro;
using UnityEngine;

public class ObjDatas : NetworkBehaviour
{
    public int maxValue = 100;

    [SyncVar(hook = nameof(OnCurrentValueChanged))]
    public int currentValue;

    float playerPushForce = 80f;
    public float impactThreshold = 10f;

    public TMP_Text valueText;

    public override void OnStartServer()
    {
        currentValue = maxValue;
    }

    private void OnCurrentValueChanged(int oldValue, int newValue)
    {
        valueText.text = newValue.ToString();
    }

    private void Start()
    {
        // Ýlk deðer atamasý için (client'ta hook tetiklenir)
        valueText.text = currentValue.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return;

        float impactForce = collision.relativeVelocity.magnitude;
        if (impactForce >= impactThreshold)
        {
            int damage = Mathf.RoundToInt(impactForce);
            currentValue -= damage;
            currentValue = Mathf.Clamp(currentValue, 0, maxValue);

            if (collision.collider.CompareTag("Player"))
            {
                NetworkIdentity playerNetId = collision.collider.GetComponent<NetworkIdentity>();
                if (playerNetId != null)
                {
                    Vector3 pushDirection = collision.contacts[0].normal;
                    RpcPushPlayer(playerNetId.netId, -pushDirection * playerPushForce);
                }
            }
        }
    }

    [ClientRpc]
    private void RpcPushPlayer(uint playerNetId, Vector3 force)
    {
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

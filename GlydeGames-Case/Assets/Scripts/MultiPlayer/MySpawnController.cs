using Mirror;
using UnityEngine;

public class MySpawnController : NetworkBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        // Oyuncu spawn i?lemi
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(NetworkServer.connections[0], player);
    }
}
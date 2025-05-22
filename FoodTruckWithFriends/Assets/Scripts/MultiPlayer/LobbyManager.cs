using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager instance;
    public string PlayerTag;
    public GameObject[] Players;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayerData()
    {
        if (isServer)
        {
            ServerPlayerListAdd();
        }
    }

    //
    [Server]
    public void ServerPlayerListAdd()
    {
        RpcPlayerListAdd();
    }

    [ClientRpc]
    public void RpcPlayerListAdd()
    {
        Players = GameObject.FindGameObjectsWithTag(PlayerTag);
    }
}
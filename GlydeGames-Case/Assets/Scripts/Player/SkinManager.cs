using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Steamworks;
using Unity.VisualScripting;

public class SkinManager : NetworkBehaviour
{
    [SyncVar] public ulong SteamID;
    
    public GameObject DefaultSkin;

    public GameObject ToficSkin;
    public GameObject TalosSkin;
    public GameObject BongoSkin;

    public List<GameObject> thisObj;
    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }

            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }
    void Start() {

        
        if (isServer)
        {
            ServerSkin();  
        }
    }
    void Update() {
        if (isServer)
        {
            ServerSkin();  
        }
    }
    [Server]
    private void ServerSkin() {
        if(Manager == null)return;
        SteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.currentLobbyID, Manager.GamePlayers.Count);
        //RpcSkin(SteamID);
    }

}

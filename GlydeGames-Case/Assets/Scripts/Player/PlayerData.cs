using System;
using Mirror;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;


public class PlayerData : NetworkBehaviour
{
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    //private bool AvatarReceived;

    [SyncVar(hook = nameof(PlayerNameUpdate))]
    public string PlayerName;

    public TMP_Text PlayerNameText;

    public GameObject PlayerUI;
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

    private void Start()
    {    
        foreach (var player in Manager.playerDatas)
        {
            if (player.PlayerIdNumber == this.PlayerIdNumber)
            {
                PlayerUI.SetActive(false);
            }
            else
            {
                PlayerUI.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (isServer)
        {
            SetPlayerValues();
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
    }

    public override void OnStartClient()
    {
        Manager.playerDatas.Add(this);
    }

    public override void OnStopClient()
    {
        Manager.playerDatas.Remove(this);
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if (isServer)
        {
            this.PlayerName = NewValue;
        }

        // if (isClient)
        // {
        //     LobbyController.Instance.UpdatePlayerList();
        // }
    }
    [Server]
    public void SetPlayerValues()
    {
        RpcSetPlayerValues();
    }

    [ClientRpc]
    public void RpcSetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
    }
}
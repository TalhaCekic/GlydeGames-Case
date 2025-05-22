using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class InGameLobbyController : NetworkBehaviour
{ 
    public static InGameLobbyController Instance;

    [Header("Player Data")] public GameObject PlayerListViewContant;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    [Header("Other Data")] public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    public List<PlayerData> playerDatas = new List<PlayerData>();
    public PlayerData PlayerData;

    //Manager
    private CustomNetworkManager manager;

    [Header("Ready System")]
    // public Button StartGameButton;
    public TMP_Text ReadyButtonText;

    public string ReadyString;
    public string UnreadyString;
    public bool isNextScene;
    [SyncVar] public int ReadyPlayerCount; 

    public GameObject lobbyOwner;

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

    void Start()
    {
        Instance = this;
    }
    
    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().currentLobbyID;
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated)
        {
            CreateHostPlayerItem();
        }

        if (playerDatas.Count < Manager.GamePlayers.Count)
        {
            CreateClintPlayerItem();
        }

        if (playerDatas.Count > Manager.GamePlayers.Count)
        {
            RemovePlayerItem();
        }

        if (playerDatas.Count == Manager.GamePlayers.Count)
        {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        //LocalPlayerController = LocalPlayerObject.GetComponent<PlayerData>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerListObjController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerData NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerData>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.SetPlayerValues();

            playerDatas.Add(NewPlayerItemScript);
        }

        PlayerItemCreated = true;
    }

    public void CreateClintPlayerItem()
    {
        foreach (PlayerListObjController player in Manager.GamePlayers)
        {
            if (!playerDatas.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerData NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerData>();
                
                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContant.transform);
                NewPlayerItem.transform.localScale = Vector3.one;

                playerDatas.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerListObjController player in Manager.GamePlayers)
        {
            foreach (PlayerData PlayerLisItemScript in playerDatas)
            {
                if (PlayerLisItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerLisItemScript.PlayerName = player.PlayerName;

                    PlayerLisItemScript.SetPlayerValues();
                }
            }
        }
    }

    public void RemovePlayerItem()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            List<PlayerData> playerListItemToRemove = new List<PlayerData>();

            foreach (PlayerData playerList in playerDatas)
            {
                if (!Manager.playerDatas.Any(b => b.ConnectionID == playerList.ConnectionID))
                {
                    playerListItemToRemove.Add(playerList);
                }
            }

            if (playerListItemToRemove.Count > 0)
            {
                foreach (PlayerData PlayerListItemRemove in playerListItemToRemove)
                {
                    GameObject ObjectToRemove = PlayerListItemRemove.gameObject;
                    playerDatas.Remove(PlayerListItemRemove);
                    Destroy(ObjectToRemove);
                    ObjectToRemove = null;
                }
            }
        }
    }

}

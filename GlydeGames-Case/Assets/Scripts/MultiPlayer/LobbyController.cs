using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : NetworkBehaviour
{
    public static LobbyController Instance;

    [Header("Player Data")]
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    [Header("Other Data")] public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    public List<PlayerListItem> PlayerListItem = new List<PlayerListItem>();
    public PlayerListObjController LocalPlayerController;

    //Manager
    private CustomNetworkManager manager;

    [Header("Ready System")]

    public string ReadyString;
    public string UnreadyString;
    [SyncVar] public int ReadyPlayerCount;

    [SyncVar] public bool AllReady;
    [SyncVar] public float nextDelay;

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

    void Awake()
    {
        Instance = this;
    }
    public void InvitePlayer()
    {
        SteamLobby.instance.InviteButton();
    }
    public void ReadyPlayer()
    {
        LocalPlayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (LocalPlayerController.Ready)
        {
            MainMenuCanvas.instance.ReadyButton.text = ReadyString;
            MainMenuCanvas.instance.ReadyButton.style.color = Color.green;
        }
        else
        {
            MainMenuCanvas.instance.ReadyButton.text = UnreadyString;
            MainMenuCanvas.instance.ReadyButton.style.color = Color.red;
        }
    }

    public void CheckIfAllReady()
    {
        if (PlayerListItem.Count == ReadyPlayerCount)
        {
            AllReady = true;
        }
        else
        {
            AllReady = false;
            nextDelay = 0;
        }
    }

    private void Update()
    {
        if (isServer)
        {
            StateReadyNextScene();
        }
    }

    [Server]
    private void StateReadyNextScene()
    {
        if (AllReady)
        {
            nextDelay += Time.deltaTime;
            if (nextDelay > 3)
            {
                SteamLobby.instance.NextScene();
            }
        }
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

        if (PlayerListItem.Count < Manager.GamePlayers.Count)
        {
            CreateClintPlayerItem();
        }

        if (PlayerListItem.Count > Manager.GamePlayers.Count)
        {
            RemovePlayerItem();
        }

        if (PlayerListItem.Count == Manager.GamePlayers.Count)
        {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerListObjController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerListObjController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.isReady = player.Ready;
            NewPlayerItemScript.SetPlayerValues();

            NewPlayerItem.transform.SetParent(player.PlayerListViewContant.transform);
            NewPlayerItem.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
            NewPlayerItem.transform.position = player.PlayerListViewContant.transform.position;
            NewPlayerItem.transform.localRotation = new Quaternion(0, 180, 0, 0);

            PlayerListItem.Add(NewPlayerItemScript);
            MainMenuCanvas.instance.UpdatePlayerCount(PlayerListItem.Count);
        }
        PlayerItemCreated = true;
    }

    public void CreateClintPlayerItem()
    {
        foreach (PlayerListObjController player in Manager.GamePlayers)
        {
            if (!PlayerListItem.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.isReady = player.Ready;
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(player.PlayerListViewContant.transform);
                NewPlayerItem.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                NewPlayerItem.transform.position = player.PlayerListViewContant.transform.position;
                NewPlayerItem.transform.localRotation = new Quaternion(0, 180, 0, 0);

                PlayerListItem.Add(NewPlayerItemScript);
                CheckIfAllReady();
                MainMenuCanvas.instance.UpdatePlayerCount(PlayerListItem.Count);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerListObjController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerLisItemScript in PlayerListItem)
            {
                if (PlayerLisItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerLisItemScript.PlayerName = player.PlayerName;
                    PlayerLisItemScript.isReady = player.Ready;

                    PlayerLisItemScript.SetPlayerValues();
                    UpdateButton();
                    MainMenuCanvas.instance.UpdatePlayerCount(PlayerListItem.Count);
                }
            }
        }
    }

    public void RemovePlayerItem()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

            foreach (PlayerListItem playerList in PlayerListItem)
            {
                if (!Manager.GamePlayers.Any(b => b.ConnectionID == playerList.ConnectionID))
                {
                    playerListItemToRemove.Add(playerList);
                }
            }
            if (playerListItemToRemove.Count > 0)
            {
                foreach (PlayerListItem PlayerListItemRemove in playerListItemToRemove)
                {
                    GameObject ObjectToRemove = PlayerListItemRemove.gameObject;
                    if (PlayerListItemRemove.isReady)
                    {
                        ReadyPlayerCount--;
                    }
                    PlayerListItem.Remove(PlayerListItemRemove);
                    Destroy(ObjectToRemove);
                    ObjectToRemove = null;
                }
            }
            MainMenuCanvas.instance.UpdatePlayerCount(PlayerListItem.Count);
        }
    }
}
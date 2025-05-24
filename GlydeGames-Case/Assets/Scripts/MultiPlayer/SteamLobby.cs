using System;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror.FizzySteam;
using System.Collections;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;
    protected Callback<SteamServersDisconnected_t> LobbyDisconnected;
    protected Callback<GSClientKick_t> lobbyKicked;

    public ulong currentLobbyID;
    public const string HostAdressKey = "HostAddress";
    private CustomNetworkManager manager;

    public string MainMenuSceneName;
    public string GamePlayScene;

    private float DelayForLobby;
    
    private CSteamID hostSteamID;  // Host'un SteamID'si
    public bool isHostDisconnected = false; // Host bağlantı durumu için kontrol

    private void Awake()
    {
        if (instance != null)
        {
            instance = this;
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // if (SceneManager.GetActiveScene().buildIndex != 0)
        // {
        //     currentLobbyID = SteamLobby.instance.currentLobbyID;
        // }
        if (SceneManager.GetActiveScene().buildIndex != 0)return;
        DelayForLobby = 0;
        CheckSteamConnection();
        if (!SteamManager.Initialized)
        {
            return;
        }

        //manager = GetComponent<CustomNetworkManager>();
        GameObject Manager = GameObject.Find("NetworkManager");
        manager = Manager.GetComponent<CustomNetworkManager>();
        instance = this;

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(onJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        if (NetworkServer.active) return;
        Host();
    }

    private void Update()
    {
        CheckHostStatus();
    }
    public void CheckHostStatus()
    {
       // if (currentLobbyID == 0) return;
       currentLobbyID = SteamLobby.instance.currentLobbyID; 
        
        int memberCount = SteamMatchmaking.GetNumLobbyMembers(new CSteamID(currentLobbyID));

        for (int i = 0; i < memberCount; i++)
        {
            CSteamID memberID = SteamMatchmaking.GetLobbyMemberByIndex(new CSteamID(currentLobbyID), i);
            if (memberID == hostSteamID)
            {
                isHostDisconnected = false; 
                DelayForLobby = 0;
                return;
            }
        }

        isHostDisconnected = true;
        if (currentLobbyID == 0 && isHostDisconnected) 
        {
            if (DelayForLobby > 3)
            {
                DelayForLobby = 0;
                leaving();
            }
            else
            {
                DelayForLobby += Time.deltaTime;
            }
        }
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        NetworkManager.singleton.ServerChangeScene(GamePlayScene);
        currentLobbyID = SteamLobby.instance.currentLobbyID; 
    }

    private void CheckSteamConnection()
    {
        if (!SteamManager.Initialized)
        {
            SteamManager steamManager = manager.GetComponent<SteamManager>();
            steamManager.enabled = true;
            FizzySteamworks.active.ClientConnected();
        }
    }

    public void Host()
    {
        print("host");
        NetworkManager.singleton.maxConnections = 4;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, NetworkManager.singleton.maxConnections);
        hostSteamID = SteamUser.GetSteamID();
    }

    // lobiden ayrılma li
    public void leaving()
    {
       // if (NetworkServer.active)
       // {
       //     NetworkManager.singleton.StopHost();
       // }
       // else if (NetworkClient.active)
      //  {
            NetworkManager.singleton.StopClient();
       // }

       // SteamMatchmaking.LeaveLobby(new CSteamID(currentLobbyID));
        //SteamMatchmaking.DeleteLobbyData((CSteamID)currentLobbyID, "name");
        currentLobbyID = 0;
       // if (LobbyDatas.instance != null)
      //  {
        //    Destroy(LobbyDatas.instance.gameObject);
       // }
        SceneManager.LoadScene(0);
        StartCoroutine(DelayedAction());
    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
        CustomNetworkManager.singleton.StartHost();
        Host();
    }

    // lobiden ayrılma butonu
    public void leavingToServer()
    {
        
        if (NetworkServer.active)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.active)
        {
            NetworkManager.singleton.StopClient();
        }

        SteamMatchmaking.LeaveLobby(new CSteamID(currentLobbyID));
        SteamMatchmaking.DeleteLobbyData((CSteamID)currentLobbyID, "name");
        currentLobbyID = 0;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        NetworkManager.singleton.ServerChangeScene(GamePlayScene);
    }

    public void InviteButton()
    {
        SteamFriends.ActivateGameOverlay("Friends");
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        //manager.StartHost();
        NetworkManager.singleton.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey,
            SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name",
            SteamFriends.GetPersonaName().ToString());
        hostSteamID = SteamUser.GetSteamID();
    }

    private void onJoinRequest(GameLobbyJoinRequested_t callback)
    {
        //leavingToServer();
        //MainMenuCanvas.instance.joinedLobby();
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //everyone
        currentLobbyID = callback.m_ulSteamIDLobby;

        //clients
        if (NetworkServer.active)
        {
            return;
        }

        NetworkManager.singleton.networkAddress =
            SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey);
        NetworkManager.singleton.StartClient();
    }
    
}
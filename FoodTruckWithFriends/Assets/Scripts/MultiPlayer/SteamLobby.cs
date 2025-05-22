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

    private void Awake()
    {
        if (instance != null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        CheckSteamConnection();
        if (!SteamManager.Initialized)
        {
            return;
        }

        manager = GetComponent<CustomNetworkManager>();
        instance = this;

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(onJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        LobbyDisconnected = Callback<SteamServersDisconnected_t>.Create(OnLobbyDisconnected);
        lobbyKicked = Callback<GSClientKick_t>.Create(OnLobbyKicked);
        Host();
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        NetworkManager.singleton.ServerChangeScene(GamePlayScene);
    }

    private void CheckSteamConnection()
    {
        if (!SteamManager.Initialized)
        {
            SteamManager steamManager = gameObject.GetComponent<SteamManager>();
            steamManager.enabled = true;
            FizzySteamworks.active.ClientConnected();
        }
    }

    public void SinglePlayerButton()
    {
        manager.maxConnections = 1;
        NextScene();
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    public void Host()
    {
        manager.maxConnections = 4;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    // lobiden ayrılma butonu
    public void leaving()
    {
        SteamMatchmaking.LeaveLobby((CSteamID)currentLobbyID);
        SteamMatchmaking.DeleteLobbyData((CSteamID)currentLobbyID, "name");
        MainMenu();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        NetworkManager.singleton.ServerChangeScene(GamePlayScene);
    }

    public void MainMenu()
    {
        NetworkManager.singleton.StopClient();
        NetworkManager.singleton.StopHost();
        SceneManager.LoadScene(0);
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

        manager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey,
            SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name",
            SteamFriends.GetPersonaName().ToString());
    }

    private void onJoinRequest(GameLobbyJoinRequested_t callback) {
        leaving();
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

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey);
        manager.StartClient();
    }

    private void OnLobbyDisconnected(SteamServersDisconnected_t callback)
    {
        Debug.Log("Steam sunucularıyla bağlantı kesildi. Sonuç: " + callback.m_eResult);
    }

    private void OnLobbyKicked(GSClientKick_t callback)
    {
        Debug.Log("Kicked from lobby.");
    }

    public void ApplicationQuit()
    {
        ApplicationQuit();
    }
}
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
            print("Steam is not initialized, enabling SteamManager.");
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

    private void onJoinRequest(GameLobbyJoinRequested_t callback)
    {
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

    public void ApplicationQuit()
    {
        ApplicationQuit();
    }
    public void LeaveLobbyAndReturnToMenu()
    {
        // Steam lobby'den çık
        if (currentLobbyID != 0)
        {
            SteamMatchmaking.LeaveLobby((CSteamID)currentLobbyID);
            currentLobbyID = 0;
        }

        // Mirror bağlantılarını kapat
        if (NetworkManager.singleton != null)
        {
            if (NetworkServer.active && NetworkClient.isConnected)
                NetworkManager.singleton.StopHost();
            else if (NetworkClient.isConnected)
                NetworkManager.singleton.StopClient();
            else if (NetworkServer.active)
                NetworkManager.singleton.StopServer();
        }

        // Callback'leri temizle
        LobbyCreated = null;
        JoinRequest = null;
        LobbyEntered = null;

        Destroy(this.gameObject);
        // Ana menü sahnesine dön
        SceneManager.LoadScene(MainMenuSceneName);
    }
}
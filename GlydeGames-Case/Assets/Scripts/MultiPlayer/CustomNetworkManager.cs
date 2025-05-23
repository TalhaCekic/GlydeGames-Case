using System.Collections.Generic;
using Mirror;
using Mirror.FizzySteam;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    public static CustomNetworkManager instance;
    [SerializeField] private PlayerListObjController GamePlayerPrefab;
    [SerializeField] private PlayerData playerDataPrefab;
    public List<PlayerListObjController> GamePlayers { get; } = new List<PlayerListObjController>();
    public List<PlayerData> playerDatas { get; } = new List<PlayerData>();

    private PlayerListObjController GamePlayerDataInstance;

    Transform SelectSpawnPos;
    public List<GameObject> spawnPoints = new List<GameObject>();

    [Header("Day System")] 
    public int LastDay; 
    public float LastMoney; 

    public override void Start()
    {
        instance = this;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (spawnPoints != null)
        {
            GameObject[] spawnPos = GameObject.FindGameObjectsWithTag("SpawnPos");
            for (int i = 0; i < spawnPos.Length; i++)
            {
                spawnPoints.Add(spawnPos[i]);
            }

            Transform spawnPoint = spawnPoints[GamePlayers.Count].transform;

            GamePlayerDataInstance = Instantiate(GamePlayerPrefab, spawnPoint.position,
                spawnPoints[GamePlayers.Count].transform.rotation);

            GamePlayerDataInstance.ConnectionID = conn.connectionId;
            GamePlayerDataInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerDataInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
                (CSteamID)SteamLobby.instance.currentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerDataInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (GamePlayerDataInstance != null)
            {
                NetworkServer.RemovePlayerForConnection(conn, GamePlayerDataInstance.gameObject);
            }
        }
        else
        {
            NetworkServer.RemovePlayerForConnection(conn, playerPrefab);
        }
        
    }

    public override void OnServerChangeScene(string a)
    {
        if(LastDay==0) LastMoney = 1050;
        
        //GamePlayerDataInstance.transform.rotation = spawnPoints[GamePlayers.Count].transform.rotation;
        spawnPoints.Clear();
        GamePlayers.Clear();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        spawnPoints.Clear();
        GamePlayers.Clear();
        SteamLobby.instance.leavingToServer();
    }

    public void ResetData()
    {
        LastMoney = 1050;
        LastDay = 0;
    }
}
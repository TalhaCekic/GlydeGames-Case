using System.Collections.Generic;
using Mirror;
using Mirror.FizzySteam;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
	[SerializeField] private PlayerListObjController GamePlayerPrefab;
	[SerializeField] private PlayerData playerDataPrefab;
	public List<PlayerListObjController> GamePlayers { get; } = new List<PlayerListObjController>();
	public List<PlayerData> playerDatas { get; } = new List<PlayerData>();

	private PlayerListObjController GamePlayerDataInstance;
	private PlayerData playerData;

	Transform SelectSpawnPos;
	public List<GameObject> spawnPoints = new List<GameObject>();

	public ulong ToficID;
	public ulong TalosID;
	public ulong BongoID;
	public GameObject ToficTransform;
	public GameObject TalosTransform;
	public GameObject BongoTransform;
	
	//
	public int maxPacketSizeReliable = 5097152; // 5 MB
	public int maxPacketSizeUnreliable = 15000;  // 1.5 KB
	
	public override void Awake()
	{
		base.Awake();
		
		var fizzyTransport = GetComponent<FizzySteamworks>();
		// if (fizzyTransport != null)
		// {
		// 	var fieldInfo = typeof(FizzySteamworks).GetField("Channels", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		// 	if (fieldInfo != null)
		// 	{
		// 		var channels = (EP2PSend[])fieldInfo.GetValue(fizzyTransport);
		// 		for (int i = 0; i < channels.Length; i++)
		// 		{
		// 			if (channels[i] == EP2PSend.k_EP2PSendReliable || channels[i] == EP2PSend.k_EP2PSendReliableWithBuffering)
		// 			{
		// 				// Adjust maximum packet size for reliable channels
		// 				channels[i] = (EP2PSend)maxPacketSizeReliable;
		// 			}
		// 			else if (channels[i] == EP2PSend.k_EP2PSendUnreliable || channels[i] == EP2PSend.k_EP2PSendUnreliableNoDelay)
		// 			{
		// 				// Adjust maximum packet size for unreliable channels
		// 				channels[i] = (EP2PSend)maxPacketSizeUnreliable;
		// 			}
		// 		}
		// 		fieldInfo.SetValue(fizzyTransport, channels);
		// 	}
		// }
	}


	public override void Start() {
		GameObject[] spawnPos = GameObject.FindGameObjectsWithTag("SpawnPos");
		for (int i = 0; i < spawnPos.Length; i++)
		{
			spawnPoints.Add(spawnPos[i]);
		}
		ToficTransform = GameObject.FindGameObjectWithTag("ToficPos");
		TalosTransform = GameObject.FindGameObjectWithTag("TalosPos");
		BongoTransform = GameObject.FindGameObjectWithTag("BongoPos");
	}

	public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			if (spawnPoints != null)
			{
				Transform spawnPoint = spawnPoints[GamePlayers.Count].transform;
				
				GamePlayerDataInstance = Instantiate(GamePlayerPrefab, spawnPoint.position,new Quaternion(0,180,0,1));

				GamePlayerDataInstance.transform.rotation = spawnPoints[GamePlayers.Count].transform.rotation;
				
				GamePlayerDataInstance.ConnectionID = conn.connectionId;
				GamePlayerDataInstance.PlayerIdNumber = GamePlayers.Count + 1;
				GamePlayerDataInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
					(CSteamID)SteamLobby.instance.currentLobbyID, GamePlayers.Count);
				
				LobbySpawnerPos(GamePlayerDataInstance,GamePlayers.Count);
				LobbySpawnerPosLividy(GamePlayerDataInstance,GamePlayerDataInstance.PlayerSteamID ,GamePlayers.Count);

				NetworkServer.AddPlayerForConnection(conn, GamePlayerDataInstance.gameObject);
			}
		}
		else
		{
			GameObject[] spawnPos = GameObject.FindGameObjectsWithTag("SpawnPos");
			if (spawnPos != null)
			{
				int randomIndex = Random.Range(0, spawnPos.Length);
				Transform spawnPoint = spawnPos[randomIndex].transform; 
				
				playerData = Instantiate(playerDataPrefab,spawnPoint.position, Quaternion.identity);

				playerData.ConnectionID = conn.connectionId;
				playerData.PlayerIdNumber = playerDatas.Count + 1;
				playerData.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
					(CSteamID)SteamLobby.instance.currentLobbyID, GamePlayers.Count);

				NetworkServer.AddPlayerForConnection(conn, playerData.gameObject);
			}
		}
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn) {
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
	public override void OnStopClient() {
		base.OnStopClient();

		SteamLobby.instance.leaving();
	}

	public void LobbySpawnerPos(PlayerListObjController listObjController,int i)
	{
		LobbySpawner lobbySpawner = spawnPoints[i].GetComponent<LobbySpawner>();
		switch (lobbySpawner.stayPos)
		{
			case 0:
				listObjController.stayPos = 0;
				listObjController.InAnim(true);
				break;	
			case 1:
				listObjController.stayPos = 1;
				listObjController.UpAnim(true);
				
				break;		
			case 2:
				listObjController.stayPos = 2;
				listObjController.OutAnim(true);
				break;
		}
	}	
	 public void LobbySpawnerPosLividy(PlayerListObjController listObjController,ulong steamId,int i)
	 {
		 LobbySpawner lobbySpawner = spawnPoints[i].GetComponent<LobbySpawner>();
		 switch (lobbySpawner.stayPos)
		 {
			 case 0:
				 listObjController.stayPos = 0;
				 listObjController.InAnim(true);
				 break;	
			 case 1:
				 listObjController.stayPos = 1;
				 listObjController.UpAnim(true);
	 			
				 break;		
			 case 2:
				 listObjController.stayPos = 2;
				 listObjController.OutAnim(true);
				 break;
		 }
		 
	 	// if (TalosID ==steamId)
	 	// {
	 	// 	GamePlayerDataInstance.transform.position = TalosTransform.transform.position;
	 	// 	listObjController.IsTalos(true);
	 	// }	
	 	// else if (ToficID ==steamId)
	 	// {
	 	// 	GamePlayerDataInstance.transform.position = ToficTransform.transform.position;
	 	// 	listObjController.IsTofic(true);
		 //
	 	// }
	 	// else if (BongoID ==steamId)
	 	// {
	 	// 	GamePlayerDataInstance.transform.position = BongoTransform.transform.position;
	 	// 	listObjController.IsBongo(true);
	 	// }
	 	// else
	 	// {
	 	// 	LobbySpawner lobbySpawner = spawnPoints[i].GetComponent<LobbySpawner>();
	 	// 	switch (lobbySpawner.stayPos)
	 	// 	{
	 	// 		case 0:
	 	// 			listObjController.stayPos = 0;
	 	// 			listObjController.InAnim(true);
	 	// 			break;	
	 	// 		case 1:
	 	// 			listObjController.stayPos = 1;
	 	// 			listObjController.UpAnim(true);
	 	// 		
	 	// 			break;		
	 	// 		case 2:
	 	// 			listObjController.stayPos = 2;
	 	// 			listObjController.OutAnim(true);
	 	// 			break;
	 	// 	}
	 	// }
	}
}

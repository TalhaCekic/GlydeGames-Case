using Mirror;
using Steamworks;
using UnityEngine;

public class PlayerListObjController : NetworkBehaviour
{
    [SyncVar] public int stayPos;
    public Animator StayAnims;
    public GameObject PlayerListViewContant;
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;

    [SyncVar(hook = nameof(PlayerNameUpdate))]
    public string PlayerName;

    [SyncVar(hook = nameof(PlayerReadyUpdate))]
    public bool Ready;

    [SyncVar] public bool inAnim;
    [SyncVar] public bool outAnim;
    [SyncVar] public bool upAnim;
    [SyncVar] public bool isTofic;
    [SyncVar] public bool isTalos;
    [SyncVar] public bool isBongo;

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
    void Update()
    {
        if (isServer)
        {
            //LobbyAnimation();
        }
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.Ready = newValue;
            LobbyController.Instance.UpdatePlayerList();
        }
        else
        {
            this.Ready = newValue;
            LobbyController.Instance.UpdatePlayerList();
        }
    }
    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);

        if (Ready)
        {
            LobbyController.Instance.ReadyPlayerCount++;
            LobbyController.Instance.CheckIfAllReady();
        }
        else if (!Ready)
        {
            LobbyController.Instance.ReadyPlayerCount--;
            LobbyController.Instance.CheckIfAllReady();
        }
    }

    public void ChangeReady()
    {
        if (isOwned)
        {
            CmdSetPlayerReady();
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";

        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
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

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }
}
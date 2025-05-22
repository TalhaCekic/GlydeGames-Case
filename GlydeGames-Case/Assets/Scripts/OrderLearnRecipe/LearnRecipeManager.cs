using Mirror;
using UnityEngine;

public class LearnRecipeManager : NetworkBehaviour
{
    public static LearnRecipeManager instance;
    public RecipeDataList _ScribtableLearnRecipe;
    
    // public override void OnStartServer()
    // {
    //     base.OnStartServer();
    //     _PcCanvas.isReadyLearnedStart();
    // }
    //
    // public override void OnStartClient()
    // {
    //     base.OnStartClient();
    //     _PcCanvas.isReadyLearnedStart();
    // }
    void Start()
    {
        instance = this;
        if (isServer)
        {
            ServerStart();
        }
    }

    [Server]
    private void ServerStart()
    {
        RpcStart();
    }
    [ClientRpc]
    private void RpcStart()
    {
        
    }
    void Update()
    {
        if (isServer)
        {
            ServerUpdate();
        }
    }
    [Server]
    private void ServerUpdate()
    {
        RpcUpdate();
    }
    [ClientRpc]
    private void RpcUpdate()
    {
        
    }
}

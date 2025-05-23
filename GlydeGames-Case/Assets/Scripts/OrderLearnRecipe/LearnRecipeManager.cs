using Mirror;
using UnityEngine;

public class LearnRecipeManager : NetworkBehaviour
{
    public static LearnRecipeManager instance;
    public Datas _ScribtableLearnRecipe;
    
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

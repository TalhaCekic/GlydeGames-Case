
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOrder : NetworkBehaviour
{
    [Header("Ürün bilgisi")]
    public string OrderImageName;
    public Image OrderImage;
    public ScribtableOrderItem _scribtableOrderItem;
    [Header("Ürün Adedi")]
    [SyncVar] public int Number;
    public TMP_Text NumberText;

    void Start()
    {
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
        NumberText.text = Number.ToString();
        AddValues();
    }
    public void AddValues()
    {
        foreach (Sprite UıImage in _scribtableOrderItem.itemSpriteUI)
        {
            if (UıImage.name == OrderImageName)
            {
                OrderImage.sprite = UıImage;
            }
        }
    }

    void Update()
    {
        
    }



    [Server]
    public void ServerNumberAdd(int value)
    {
        Number += value;
        print("sayıekle");
    }  
    [Server]
    public void ServerNumberRemove(int value)
    {
        Number -= value;
    }

 
}

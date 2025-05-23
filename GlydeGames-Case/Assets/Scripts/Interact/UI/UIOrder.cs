using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOrder : NetworkBehaviour
{
    [Header("Ürün bilgisi")] [SyncVar] public string OrderImageName;
    public TMP_Text OrderName;
    public Image OrderImage;
    [SyncVar] public int value;
    [Header("Ürün Adedi")] [SyncVar] public int Number;
    public TMP_Text NumberText;

    public Datas RecipeOrderDataList;

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
        AddValues();
    }

    private void AddValues()
    {
        NumberText.text = Number.ToString();
        OrderName.text = OrderImageName;
        for (int i = 0; i < RecipeOrderDataList._MenuDatas.Length; i++)
        {
            switch (value)
            {
                case 1:
                    for (int a = 0; a < RecipeOrderDataList._foodData.Length; a++)
                    {
                        if (OrderImageName == RecipeOrderDataList._foodData[a]._name)
                        {
                            OrderImage.sprite = RecipeOrderDataList._foodData[a]._sprite; // Sprite atanıyor
                        }
                    }

                    break;
                case 2:
                    for (int a = 0; a < RecipeOrderDataList._drinkData.Length; a++)
                    {
                        if (OrderImageName == RecipeOrderDataList._drinkData[a]._name)
                        {
                            OrderImage.sprite = RecipeOrderDataList._drinkData[a]._sprite; // Sprite atanıyor
                        }
                    }

                    break;
                case 3:
                    for (int a = 0; a < RecipeOrderDataList._snackData.Length; a++)
                    {
                        if (OrderImageName == RecipeOrderDataList._snackData[a]._name)
                        {
                            OrderImage.sprite = RecipeOrderDataList._snackData[a]._sprite; // Sprite atanıyor
                        }
                    }

                    break;
            }
        }
    }
}
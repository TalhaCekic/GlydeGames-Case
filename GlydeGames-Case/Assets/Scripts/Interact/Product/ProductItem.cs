using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class ProductItem : NetworkBehaviour
{
    public static ProductItem instance;
    [Header("Scripts Data")] public AddToCartManager addCartManager;
    [SyncVar] public bool isItemAlreadyAdded;

    public Datas ItemData;

    [SyncVar] public int _totalAmount;

    [Header("Buying")] public Button _BuyButton;

    private void Awake()
    {
        instance = this;
    }

    public void AddCart(int index, VisualElement Cart, VisualTreeAsset _template, ScrollView CartList, int _amount,
        Label _totalAmountLabel)
    {
        isItemAlreadyAdded =
            addCartManager.ItemCardItemList.Exists(item => item.name == ItemData._categoryData[index]._name);

        if (!isItemAlreadyAdded || addCartManager.BuyCardItemList.Count == 0)
        {
            Cart = _template.CloneTree();

            Cart.Q<VisualElement>("CardToCart"); // Sepetteki kartı bul
            Label CardName = Cart.Q<Label>("CardNameText"); // isim  text bul
            Label TotalAmount = Cart.Q<Label>("TotalAmountTText"); // Toplam Tutar Text bul
            VisualElement testAvatar = Cart.Q<VisualElement>("Avatar"); // avatar bul

            Cart.name = ItemData._categoryData[index]._name;

            CardName.text = _amount + " " + ItemData._categoryData[index]._name;
            int newPurchaseAmount = ItemData._categoryData[index]._buyValue * _amount;
            TotalAmount.text = newPurchaseAmount.ToString();
            testAvatar.style.backgroundImage = ItemData._categoryData[index]._image;

            //TOTAL PARA EKLER
            _totalAmount += ItemData._categoryData[index]._buyValue * _amount;
            _totalAmountLabel.text = _totalAmount.ToString();

            CartList.Add(Cart);

            addCartManager.BuyCardItemList.Add(Cart);
            for (int i = 0; i < _amount; i++)
            {
                addCartManager.BuyProductItemList.Add(ItemData._categoryData[index].ObjPrefab);
            }

            addCartManager.ItemCardItemList.Add(new Item(ItemData._categoryData[index]._name,
                ItemData._categoryData[index]._buyValue, ItemData._categoryData[index]._amount,
                _totalAmount));
        }
        else
        {
            foreach (VisualElement element in addCartManager.BuyCardItemList)
            {
                if (element.name == ItemData._categoryData[index]._name)
                {
                    foreach (var item in addCartManager.ItemCardItemList)
                    {
                        if (element.name == item.name)
                        {
                            item.currentAmount += _amount;
                            item.TotalCardAmount = item.currentAmount * ItemData._categoryData[index]._buyValue;

                            Label CardName = element.Q<Label>("CardNameText"); // isim  text bul
                            Label TotalAmount = element.Q<Label>("TotalAmountTText"); // Toplam Tutar Text bul

                            CardName.text = item.currentAmount + " " + ItemData._categoryData[index]._name;
                            TotalAmount.text = item.TotalCardAmount.ToString();

                            //TOTAL PARA EKLER
                            _totalAmount += item.saleAmount * _amount;
                            _totalAmountLabel.text = _totalAmount.ToString();

                            for (int i = 0; i < _amount; i++)
                            {
                                addCartManager.BuyProductItemList.Add(ItemData._categoryData[index].ObjPrefab);
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }
    }

    public void BuyButton(ScrollView CartList, Label _totalAmountLabel, Button _buyButton)
    {
        addCartManager.ServerBuyBoxItemList(CartList);
        _totalAmountLabel.text = _totalAmount.ToString();
        _BuyButton = _buyButton;
    }

    public void LearnRecipeButton(ScrollView CartList, int buyRecipe)
    {
        addCartManager.LearnedBuyRecipe(CartList, buyRecipe);
    }


    [Server]
    public void ServerBuyButtonSettings(bool value)
    {
        ClientBuyButtonSettings(value);
    }

    [ClientRpc]
    private void ClientBuyButtonSettings(bool value)
    {
        if(_BuyButton ==null)return;
        if (value)
        {
            _BuyButton.SetEnabled(false);
        }
        else
        {
            _BuyButton.SetEnabled(true);
        }
    }
}
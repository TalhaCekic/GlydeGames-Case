using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ProductItem : NetworkBehaviour
{
    [Header("Scripts Data")] public AddToCartManager addCartManager;
    [SyncVar] public bool isItemAlreadyAdded;

    public ScibtableProductBox scibtableProductBox;
    public DataList _productItemData;

    [SyncVar] public int _totalAmount;
    
    public void AddCart(int index, VisualElement Cart, VisualTreeAsset _template, ScrollView CartList, int _amount,
        Label _totalAmountLabel, List<Button> _cartDeleteCardButton)
    {
        isItemAlreadyAdded =
            addCartManager.ItemCardItemList.Exists(item => item.name == _productItemData.dataList[index]._Name);

        if (!isItemAlreadyAdded || addCartManager.BuyCardItemList.Count == 0)
        {
            Cart = _template.CloneTree();

            Cart.Q<VisualElement>("CardToCart"); // Sepetteki kartı bul
            Label CardName = Cart.Q<Label>("CardNameText"); // isim  text bul
            Label TotalAmount = Cart.Q<Label>("TotalAmountTText"); // Toplam Tutar Text bul
            VisualElement testAvatar = Cart.Q<VisualElement>("Avatar"); // avatar bul
            
            Cart.name = _productItemData.dataList[index]._Name;

            CardName.text = _amount + " " + _productItemData.dataList[index]._Name;
            int newPurchaseAmount = _productItemData.dataList[index]._PurchaseAmount * _amount;
            TotalAmount.text = newPurchaseAmount.ToString();
            //TotalAmount.text = _productItemData.dataList[index]._PurchaseAmount.ToString();
            testAvatar.style.backgroundImage = _productItemData.dataList[index]._Image;

            //TOTAL PARA EKLER
            _totalAmount += _productItemData.dataList[index]._PurchaseAmount * _amount;
            _totalAmountLabel.text = _totalAmount.ToString();

            CartList.Add(Cart);

            addCartManager.BuyCardItemList.Add(Cart);
            for (int i = 0; i < _amount; i++)
            {
                addCartManager.BuyProductItemList.Add(scibtableProductBox.Objs[index]);
            }
            addCartManager.ItemCardItemList.Add(new Item(_productItemData.dataList[index]._Name,
                _productItemData.dataList[index]._PurchaseAmount, _productItemData.dataList[index]._CurrentAmount,
                _productItemData.dataList[index]._PurchaseAmount));
            
            // silme butonu eklenir
            Button cartDeleteCardButton = Cart.Q<Button>("DeleteButtons"); // silme butonu bul
            cartDeleteCardButton.name = _productItemData.dataList[index]._Name;
            _cartDeleteCardButton.Add(cartDeleteCardButton);
        }
        else
        {
            foreach (VisualElement element in addCartManager.BuyCardItemList)
            {
                if (element.name == _productItemData.dataList[index]._Name)
                {
                    foreach (var item in addCartManager.ItemCardItemList)
                    {
                        if (element.name ==item.name)
                        {
                            
                            item.currentAmount += _amount;
                            item.TotalCardAmount = item.currentAmount * _productItemData.dataList[index]._PurchaseAmount;
                            
                            Label CardName = element.Q<Label>("CardNameText"); // isim  text bul
                            Label TotalAmount = element.Q<Label>("TotalAmountTText"); // Toplam Tutar Text bul
                            
                            CardName.text = item.currentAmount + " " + _productItemData.dataList[index]._Name;
                            TotalAmount.text = item.TotalCardAmount.ToString();
                            
                            //TOTAL PARA EKLER
                            _totalAmount += item.saleAmount * _amount;
                            _totalAmountLabel.text = _totalAmount.ToString();
                            
                            for (int i = 0; i < _amount; i++)
                            {
                                addCartManager.BuyProductItemList.Add(scibtableProductBox.Objs[index]);
                            }
                            break;
                        }
                    }
                    break;
                }
                
            }
        }
    }

    public void BuyButton(ScrollView CartList, Label _totalAmountLabel)
    {
        addCartManager.ServerBuyBoxItemList(CartList);
        _totalAmountLabel.text = _totalAmount.ToString();
    }
    public void LearnRecipeButton(ScrollView CartList, Label _totalAmountLabel,int buyRecipe)
    {
        addCartManager.LearnedBuyRecipe(CartList,buyRecipe);
    }


}
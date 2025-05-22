using System;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RegisterItemProduct : NetworkBehaviour
{
	[Header("Scripts Data")] 
	public RegisterAddToCartManager registerAddToCartManager;
	[SyncVar] public bool isItemAlreadyAdded; 

    [SyncVar] public int _totalAmount;
	
	public RecipeDataList[] CategoryDataList;

	//[Server]
    public void AddCart(int index, int categoryIndex, VisualElement Cart, VisualTreeAsset _template, ScrollView CartList, int _amount, Label _totalAmountLabel, List<Button> _cartDeleteCardButton)
    {
        isItemAlreadyAdded = registerAddToCartManager.ItemCardItemList.Exists(item => item.name == CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._name);

        if (!isItemAlreadyAdded || registerAddToCartManager.BuyCardItemList.Count == 0)
        {
            Cart = _template.CloneTree();

            Cart.Q<VisualElement>("CardToCart"); // Sepetteki kartı bul
            Label CardName = Cart.Q<Label>("CardNameText"); // isim  text bul
            Label TotalAmount = Cart.Q<Label>("TotalAmountTText"); // Toplam Tutar Text bul
            VisualElement Avatar = Cart.Q<VisualElement>("Avatar"); // avatar bul

            Cart.name = CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._name;
            CardName.text = _amount + " " + CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._name;
            TotalAmount.text = CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._sellAmount.ToString();
            Avatar.style.backgroundImage = CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._ımage;

            //TOTAL PARA EKLER
            _totalAmount += CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._sellAmount * _amount;
            _totalAmountLabel.text = _totalAmount.ToString();

            CartList.Add(Cart);

            registerAddToCartManager.BuyCardItemList.Add(Cart);
            //registerAddToCartManager.BuyProductItemList.Add(CategoryDataList[categoryIndex].Objs[index]);
            registerAddToCartManager.ItemCardItemList.Add(new RegisterItem(CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._name, CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._sellAmount, _amount,
                CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._sellAmount));
            // silme butonu eklenir
            Button cartDeleteCardButton = Cart.Q<Button>("DeleteButtons"); // silme butonu bul
            cartDeleteCardButton.name = CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._name;
            _cartDeleteCardButton.Add(cartDeleteCardButton);
        }
        else
        {
            foreach (VisualElement element in registerAddToCartManager.BuyCardItemList)
            {
                if (element.name == CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._name)
                {
                    foreach (var item in registerAddToCartManager.ItemCardItemList)
                    {
                        if (element.name == item.name)
                        {
                            item.currentAmount += _amount;
                            item.TotalCardAmount = item.currentAmount * CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._sellAmount;

                            print(item.currentAmount);

                            Label CardName = element.Q<Label>("CardNameText"); // isim  text bul
                            Label TotalAmount = element.Q<Label>("TotalAmountTText"); // Toplam Tutar Text bul

                            CardName.text = item.currentAmount + " " + CategoryDataList[categoryIndex]._ScribtableLearnableItems[index]._name;
                            TotalAmount.text = item.TotalCardAmount.ToString();

                            //TOTAL PARA EKLER
                            _totalAmount += item.saleAmount * _amount;
                            _totalAmountLabel.text = _totalAmount.ToString();

                           // registerAddToCartManager.BuyProductItemList.Add(CategoryDataList[categoryIndex].Objs[index]);
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
        registerAddToCartManager.ServerBuyCustomerItemList(CartList,_totalAmountLabel);
        //_totalAmountLabel.text = _totalAmount.ToString();
    }
}

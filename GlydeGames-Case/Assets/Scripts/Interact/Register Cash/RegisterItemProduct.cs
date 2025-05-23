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
    public CustomerManager _customerManager;
    public RegisterAddToCartManager _registerAddToCartManager;

	[SyncVar] public bool isItemAlreadyAdded; 

    [SyncVar] public int _totalAmount;
	
	public Datas CategoryDataList;

    public BurgerScreen _BurgerScreen;
    public SnackScreen _SnackScreen;

    public void AddCart(int index, VisualElement Cart, VisualTreeAsset _template,
        ScrollView CartList, int _amount, Label _totalAmountLabel, List<Button> _cartDeleteCardButton, string name,Texture2D avatar,int sellValue,int categoryIndex)
    {
        isItemAlreadyAdded = registerAddToCartManager.ItemCardItemList.Exists(item => item.name == CategoryDataList._categoryData[index]._name);
        
        if (!isItemAlreadyAdded || registerAddToCartManager.BuyCardItemList.Count == 0)
        {
            Cart = _template.CloneTree();
        
            Cart.Q<VisualElement>("CardToCart"); // Sepetteki kartı bul
            Label CardName = Cart.Q<Label>("CardNameText"); // isim  text bul
            Label TotalAmount = Cart.Q<Label>("TotalAmountTText"); // Toplam Tutar Text bul
            VisualElement Avatar = Cart.Q<VisualElement>("Avatar"); // avatar bul
        
            Cart.name = name;
            CardName.text = _amount + " " + name;
            int newPurchaseAmount = sellValue * _amount;
            TotalAmount.text = newPurchaseAmount.ToString();
            Avatar.style.backgroundImage = avatar;
        
            //TOTAL PARA EKLER
            _totalAmount += sellValue * _amount;
            _totalAmountLabel.text = _totalAmount.ToString();
        
            CartList.Add(Cart);
            
            switch (categoryIndex)
            {
                case 0:
                    _BurgerScreen.AddCart(name,false,0);
                    break;
                case 1:
                    _SnackScreen.AddCart(name,false,0);
                    break;
                case 2:
                    _SnackScreen.AddCart(name,false,0);
                    break;
            }
            
        
            registerAddToCartManager.BuyCardItemList.Add(Cart);
            registerAddToCartManager.ItemCardItemList.Add(new RegisterItem(name, sellValue, _amount, newPurchaseAmount));
                
            foreach (var orderItem in _customerManager.OrderStayQueue[0].GetComponent<Customer>().orderItems)
            {
                foreach (var RegisterItem in _registerAddToCartManager.BuyCardItemList)
                {
                    if (RegisterItem.name.Contains(orderItem.MealName))
                    {
                        _registerAddToCartManager.isFood = true;
                    }

                    if (RegisterItem.name.Contains(orderItem.DrinkName))
                    {
                        _registerAddToCartManager.isDrink = true;
                    }

                    if (RegisterItem.name.Contains(orderItem.SnackName))
                    {
                        _registerAddToCartManager.isSnack = true;
                    }
                }
            }
        }
        else
        {
            foreach (VisualElement element in registerAddToCartManager.BuyCardItemList)
            {
                if (element.name == name)
                {
                    foreach (var item in registerAddToCartManager.ItemCardItemList)
                    {
                        if (element.name == item.name)
                        {
                            item.currentAmount += _amount;
                            item.TotalCardAmount = item.currentAmount * sellValue;
                            
                            Label CardName = element.Q<Label>("CardNameText"); // isim  text bul
                            Label TotalAmount = element.Q<Label>("TotalAmountTText"); // Toplam Tutar Text bul
        
                            CardName.text = item.currentAmount + " " + name;
                            TotalAmount.text = item.TotalCardAmount.ToString();
        
                            //TOTAL PARA EKLER
                            _totalAmount += item.saleAmount * _amount;
                            _totalAmountLabel.text = _totalAmount.ToString();
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

using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class CashRegisterCanvas : NetworkBehaviour
{
    public RegisterItemProduct RegisterItem;
    public UIDocument _document;

    public int DrinksCardAmount;
    public int SnacksCardAmount;
    public int MealsCardAmount;

    // Güncel isimler text
    private Label[] _cardDrinksNameTexts;
    private Label[] _cardSnacksNameTexts;
    private Label[] _cardMealsNameTexts;

    // Güncel Avatar visual element
    private VisualElement[] _cardDrinksAvatars;
    private VisualElement[] _cardSnacksAvatars;
    private VisualElement[] _cardMealsAvatars;

    // Güncel tutar text
    private Label[] _cardDrinksSellAmount;
    private Label[] _cardSnackSellAmount;
    private Label[] _cardMealsSellAmount;

    // Sepete ekleme butonu
    private Button[] _addDrinksCardButtons;
    private Button[] _addSnacksCardButtons;
    private Button[] _addMealsCardButtons;

    [Header("Category Button")] private Button _drinksButton;
    private Button _snackButton;
    private Button _mealsButton;

    [Header("Category Button")] private VisualElement _drinksPanel;
    private VisualElement _snackPanel;
    private VisualElement _mealsPanel;

    // Sipariş verilen buton
    private Button _buyButton;

    // Sipariş toplam tutarı text
    private Label _totalAmountText;

    // ürün silme
    private List<Button> _cartDeleteCardButton = new List<Button>();

    private Button deleteButton;

    // Scroll view
    private ScrollView CartList;
    private VisualElement VisuelCart;
    public VisualTreeAsset Template;

    public override void OnStartServer()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        base.OnStartServer();
        isReadyProductStart();
    }

    public override void OnStartClient()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        base.OnStartClient();
        isReadyProductStart();
    }

    public void isReadyProductStart()
    {
        ProductAddeValueCard(); // itemlerin yazdırılması
    }

    private void ProductAddeValueCard()
    {
        var root = _document.rootVisualElement;

        DrinksCardAmount = RegisterItem.CategoryDataList._drinkData.Length;
        SnacksCardAmount = RegisterItem.CategoryDataList._snackData.Length;
        MealsCardAmount = RegisterItem.CategoryDataList._foodData.Length;
        // card ayarları ekle
        _cardDrinksNameTexts = new Label[DrinksCardAmount];
        _cardSnacksNameTexts = new Label[SnacksCardAmount];
        _cardMealsNameTexts = new Label[MealsCardAmount];
        //
        _cardDrinksAvatars = new VisualElement[DrinksCardAmount];
        _cardSnacksAvatars = new VisualElement[SnacksCardAmount];
        _cardMealsAvatars = new VisualElement[MealsCardAmount];
        //
        _cardDrinksSellAmount = new Label[DrinksCardAmount];
        _cardSnackSellAmount = new Label[SnacksCardAmount];
        _cardMealsSellAmount = new Label[MealsCardAmount];
        // atamları yap
        for (int i = 0; i < DrinksCardAmount; i++)
        {
            _cardDrinksNameTexts[i] = root.Q<Label>($"CardNameTextDrink{i + 1}");
            _cardDrinksAvatars[i] = root.Q<VisualElement>($"AvatarDrink{i + 1}");
            _cardDrinksSellAmount[i] = root.Q<Label>($"SellAmountDrink{i + 1}");
        }

        for (int i = 0; i < SnacksCardAmount; i++)
        {
            _cardSnacksNameTexts[i] = root.Q<Label>($"CardNameTextSnack{i + 1}");
            _cardSnacksAvatars[i] = root.Q<VisualElement>($"AvatarSnack{i + 1}");
            _cardSnackSellAmount[i] = root.Q<Label>($"SellAmountSnack{i + 1}");
        }

        for (int i = 0; i < MealsCardAmount; i++)
        {
            _cardMealsNameTexts[i] = root.Q<Label>($"CardNameTextMeal{i + 1}");
            _cardMealsAvatars[i] = root.Q<VisualElement>($"AvatarMeal{i + 1}");
            _cardMealsSellAmount[i] = root.Q<Label>($"SellAmountMeal{i + 1}");
        }

        // girdileri işle
        for (int i = 0; i < DrinksCardAmount; i++)
        {
            _cardDrinksNameTexts[i].text = RegisterItem.CategoryDataList._drinkData[i]._name;
            _cardDrinksAvatars[i].style.backgroundImage = RegisterItem.CategoryDataList._drinkData[i]._image;
            _cardDrinksSellAmount[i].text = RegisterItem.CategoryDataList._drinkData[i]._saleValue.ToString();
        }

        for (int i = 0; i < SnacksCardAmount; i++)
        {
            _cardSnacksNameTexts[i].text = RegisterItem.CategoryDataList._snackData[i]._name;
            _cardSnacksAvatars[i].style.backgroundImage = RegisterItem.CategoryDataList._snackData[i]._image;
            _cardSnackSellAmount[i].text = RegisterItem.CategoryDataList._snackData[i]._saleValue.ToString();
        }

        for (int i = 0; i < MealsCardAmount; i++)
        {
            _cardMealsNameTexts[i].text = RegisterItem.CategoryDataList._foodData[i]._name;
            _cardMealsAvatars[i].style.backgroundImage = RegisterItem.CategoryDataList._foodData[i]._image;
            _cardMealsSellAmount[i].text = RegisterItem.CategoryDataList._foodData[i]._saleValue.ToString();
        }

        // paneller
        _drinksPanel = root.Q<VisualElement>("DrinksPanel");
        _snackPanel = root.Q<VisualElement>("SnackPanel");
        _mealsPanel = root.Q<VisualElement>("MealPanel");

        _drinksPanel.style.display = DisplayStyle.None;
        _snackPanel.style.display = DisplayStyle.None;
        _mealsPanel.style.display = DisplayStyle.None;
    }

    void OnEnable()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        _document.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) =>
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100, Color.magenta);

            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit, 100f, LayerMask.GetMask("UI")))
            {
                return invalidPosition;
            }

            Vector2 pixelUV = hit.textureCoord;

            pixelUV.y = 1 - pixelUV.y;
            pixelUV.x *= _document.panelSettings.targetTexture.width;
            pixelUV.y *= _document.panelSettings.targetTexture.height;

            // var cursor = _document.rootVisualElement.Q<VisualElement>("Cursor");
            // if (cursor != null)
            // {
            //     cursor.style.left = pixelUV.x;
            //     cursor.style.top = pixelUV.y;
            // }
            return pixelUV;
        });
    }

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        GameObject cashRegister = GameObject.Find("CashRegister");
        if (cashRegister == null) return;
        _document = cashRegister.GetComponent<UIDocument>();
        RegisterItem = cashRegister.GetComponent<RegisterItemProduct>();

        AddedCardButton(); // seçilen ürünü sepete ekler 

        ClickEvent(); // basılan butonu çalıştırma
    }

    private void AddedCardButton()
    {
        DrinksCardAmount = RegisterItem.CategoryDataList._drinkData.Length;
        SnacksCardAmount = RegisterItem.CategoryDataList._snackData.Length;
        MealsCardAmount = RegisterItem.CategoryDataList._foodData.Length;
        // butonları oluştur
        _addDrinksCardButtons = new Button[DrinksCardAmount];
        _addSnacksCardButtons = new Button[SnacksCardAmount];
        _addMealsCardButtons = new Button[MealsCardAmount];

        var root = _document.rootVisualElement;

        for (int i = 0; i < DrinksCardAmount; i++)
        {
            _addDrinksCardButtons[i] = root.Q<Button>($"AddtoCardButtonDrink{i + 1}");
        }

        for (int i = 0; i < SnacksCardAmount; i++)
        {
            _addSnacksCardButtons[i] = root.Q<Button>($"AddtoCardButtonSnack{i + 1}");
        }

        for (int i = 0; i < MealsCardAmount; i++)
        {
            _addMealsCardButtons[i] = root.Q<Button>($"AddtoCardButtonMeal{i + 1}");
        }

        _drinksButton = root.Q<Button>("DrinksButton");
        _snackButton = root.Q<Button>("SnackButton");
        _mealsButton = root.Q<Button>("MealsButton");
        
        deleteButton = root.Q<Button>("DeleteButton");

        // paneller
        _drinksPanel = root.Q<VisualElement>("DrinksPanel");
        _snackPanel = root.Q<VisualElement>("SnackPanel");
        _mealsPanel = root.Q<VisualElement>("MealPanel");

        _drinksPanel.style.display = DisplayStyle.None;
        _snackPanel.style.display = DisplayStyle.None;
        _mealsPanel.style.display = DisplayStyle.None;

        _buyButton = root.Q<Button>("BuyButton");
    }

    private void ClickEvent()
    {
        DrinksCardAmount = RegisterItem.CategoryDataList._drinkData.Length;
        SnacksCardAmount = RegisterItem.CategoryDataList._snackData.Length;
        MealsCardAmount = RegisterItem.CategoryDataList._foodData.Length;
        for (int i = 0; i < DrinksCardAmount; i++)
        {
            int index = i;
            if (_addDrinksCardButtons != null)
            {
                _addDrinksCardButtons[i].clicked += () => OnClickButton(index, 0);
            }
        }

        for (int i = 0; i < SnacksCardAmount; i++)
        {
            int index = i;
            if (_addSnacksCardButtons != null)
            {
                _addSnacksCardButtons[i].clicked += () => OnClickButton(index, 1);
            }
        }

        for (int i = 0; i < MealsCardAmount; i++)
        {
            int index = i;
            if (_addMealsCardButtons != null)
            {
                _addMealsCardButtons[i].clicked += () => OnClickButton(index, 2);
            }
        }

        _drinksButton.clicked += () => OnDrinkClickButton(); // içecek kategorisi aç
        _snackButton.clicked += () => OnSnackClickButton(); // atıştırmalık kategorisi aç
        _mealsButton.clicked += () => OnMeatClickButton(); // yiyecek kategorisi aç

        _buyButton.clicked += () => BuyClickButton();
        
        deleteButton.clicked += () => OnDeleteButton();
    }
    //sepete ekleme eylemi
    private void OnDeleteButton()
    {
        if (!isLocalPlayer) return;
        CommandOnDeleteButton();
    }

    [Command]
    private void CommandOnDeleteButton()
    {
        RpcOnDeleteButton();
    }

    [ClientRpc]
    private void RpcOnDeleteButton()
    {
        var root = _document.rootVisualElement;
        CartList = root.Q<ScrollView>("CartListView");
        _totalAmountText = root.Q<Label>("TotalAmountSell");

        RegisterItem._totalAmount = 0;
        _totalAmountText.text = RegisterItem._totalAmount.ToString();
        
        CartList.Clear();
        RegisterItem.registerAddToCartManager.ItemCardItemList.Clear();
        RegisterItem.registerAddToCartManager.BuyCardItemList.Clear();

     
            RegisterItem._BurgerScreen.ListClear(); 
            RegisterItem._SnackScreen.ListClear();
        

        RegisterItem._registerAddToCartManager.isDrink = false;
        RegisterItem._registerAddToCartManager.isSnack = false;
        RegisterItem._registerAddToCartManager.isFood = false;
    }

    //sepete ekleme eylemi
    private void OnClickButton(int index, int categoryIndex)
    {
        if (!isLocalPlayer) return;
        CommandAddOrderButton(index, categoryIndex);
    }

    [Command]
    private void CommandAddOrderButton(int index, int categoryIndex)
    {
        RpcAddOrderButton(index, categoryIndex);
    }

    [ClientRpc]
    private void RpcAddOrderButton(int index, int categoryIndex)
    {
        var root = _document.rootVisualElement;
        CartList = root.Q<ScrollView>("CartListView");
        _totalAmountText = root.Q<Label>("TotalAmountSell");
        
        switch (categoryIndex)
        {
            case 0:
                if(RegisterItem._registerAddToCartManager.isDrink)return;
                RegisterItem.AddCart(index, VisuelCart, Template, CartList,
                    1, _totalAmountText, _cartDeleteCardButton, RegisterItem.CategoryDataList._drinkData[index]._name,
                    RegisterItem.CategoryDataList._drinkData[index]._image,
                    RegisterItem.CategoryDataList._drinkData[index]._saleValue,1);
                break;
            case 1:
                if(RegisterItem._registerAddToCartManager.isSnack)return;
                RegisterItem.AddCart(index, VisuelCart, Template, CartList,
                    1, _totalAmountText, _cartDeleteCardButton, RegisterItem.CategoryDataList._snackData[index]._name,
                    RegisterItem.CategoryDataList._snackData[index]._image,
                    RegisterItem.CategoryDataList._snackData[index]._saleValue,2);
                break;
            case 2:
                if(RegisterItem._registerAddToCartManager.isFood)return;
                RegisterItem.AddCart(index, VisuelCart, Template, CartList,
                    1, _totalAmountText, _cartDeleteCardButton, RegisterItem.CategoryDataList._foodData[index]._name,
                    RegisterItem.CategoryDataList._foodData[index]._image,
                    RegisterItem.CategoryDataList._foodData[index]._saleValue,0);
                break;
        }
    }

    // panel ayaları  /////
    // Drinks Button Panel ayarı
    private void OnDrinkClickButton()
    {
        if (!isLocalPlayer) return;
        if (RegisterItem._registerAddToCartManager._customerManager.OrderStayQueue.Count > 0)
        {
            CommandCategoryDrinkButton();
        }
        else
        {
            RpcCommandCategoryDrinkButtonButton2();
        }
    }

    [Command]
    private void CommandCategoryDrinkButton()
    {
        RpcCommandCategoryDrinkButtonButton();
    }

    [ClientRpc]
    private void RpcCommandCategoryDrinkButtonButton()
    {
        var root = _document.rootVisualElement;

        _drinksPanel = root.Q<VisualElement>("DrinksPanel");
        _snackPanel = root.Q<VisualElement>("SnackPanel");
        _mealsPanel = root.Q<VisualElement>("MealPanel");

        _drinksPanel.style.display = DisplayStyle.Flex;
        _snackPanel.style.display = DisplayStyle.None;
        _mealsPanel.style.display = DisplayStyle.None;
    }    [ClientRpc]
    private void RpcCommandCategoryDrinkButtonButton2()
    {
        var root = _document.rootVisualElement;

        _drinksPanel = root.Q<VisualElement>("DrinksPanel");
        _snackPanel = root.Q<VisualElement>("SnackPanel");
        _mealsPanel = root.Q<VisualElement>("MealPanel");

        _drinksPanel.style.display = DisplayStyle.None;
        _snackPanel.style.display = DisplayStyle.None;
        _mealsPanel.style.display = DisplayStyle.None;
    }

    // snack Button Panel ayarı
    private void OnSnackClickButton()
    {
        if (!isLocalPlayer) return;
        if (RegisterItem._registerAddToCartManager._customerManager.OrderStayQueue.Count > 0)
        {
            CommandCategorySnackButton();
        }
        else
        {
            RpcCommandCategorySnackButtonButton2();
        }
        
    }

    [Command]
    private void CommandCategorySnackButton()
    {
        RpcCommandCategorySnackButtonButton();
    }

    [ClientRpc]
    private void RpcCommandCategorySnackButtonButton()
    {
        var root = _document.rootVisualElement;

        _drinksPanel = root.Q<VisualElement>("DrinksPanel");
        _snackPanel = root.Q<VisualElement>("SnackPanel");
        _mealsPanel = root.Q<VisualElement>("MealPanel");

        _drinksPanel.style.display = DisplayStyle.None;
        _snackPanel.style.display = DisplayStyle.Flex;
        _mealsPanel.style.display = DisplayStyle.None;
    } 
    [ClientRpc]
    private void RpcCommandCategorySnackButtonButton2()
    {
        var root = _document.rootVisualElement;

        _drinksPanel = root.Q<VisualElement>("DrinksPanel");
        _snackPanel = root.Q<VisualElement>("SnackPanel");
        _mealsPanel = root.Q<VisualElement>("MealPanel");

        _drinksPanel.style.display = DisplayStyle.None;
        _snackPanel.style.display = DisplayStyle.None;
        _mealsPanel.style.display = DisplayStyle.None;
    }

    // meat Button Panel ayarı
    private void OnMeatClickButton()
    {
        if (!isLocalPlayer) return;
        if (RegisterItem._registerAddToCartManager._customerManager.OrderStayQueue.Count > 0)
        {
            CommandCategoryMeatButton();
        }
        else
        {
            RpcCommandCategoryMeatButtonButton2();
        }
    }

    [Command]
    private void CommandCategoryMeatButton()
    {
        RpcCommandCategoryMeatButtonButton();
    }

    [ClientRpc]
    private void RpcCommandCategoryMeatButtonButton()
    {
        var root = _document.rootVisualElement;

        _drinksPanel = root.Q<VisualElement>("DrinksPanel");
        _snackPanel = root.Q<VisualElement>("SnackPanel");
        _mealsPanel = root.Q<VisualElement>("MealPanel");

        _drinksPanel.style.display = DisplayStyle.None;
        _snackPanel.style.display = DisplayStyle.None;
        _mealsPanel.style.display = DisplayStyle.Flex;
    }
    [ClientRpc]
    private void RpcCommandCategoryMeatButtonButton2()
    {
        var root = _document.rootVisualElement;

        _drinksPanel = root.Q<VisualElement>("DrinksPanel");
        _snackPanel = root.Q<VisualElement>("SnackPanel");
        _mealsPanel = root.Q<VisualElement>("MealPanel");

        _drinksPanel.style.display = DisplayStyle.None;
        _snackPanel.style.display = DisplayStyle.None;
        _mealsPanel.style.display = DisplayStyle.None;
    }

    // Müşterinin ürün fiyatlarını keser
    private void BuyClickButton()
    {
        if (!isLocalPlayer) return;
        CommandBuyButton();
    }

    [Command]
    private void CommandBuyButton()
    {
        RpcCommandBuyButtonButton();
    }

    [ClientRpc]
    private void RpcCommandBuyButtonButton()
    {
        var root = _document.rootVisualElement;
        CartList = root.Q<ScrollView>("CartListView");
        _totalAmountText = root.Q<Label>("TotalAmountSell");
        //RegisterItem.registerAddToCartManager.ServerBuyCustomerItemList(CartList,_totalAmountText);
        RegisterItem.BuyButton(CartList, _totalAmountText);
    }
}
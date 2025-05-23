using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PcCanvas : NetworkBehaviour
{
    public ProductItem _ProductItem;
    public LaptopInteract _LaptopInteract;
    public LearnRecipeManager _LearnRecipeManager;
    public UIDocument _document;

    public int OrderCardAmount;
    public int LearnCardAmount;

    private bool isChangeLearned;

    [Header("Pc Menu")] private VisualElement _pcCanvasPanel;
    private VisualElement _pcShopCanvasPanel;
    private VisualElement _pcOrderCanvasPanel;
    private VisualElement _pcLearnRecipeCanvasPanel;

    private Button _orderButton;
    private Button _shopButton;
    private Button _LearnButton;

    private Button[] _backButton;

    [Header("Order Panel")]
    // tafinin üzeirne kilit atar
    private VisualElement[] _dontRecipe;

    // Güncel isimler text
    private Label[] _cardNameTexts;

    // Güncel Avatar visual element
    private VisualElement[] _cardAvatars;

    // Güncel tutar text
    private Label[] _cardTotalAmounts;

    // Güncel adet text
    private Label[] _cardCurrentAmounts;

    // Sepete ekleme butonu
    private Button[] _addCardButtons;

    // Ürün sayısı arttırma
    private Button[] _cardUpAmounts;

    // Ürün sayısı azaltma
    private Button[] _cardDownAmounts;

    // Sipariş verilen buton
    private Button _buyButton;

    // Sipariş toplam tutarı text
    private Label _totalAmountText;

    // üürn silme
    private Button _cartDeleteCardButton;

    private Button deleteButton;

    // sepet sınırı 
    private Label _cartBorder;

    [Header("Reciable Learn")]
    // Güncel isimler text
    private Label[] _LearnReciablecardNameTexts;

    // Güncel Avatar visual element
    private VisualElement[] _learnReciableCardAvatars;

    // Ürünü öğren butonu
    private Button[] _learnRecipeCardButtons;

    // Bilgi butonu
    private Button[] _learnInfoCardButtons;

    // öğrenme tutarı
    private Label[] LearnRecipeBuyAmountText;

    // bilgi elemanı
    private VisualElement _infoVisualElement;
    private VisualElement _infoVisualElementImage;
    private Button infoBack;

    [Header("Update Panel")]
    // geliştirme isimleri
    private Label[] _UpdateNameTexts;

    // Güncel Avatar visual element
    private VisualElement[] _UpdateCardAvatars;

    // Ürünü öğren butonu
    private Button[] _UpdateCardButtons;

    // öğrenme tutarı
    private Label[] _UpdateBuyAmountText;

    /// 

    // gün başlangıç için buton
    private Button _shopStateButton;

    // Scroll view
    private ScrollView CartList;
    private VisualElement VisuelCart;
    public VisualTreeAsset Template;

    public override void OnStartServer()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        base.OnStartServer();
        isReadyProductStart();
        isReadyLearnedStart();
    }

    public override void OnStartClient()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        base.OnStartClient();
        isReadyProductStart();
        isReadyLearnedStart();
    }

    // product 
    public void isReadyProductStart()
    {
        ProductAddeValueCard(); // itemlerin yazdırılması
    }

    private void ProductAddeValueCard()
    {
        var root = _document.rootVisualElement;
        // butonları ayarla
        _cardNameTexts = new Label[_ProductItem.ItemData._categoryData.Length];
        _cardAvatars = new VisualElement[_ProductItem.ItemData._categoryData.Length];
        _cardTotalAmounts = new Label[_ProductItem.ItemData._categoryData.Length];
        _cardCurrentAmounts = new Label[_ProductItem.ItemData._categoryData.Length];
        _dontRecipe = new VisualElement[_ProductItem.ItemData._categoryData.Length];

        // Update
        _UpdateNameTexts = new Label[_ProductItem.ItemData.updateData.Length];
        _UpdateCardAvatars = new VisualElement[_ProductItem.ItemData.updateData.Length];
        _UpdateCardButtons = new Button[_ProductItem.ItemData.updateData.Length];
        _UpdateBuyAmountText = new Label[_ProductItem.ItemData.updateData.Length];

        for (int i = 0; i < _ProductItem.ItemData._categoryData.Length; i++)
        {
            _cardNameTexts[i] = root.Q<Label>($"CardNameText{i + 1}");
            _cardAvatars[i] = root.Q<VisualElement>($"Avatar{i + 1}");
            _cardTotalAmounts[i] = root.Q<Label>($"TotalAmountTText{i + 1}");
            _cardCurrentAmounts[i] = root.Q<Label>($"CurrentAmountText{i + 1}");
            _dontRecipe[i] = root.Q<VisualElement>($"DontLearned{i + 1}");
            _dontRecipe[i].style.display = DisplayStyle.None;
        }

        for (int i = 0; i < _ProductItem.ItemData._categoryData.Length; i++)
        {
            if (_cardNameTexts[i] != null)
            {
                _cardNameTexts[i].text = _ProductItem.ItemData._categoryData[i]._name;

                // Avatarın eklenmesi 
                _cardAvatars[i].style.backgroundImage = _ProductItem.ItemData._categoryData[i]._image;

                // Satış tutarı yazdırması
                _cardTotalAmounts[i].text = _ProductItem.ItemData._categoryData[i]._buyValue.ToString();

                // Seçilecek adet yazdırması
                _cardCurrentAmounts[i].text = _ProductItem.ItemData._categoryData[i]._amount.ToString();
                // if (_ProductItem.ItemData._categoryData[i].isDrink)
                // {
                //     _cardNameTexts[i].text = _ProductItem.ItemData._categoryData[0]._PcName;
                //
                //     // Avatarın eklenmesi 
                //     _cardAvatars[i].style.backgroundImage = _ProductItem.ItemData._categoryData[0]._image;
                //
                //     // Satış tutarı yazdırması
                //     _cardTotalAmounts[i].text = _ProductItem.ItemData._categoryData[0]._buyValue.ToString();
                //
                //     // Seçilecek adet yazdırması
                //     _cardCurrentAmounts[i].text = _ProductItem.ItemData._categoryData[0]._amount.ToString();
                // }
                // else
                // {
                //     _cardNameTexts[i].text = _ProductItem.ItemData._categoryData[i]._name;
                //
                //     // Avatarın eklenmesi 
                //     _cardAvatars[i].style.backgroundImage = _ProductItem.ItemData._categoryData[i]._image;
                //
                //     // Satış tutarı yazdırması
                //     _cardTotalAmounts[i].text = _ProductItem.ItemData._categoryData[i]._buyValue.ToString();
                //
                //     // Seçilecek adet yazdırması
                //     _cardCurrentAmounts[i].text = _ProductItem.ItemData._categoryData[i]._amount.ToString();
                // }
                // burada öğrenme işlemini şimdilik fale yapacağız  /////////////* *//////////////
                // _ProductItem.ItemData._categoryData[i]._isLearned = true;
                // // öğrenilmiş ürünleri aç
                // if (!_ProductItem.ItemData._categoryData[i]._isLearned)
                // {
                //     _dontRecipe[i].style.display = DisplayStyle.Flex;
                // }
            }
        }
        //_dontRecipe[_ProductItem.ItemData._categoryData.Length-1].style.display = DisplayStyle.None; // bu tüm ürünleri açıyor

        for (int i = 0; i < _ProductItem.ItemData.updateData.Length; i++)
        {
            _UpdateNameTexts[i] = root.Q<Label>($"UpdateCardNameText{i + 1}");
            _UpdateCardAvatars[i] = root.Q<VisualElement>($"UpdateAvatar{i + 1}");
            _UpdateBuyAmountText[i] = root.Q<Label>($"UpdateBuyAmountText{i + 1}");
        }
        
        for (int i = 0; i < _ProductItem.ItemData.updateData.Length; i++)
        {
            _UpdateNameTexts[i].text =_ProductItem.ItemData.updateData[i]._name;
            _UpdateCardAvatars[i].style.backgroundImage = _ProductItem.ItemData.updateData[i]._image;
            _UpdateBuyAmountText[i].text = _ProductItem.ItemData.updateData[i]._saleValue.ToString();
        }

        // canvas 
        _pcCanvasPanel = root.Q<VisualElement>("PcCanvas");
        _pcShopCanvasPanel = root.Q<VisualElement>("ShopPanel");
        _pcOrderCanvasPanel = root.Q<VisualElement>("OrderPanel");
        _pcLearnRecipeCanvasPanel = root.Q<VisualElement>("LearnRecipePanel");
        // panel giriş ayarları
        _pcCanvasPanel.style.display = DisplayStyle.Flex;
        _pcShopCanvasPanel.style.display = DisplayStyle.None;
        _pcOrderCanvasPanel.style.display = DisplayStyle.None;
        _pcLearnRecipeCanvasPanel.style.display = DisplayStyle.None;

        _LaptopInteract.ShopIsOpenStateChange(_shopStateButton);
    }

    // Learned Obj 
    public void isReadyLearnedStart()
    {
        //LearnedAddeValueCard(); // itemlerin yazdırılması
    }

    // private void LearnedAddeValueCard()
    // {
    //     var root = _document.rootVisualElement;
    //
    //     // product order
    //     // butonları ayarla
    //     _LearnReciablecardNameTexts = new Label[_ProductItem.ItemData._categoryData.Length];
    //     _learnReciableCardAvatars = new VisualElement[_ProductItem.ItemData._categoryData.Length];
    //     LearnRecipeBuyAmountText = new Label[_ProductItem.ItemData._categoryData.Length];
    //
    //     for (int i = 0; i < _ProductItem.ItemData._categoryData.Length; i++)
    //     {
    //         _LearnReciablecardNameTexts[i] = root.Q<Label>($"LearnCardNameText{i + 1}");
    //         _learnReciableCardAvatars[i] = root.Q<VisualElement>($"LearnAvatar{i + 1}");
    //         LearnRecipeBuyAmountText[i] = root.Q<Label>($"LearneBuyAmountText{i + 1}");
    //     }
    //
    //     _infoVisualElement = root.Q<VisualElement>("InfoVisualElement");
    //     _infoVisualElementImage = root.Q<VisualElement>("InfoPath");
    //
    //     _cartBorder = root.Q<Label>("CartText");
    //     _cartBorder.text = _ProductItem.addCartManager.BuyProductItemList.Count +"/"+"3";
    //     
    //     for (int i = 0; i < _ProductItem.ItemData._categoryData.Length; i++)
    //     {
    //         if (_LearnReciablecardNameTexts[i] != null)
    //         {
    //             // isimi 
    //             _LearnReciablecardNameTexts[i].text = _ProductItem.ItemData._categoryData[i]._name;
    //         
    //             // Avatarın eklenmesi 
    //             _learnReciableCardAvatars[i].style.backgroundImage =
    //                 _ProductItem.ItemData._categoryData[i]._image;
    //         
    //             // öğrenme değeri eklenmesi 
    //             LearnRecipeBuyAmountText[i].text = _ProductItem.ItemData._categoryData[i]._LearnedValue.ToString();
    //         }
    //     }
    //
    //     _infoVisualElement.style.display = DisplayStyle.None;
    // }

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        GameObject Laptop = GameObject.Find("Laptop");
        GameObject learnRecipeManager = GameObject.Find("LearnRecipeManager");
        if (Laptop == null && learnRecipeManager == null) return;
        _LearnRecipeManager = learnRecipeManager.GetComponent<LearnRecipeManager>();
        _document = Laptop.GetComponent<UIDocument>();
        _ProductItem = Laptop.GetComponent<ProductItem>();
        _LaptopInteract = Laptop.GetComponent<LaptopInteract>();

        AddedCardButton(); // seçilen ürünü sepete ekler 

        ClickEvent(); // basılan butonu çalıştırma
    }

    private void AddedCardButton()
    {
        // butonları oluştur
        _addCardButtons = new Button[_ProductItem.ItemData._categoryData.Length];
        _cardUpAmounts = new Button[_ProductItem.ItemData._categoryData.Length];
        _cardDownAmounts = new Button[_ProductItem.ItemData._categoryData.Length];
        _backButton = new Button[3];

        var root = _document.rootVisualElement;

        for (int i = 0; i < _ProductItem.ItemData._categoryData.Length; i++)
        {
            _addCardButtons[i] = root.Q<Button>($"AddtoCardButton{i + 1}");
            _ProductItem.ItemData._categoryData[i]._amount =
                1; //////// burada şimdilik sayıların başlamgıcını 1 yapacağız /////////*/////////
            _cardUpAmounts[i] = root.Q<Button>($"UpButton{i + 1}");
            _cardDownAmounts[i] = root.Q<Button>($"DownButton{i + 1}");
        }

        // canvas 
        _pcCanvasPanel = root.Q<VisualElement>("PcCanvas");
        _pcShopCanvasPanel = root.Q<VisualElement>("ShopPanel");
        _pcOrderCanvasPanel = root.Q<VisualElement>("OrderPanel");
        _pcLearnRecipeCanvasPanel = root.Q<VisualElement>("LearnRecipePanel");

        //button
        _buyButton = root.Q<Button>("BuyButton");

        _orderButton = root.Q<Button>("OrderButton");
        _shopButton = root.Q<Button>("ShopButton");
        _LearnButton = root.Q<Button>("LearnButton");

        for (int i = 0; i < 3; i++)
        {
            _backButton[i] = root.Q<Button>($"BackButton{i + 1}");
        }

        // Learn
        _learnRecipeCardButtons = new Button[_ProductItem.ItemData._categoryData.Length];
        _learnInfoCardButtons = new Button[_ProductItem.ItemData._categoryData.Length];

        for (int i = 0; i < _ProductItem.ItemData._categoryData.Length; i++)
        {
            _learnRecipeCardButtons[i] = root.Q<Button>($"LearnRecipeButton{i + 1}");
            _learnInfoCardButtons[i] = root.Q<Button>($"InfoButton{i + 1}");
        }

        infoBack = root.Q<Button>("InfoBack");

        // update 
        _UpdateCardButtons = new Button[_ProductItem.ItemData.updateData.Length];
        for (int i = 0; i < _ProductItem.ItemData.updateData.Length; i++)
        {
            _UpdateCardButtons[i] = root.Q<Button>($"UpdateBuy{i + 1}");
            if (_ProductItem.ItemData.updateData[i]._active)
            {
                _UpdateCardButtons[i].style.display = DisplayStyle.None;
            }
        }

        //DÜKKAN DURUM DEĞİŞİKLİĞİ ATAMASI
        _shopStateButton = root.Q<Button>("ShopOpenStateButton");

        _cartDeleteCardButton = root.Q<Button>("DeleteButton");
    }

    private void ClickEvent()
    {
        for (int i = 0; i < _ProductItem.ItemData._categoryData.Length; i++)
        {
            int index = i;
            if (_addCardButtons[i] != null)
            {
                _cardUpAmounts[i].clicked += () => OnUpButton(index);
                _cardDownAmounts[i].clicked += () => OnDownButton(index);
                _addCardButtons[i].clicked += () => OnClickButton(index);
            }
        }

        _buyButton.clicked += () => BuyOnClickButton();

        _orderButton.clicked += () => OrderPanelOnClickButton();
        _shopButton.clicked += () => ShopOnClickButton();
        _LearnButton.clicked += () => LearnOnClickButton();

        for (int i = 0; i < 3; i++)
        {
            if (_backButton != null)
            {
                _backButton[i].clicked += () => BackOnClickButton();
            }
        }

        for (int i = 0; i < _ProductItem.ItemData.updateData.Length; i++)
        {
            _UpdateCardButtons[i].clicked += () => UpdateBuyOnClickButton(i);
        }

        // learn ////
        // for (int i = 0; i < _ProductItem.ItemData._categoryData.Length; i++)
        // {
        //     int index = i;
        //     if (_learnRecipeCardButtons != null)
        //     {
        //         _learnRecipeCardButtons[i].clicked += () => BuySelectionRecipeOnClickButton(index);
        //         _learnInfoCardButtons[i].clicked += () => InfoOnClickButton(index);
        //     }
        // }

        //infoBack.clicked += () => InfoBackOnClickButton();

        // Shop açık durumu
        _shopStateButton.clicked += () => OpenShopOnClickButton();

        _cartDeleteCardButton.clicked += () => StringOnClickButton();
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
            if (!Physics.Raycast(cameraRay, out hit, 100f, LayerMask.GetMask("Laptop")))
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

    private void OnClickButton(int index)
    {
        if (!isLocalPlayer) return;
        CommandAddOrderButton(index);
    }

    [Command]
    private void CommandAddOrderButton(int index)
    {
        RpcAddOrderButton(index);
    }

    [ClientRpc]
    private void RpcAddOrderButton(int index)
    {
        var root = _document.rootVisualElement;
        CartList = root.Q<ScrollView>("CartListView");
        _totalAmountText = root.Q<Label>("TotalAmountSell");

        _ProductItem.AddCart(index, VisuelCart, Template, CartList, _ProductItem.ItemData._categoryData[index]._amount,
            _totalAmountText);
        _buyButton.SetEnabled(true);
        CartAmountText(); // cart değerini değiştirir
    }

    // seçilen ürünün sayısını azaltır
    private void OnDownButton(int index)
    {
        if (!isLocalPlayer) return;
        CommandAddOrderDownButton(index);
    }

    [Command]
    private void CommandAddOrderDownButton(int index)
    {
        RpcAddOrderDownButton(index);
    }

    [ClientRpc]
    private void RpcAddOrderDownButton(int index)
    {
        var root = _document.rootVisualElement;

        if (_ProductItem.ItemData._categoryData[index]._amount > 1)
        {
            _ProductItem.ItemData._categoryData[index]._amount--;
            _cardCurrentAmounts[index] = root.Q<Label>($"CurrentAmountText{index + 1}");
            _cardCurrentAmounts[index].text = _ProductItem.ItemData._categoryData[index]._amount.ToString();
            int newPurchaseAmount = _ProductItem.ItemData._categoryData[index]._buyValue *
                                    _ProductItem.ItemData._categoryData[index]._amount;
            _cardTotalAmounts[index].text = newPurchaseAmount.ToString();
        }
    }

    // seçilen ürünün sayısını arttırır
    private void OnUpButton(int index)
    {
        if (!isLocalPlayer) return;
        CommandAddOrderUpButton(index);
    }

    [Command]
    private void CommandAddOrderUpButton(int index)
    {
        RpcAddOrderUpButton(index);
    }

    [ClientRpc]
    private void RpcAddOrderUpButton(int index)
    {
        var root = _document.rootVisualElement;

        if (_ProductItem.ItemData._categoryData[index]._amount < 3)
        {
            _ProductItem.ItemData._categoryData[index]._amount++;
            _cardCurrentAmounts[index] = root.Q<Label>($"CurrentAmountText{index + 1}");
            _cardCurrentAmounts[index].text = _ProductItem.ItemData._categoryData[index]._amount.ToString();
            int newPurchaseAmount = _ProductItem.ItemData._categoryData[index]._buyValue *
                                    _ProductItem.ItemData._categoryData[index]._amount;
            _cardTotalAmounts[index].text = newPurchaseAmount.ToString();
        }
    }

    // sipariş onaylama
    private void BuyOnClickButton()
    {
        if (!isLocalPlayer) return;
        CommandBuyButton();
    }

    [Command]
    private void CommandBuyButton()
    {
        RpcBuyButton();
    }

    [ClientRpc]
    private void RpcBuyButton()
    {
        var root = _document.rootVisualElement;
        CartList = root.Q<ScrollView>("CartListView");
        _totalAmountText = root.Q<Label>("TotalAmountSell");

        _ProductItem.BuyButton(CartList, _totalAmountText, _buyButton);

        if (_ProductItem.addCartManager.BuyProductItemList.Count == 0)
        {
            CartAmountText(); // cart değerini değiştirir
        }
    }

    // kart silme
    public void StringOnClickButton()
    {
        if (!isLocalPlayer) return;
        CommandstringButton();
    }

    [Command]
    private void CommandstringButton()
    {
        RpcStringButton();
    }

    [ClientRpc]
    private void RpcStringButton()
    {
        var root = _document.rootVisualElement;
        CartList = root.Q<ScrollView>("CartListView");
        for (int i = 0; i < _ProductItem.addCartManager.BuyCardItemList.Count; i++)
        {
            _ProductItem.addCartManager.BuyProductItemList.Clear();
            foreach (var item in _ProductItem.addCartManager.ItemCardItemList)
            {
                _ProductItem.addCartManager.DeleteCartItems(item.TotalCardAmount);
                _ProductItem._totalAmount = 0;
                _totalAmountText.text = _ProductItem._totalAmount.ToString();
                _ProductItem.addCartManager.ItemCardItemList.Clear();
                _ProductItem.addCartManager.BuyCardItemList.Clear();
                CartList.Clear();
                CartAmountText(); // cart değerini değiştirir
                _buyButton.SetEnabled(false);
                break;
            }
        }
    }

    // öğrenme butonu
    public void BuySelectionRecipeOnClickButton(int index)
    {
        if (!isLocalPlayer) return;
        //CommandBuySelectionRecipeButton(index);
    }

    // [Command]
    // private void CommandBuySelectionRecipeButton(int index)
    // {
    //     RpcBuySelectionRecipeButton(index);
    // }
    //
    // [ClientRpc]
    // private void RpcBuySelectionRecipeButton(int index)
    // {
    //     var root = _document.rootVisualElement;
    //     CartList = root.Q<ScrollView>("CartListView");
    //     _totalAmountText = root.Q<Label>("TotalAmountSell");
    //
    //     _ProductItem.LearnRecipeButton(CartList,_ProductItem.ItemData._categoryData[index]._LearnedValue); 
    //     
    //     _learnRecipeCardButtons[index].style.display = DisplayStyle.None;
    //     
    //     _ProductItem.ItemData._categoryData[index]._isLearned = true; // öğrenildi olarak işaretle
    //     
    //     // öğrenilen kartları açar
    //     if (_ProductItem.ItemData._categoryData[index]._isLearned)
    //     {
    //         _dontRecipe[index].style.display = DisplayStyle.None;
    //         isChangeLearned = false;
    //     }
    // }

    // öğrenme butonu için bilgi almak için
    public void InfoOnClickButton(int index)
    {
        if (!isLocalPlayer) return;
        //CommandInfoButton(index);
    }

    // [Command]
    // private void CommandInfoButton(int index)
    // {
    //     RpcInfoButton(index);
    // }
    //
    // [ClientRpc]
    // private void RpcInfoButton(int index)
    // {
    //     var root = _document.rootVisualElement;
    //     _infoVisualElement = root.Q<VisualElement>("InfoVisualElement");
    //     _infoVisualElementImage = root.Q<VisualElement>("InfoPath");
    //
    //     _infoVisualElement.style.display = DisplayStyle.Flex;
    //     _infoVisualElementImage.style.backgroundImage =_ProductItem.ItemData._categoryData[index]._recipeImage;
    // }

    // Bilgi ekranı çıkış
    public void InfoBackOnClickButton()
    {
        if (!isLocalPlayer) return;
        CommandInfoBackButton();
    }

    [Command]
    private void CommandInfoBackButton()
    {
        RpcInfoBackButton();
    }

    [ClientRpc]
    private void RpcInfoBackButton()
    {
        var root = _document.rootVisualElement;
        _infoVisualElement = root.Q<VisualElement>("InfoVisualElement");
        _infoVisualElementImage = root.Q<VisualElement>("InfoPath");

        _infoVisualElement.style.display = DisplayStyle.None;
    }

    // Dükkan açop kapama
    public void OpenShopOnClickButton()
    {
        if (!isLocalPlayer) return;
        CommandOpenShopButton();
    }

    [Command]
    private void CommandOpenShopButton()
    {
        RpcOpenShopButton();
    }

    [ClientRpc]
    private void RpcOpenShopButton()
    {
        // var root = _document.rootVisualElement;
        _LaptopInteract.ShopIsOpenState(_shopStateButton);
    }


    ///// Panel geçişleri  ////
    // Order Panel geçiş
    public void OrderPanelOnClickButton()
    {
        if (!isLocalPlayer) return;
        CommandOrderPanelButton();
    }

    [Command]
    private void CommandOrderPanelButton()
    {
        // Referansı clientlara RPC ile iletiyoruz
        RpcOrderPanelButton();
    }

    [ClientRpc]
    private void RpcOrderPanelButton()
    {
        var root = _document.rootVisualElement;

        // canvas 
        _pcCanvasPanel = root.Q<VisualElement>("PcCanvas");
        _pcShopCanvasPanel = root.Q<VisualElement>("ShopPanel");
        _pcOrderCanvasPanel = root.Q<VisualElement>("OrderPanel");
        _pcLearnRecipeCanvasPanel = root.Q<VisualElement>("LearnRecipePanel");
        //
        _pcCanvasPanel.style.display = DisplayStyle.None;
        _pcOrderCanvasPanel.style.display = DisplayStyle.Flex;
    }

    // Order Panel geçiş
    public void ShopOnClickButton()
    {
        if (!isLocalPlayer) return;
        CommandShopPanelButton();
    }

    [Command]
    private void CommandShopPanelButton()
    {
        RpcShopPanelButton();
    }

    [ClientRpc]
    private void RpcShopPanelButton()
    {
        var root = _document.rootVisualElement;

        // canvas 
        _pcCanvasPanel = root.Q<VisualElement>("PcCanvas");
        _pcShopCanvasPanel = root.Q<VisualElement>("ShopPanel");
        _pcOrderCanvasPanel = root.Q<VisualElement>("OrderPanel");
        _pcLearnRecipeCanvasPanel = root.Q<VisualElement>("LearnRecipePanel");

        _pcCanvasPanel.style.display = DisplayStyle.None;
        _pcShopCanvasPanel.style.display = DisplayStyle.Flex;
    }

    // Learn Panel geçiş
    public void LearnOnClickButton()
    {
        if (!isLocalPlayer) return;
        CommandLearnPanelButton();
    }

    [Command]
    private void CommandLearnPanelButton()
    {
        RpcLearnPanelButton();
    }

    [ClientRpc]
    private void RpcLearnPanelButton()
    {
        var root = _document.rootVisualElement;

        // canvas 
        _pcCanvasPanel = root.Q<VisualElement>("PcCanvas");
        _pcShopCanvasPanel = root.Q<VisualElement>("ShopPanel");
        _pcOrderCanvasPanel = root.Q<VisualElement>("OrderPanel");
        _pcLearnRecipeCanvasPanel = root.Q<VisualElement>("LearnRecipePanel");
        //
        _pcCanvasPanel.style.display = DisplayStyle.None;
        _pcLearnRecipeCanvasPanel.style.display = DisplayStyle.Flex;
    }


    // Geri gelme Butonu
    public void BackOnClickButton()
    {
        if (!isLocalPlayer) return;
        CommandBackPanelButton();
    }

    [Command]
    private void CommandBackPanelButton()
    {
        RpcBackPanelButton();
    }

    [ClientRpc]
    private void RpcBackPanelButton()
    {
        _pcCanvasPanel.style.display = DisplayStyle.Flex;
        _pcShopCanvasPanel.style.display = DisplayStyle.None;
        _pcOrderCanvasPanel.style.display = DisplayStyle.None;
        _pcLearnRecipeCanvasPanel.style.display = DisplayStyle.None;
    }

    // Update Satın alma butonu
    public void UpdateBuyOnClickButton(int index)
    {
        if (!isLocalPlayer) return;
        CommandUpdateBuyButton(index);
    }

    [Command]
    private void CommandUpdateBuyButton(int index)
    {
        RpcUpdateBuyButton(index);
    }

    [ClientRpc]
    private void RpcUpdateBuyButton(int index)
    {
        var root = _document.rootVisualElement;
        CartList = root.Q<ScrollView>("CartListView");
        _totalAmountText = root.Q<Label>("TotalAmountSell");
        
        //_ProductItem.BuyButton(CartList, _totalAmountText, _buyButton);
        GameManager.instance.MoneyAddAndRemove(false, _ProductItem.ItemData.updateData[index - 1]._saleValue, false,
            CartList);
        _UpdateCardButtons[index-1] = _document.rootVisualElement.Q<Button>($"UpdateBuy{index}");
        _ProductItem.ItemData.updateData[index-1]._active = true;
        _UpdateCardButtons[index-1].style.display = DisplayStyle.None;
        if (_ProductItem.ItemData.updateData[0]._active)
        {
           // GameManager.instance.SpawnStartCar();
        }
    }

    private void CartAmountText()
    {
        var root = _document.rootVisualElement;

        _cartBorder = root.Q<Label>("CartText");
        _cartBorder.text = _ProductItem.addCartManager.BuyProductItemList.Count + "/" + "6";
        switch (_ProductItem.addCartManager.BuyProductItemList.Count)
        {
            case 0:
                _cartBorder.style.color = Color.white;
                break;
            case 1:
                _cartBorder.style.color = Color.green;
                break;
            case 2:
                _cartBorder.style.color = Color.green;
                break;
            case 3:
                _cartBorder.style.color = Color.green;
                break;
            case 4:
                _cartBorder.style.color = Color.green;
                break;
            case 5:
                _cartBorder.style.color = Color.green;
                break;
            case 6:
                _cartBorder.style.color = Color.green;
                break;
            case < 10:
                _cartBorder.style.color = Color.red;
                break;
        }
    }
}
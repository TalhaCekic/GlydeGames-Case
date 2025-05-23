using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class BurgerScreen : NetworkBehaviour
{
    public UIDocument Doc;
    public VisualElement Cart;
    public VisualTreeAsset _template;
    public ScrollView CartList;

    void Start()
    {
        if (!isServer) return;
    }

    void OnEnable()
    {
        Doc.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) =>
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100, Color.magenta);

            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit, 100f, LayerMask.GetMask("Screen")))
            {
                return invalidPosition;
            }

            Vector2 pixelUV = hit.textureCoord;

            pixelUV.y = 1 - pixelUV.y;
            pixelUV.x *= Doc.panelSettings.targetTexture.width;
            pixelUV.y *= Doc.panelSettings.targetTexture.height;

            // var cursor = _document.rootVisualElement.Q<VisualElement>("Cursor");
            // if (cursor != null)
            // {
            //     cursor.style.left = pixelUV.x;
            //     cursor.style.top = pixelUV.y;
            // }
            return pixelUV;
        });
    }

    public void AddCart(string name,bool isOnline,int no)
    {
        CartList = Doc.rootVisualElement.Q<ScrollView>("BurgerView");
        Cart = _template.CloneTree();

        Cart.Q<VisualElement>("Card"); // Sepetteki kartı bul
        Label CardName = Cart.Q<Label>("Name"); // isim  text bul
        VisualElement BreadImage = Cart.Q<VisualElement>("BreadIcon"); // avatar bul
        VisualElement MeatImage = Cart.Q<VisualElement>("MeatIcon"); // avatar bul
        VisualElement TomatoImage = Cart.Q<VisualElement>("TomatoIcon"); // avatar bul
        VisualElement LettuceImage = Cart.Q<VisualElement>("LettuceIcon"); // avatar bul
        VisualElement CheeseImage = Cart.Q<VisualElement>("CheeseIcon"); // avatar bul
        
        VisualElement OnlinePanel = Cart.Q<VisualElement>("OnlinePanel"); // avatar bul
        Label OnlineOrderNo = Cart.Q<Label>("OrderNo_Label"); // avatar bul

        Cart.name = name;
        CardName.text = name;

        CartList.Add(Cart);

        CartList.schedule.Execute(() => {
            CartList.ScrollTo(Cart);
        }).ExecuteLater(10);
        
        BreadImage.style.display = DisplayStyle.None;
        MeatImage.style.display = DisplayStyle.None;
        TomatoImage.style.display = DisplayStyle.None;
        LettuceImage.style.display = DisplayStyle.None;
        CheeseImage.style.display = DisplayStyle.None;
        OnlinePanel.style.display = DisplayStyle.None;

        if (name == "Hamburger - 1")
        {
            BreadImage.style.display = DisplayStyle.Flex;
            MeatImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Hamburger - 2")
        {
            BreadImage.style.display = DisplayStyle.Flex;
            MeatImage.style.display = DisplayStyle.Flex;
            TomatoImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Hamburger - 3")
        {
            BreadImage.style.display = DisplayStyle.Flex;
            MeatImage.style.display = DisplayStyle.Flex;
            LettuceImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Hamburger - 4")
        {
            BreadImage.style.display = DisplayStyle.Flex;
            MeatImage.style.display = DisplayStyle.Flex;
            CheeseImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Hamburger - 5")
        {
            BreadImage.style.display = DisplayStyle.Flex;
            MeatImage.style.display = DisplayStyle.Flex;
            TomatoImage.style.display = DisplayStyle.Flex;
            LettuceImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Hamburger - 6")
        {
            BreadImage.style.display = DisplayStyle.Flex;
            MeatImage.style.display = DisplayStyle.Flex;
            TomatoImage.style.display = DisplayStyle.Flex;
            CheeseImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Hamburger - 7")
        {
            BreadImage.style.display = DisplayStyle.Flex;
            MeatImage.style.display = DisplayStyle.Flex;
            LettuceImage.style.display = DisplayStyle.Flex;
            CheeseImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Hamburger - 8")
        {
            BreadImage.style.display = DisplayStyle.Flex;
            MeatImage.style.display = DisplayStyle.Flex;
            TomatoImage.style.display = DisplayStyle.Flex;
            LettuceImage.style.display = DisplayStyle.Flex;
            CheeseImage.style.display = DisplayStyle.Flex;
        }

        if (isOnline)
        {
            OnlinePanel.style.display = DisplayStyle.Flex;
            OnlineOrderNo.text = "No : " + no.ToString();
        }
    }
   public void ListRemoveOnlineOff(string name)
    {
        foreach (var var in CartList.Children())
        {
            if (var.Q<VisualElement>("OnlinePanel").style.display == DisplayStyle.None)
            {
                if (var.name == name)
                {                
                    print(name);
                    Destroy(var.visualTreeAssetSource); // İlk olarak GameObject'i yok et
                    CartList.Remove(var); // Sonra listedeki referansını kaldır
                    break;
                }
            }
        }
    }
    public void ListRemoveOnlineOn(string name)
    {
        foreach (var var in CartList.Children())
        {
            if (var.Q<VisualElement>("OnlinePanel").style.display == DisplayStyle.Flex)
            {
                if (var.name == name)
                {                
                    print(name);
                    Destroy(var.visualTreeAssetSource); // İlk olarak GameObject'i yok et
                    CartList.Remove(var); // Sonra listedeki referansını kaldır
                    break;
                }
            }
        }
    }
    // public void ListRemove(string name)
    // {
    //     for (int i = 0; i < CartList.childCount; i++)
    //     {
    //         if (CartList[i].name == name)
    //         {
    //             Destroy(CartList[i].visualTreeAssetSource); // İlk olarak GameObject'i yok et
    //             CartList.RemoveAt(i); // Sonra listedeki referansını kaldır
    //             break;
    //         }
    //     }
    // }

    public void ListClear()
    {
        CartList = Doc.rootVisualElement.Q<ScrollView>("BurgerView");
        for (int i = 0; i < CartList.childCount; i++)
        {
            Destroy(CartList[i].visualTreeAssetSource);
            CartList.Clear(); 
        }
    }
}
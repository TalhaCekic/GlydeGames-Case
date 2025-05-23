using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class SnackScreen : NetworkBehaviour
{
    public UIDocument Doc;
    public VisualElement Cart;
    public VisualTreeAsset _template;
    public ScrollView CartList;

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
        CartList = Doc.rootVisualElement.Q<ScrollView>("SnackView");
        
        Cart = _template.CloneTree();

        Cart.Q<VisualElement>("Card"); // Sepetteki kartı bul
        Label CardName = Cart.Q<Label>("Name"); // isim  text bul
        VisualElement ColaImage = Cart.Q<VisualElement>("ColaImage"); // avatar bul
        VisualElement PantaImage = Cart.Q<VisualElement>("PantaImage"); // avatar bul
        VisualElement SodaImage = Cart.Q<VisualElement>("SodaImage"); // avatar bul
        VisualElement PotatoImage = Cart.Q<VisualElement>("PotatoImage"); // avatar bul

        VisualElement OnlinePanel = Cart.Q<VisualElement>("OnlinePanel"); // avatar bul
        Label OnlineOrderNo = Cart.Q<Label>("OrderNo_Label"); // avatar bul
        
        Cart.name = name;
        CardName.text = name;

        
        if (Cart != null)
        {
            if (CartList != null)
            {
                CartList.Add(Cart);
            }
        }
        CartList.schedule.Execute(() => {
            CartList.ScrollTo(Cart);
        }).ExecuteLater(10);
        
        ColaImage.style.display = DisplayStyle.None;
        PantaImage.style.display = DisplayStyle.None;
        SodaImage.style.display = DisplayStyle.None;
        PotatoImage.style.display = DisplayStyle.None;
        OnlinePanel.style.display = DisplayStyle.None;

        if (name == "Cola")
        {
            ColaImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Panta")
        {
            PantaImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Soda")
        {
            SodaImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Patato")
        {
            PotatoImage.style.display = DisplayStyle.Flex;
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
        // for (int i = 0; i < CartList.childCount; i++)
        // {
        //     if (CartList[i].Q<VisualElement>("OnlinePanel").style.display == DisplayStyle.None)
        //     {
        //         print("silmeye gir");
        //         if (CartList[i].name == name)
        //         {                
        //             print(name);
        //             Destroy(CartList[i].visualTreeAssetSource); // İlk olarak GameObject'i yok et
        //             CartList.RemoveAt(i); // Sonra listedeki referansını kaldır
        //             break;
        //         }
        //     }
        //     
        //     // if (CartList[i].name == name)
        //     // { 
        //     //     Destroy(CartList[i].visualTreeAssetSource); // İlk olarak GameObject'i yok et
        //     //     CartList.RemoveAt(i); // Sonra listedeki referansını kaldır
        //     //     break;
        //     // }
        // }
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
    public void ListClear()
    {
        CartList = Doc.rootVisualElement.Q<ScrollView>("SnackView");
        
        for (int i = 0; i < CartList.childCount; i++)
        {
            Destroy(CartList[i].visualTreeAssetSource);
            CartList.Clear(); 
        }
        
    }
}

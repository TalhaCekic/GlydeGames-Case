using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ItemInfoCanvas : NetworkBehaviour
{
    public Datas data;
    [Header("UI")] public UIDocument _document;

    private ScrollView Bg2;
    private Label _ıtemName;
    private VisualElement BreadVisuel;
    private VisualElement RawMeatVisuel;
    private VisualElement BakedMeatVisuel;
    private VisualElement TomatoVisual;
    private VisualElement LettuceVisual;
    private VisualElement CheeseVisual;
    private VisualElement CupVisual;
    
    [Header("Drinks")] 
    private Label DrinkName;
    private VisualElement ColaVisual;
    private VisualElement PantaVisual;
    private VisualElement SodaVisual;
    
    [Header("Menus")] 
    private Label BurgerName;
    private VisualElement Hamburger1;
    private VisualElement Hamburger2;
    private VisualElement Hamburger3;
    private VisualElement Hamburger4;
    private VisualElement Hamburger5;
    private VisualElement Hamburger6;
    private VisualElement Hamburger7;
    private VisualElement Hamburger8;
    
    [Header("Snack")] 
    private Label SnackName;
    private VisualElement PatatoVisual;
    private VisualElement PatatoPacketVisual;

    [Header("Ray")] [SerializeField] [Min(1)]
    private float hitRange = 1;

    int pickupLayerMask;
    int pickupLayerMask1;

    private GameObject lastObj;

    [SyncVar] private bool isBurger; 
    [SyncVar] private bool isDrink; 
   [SyncVar] private bool isSnack; 
   
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        CmdItemAdd();
        pickupLayerMask = LayerMask.GetMask("Item");
        pickupLayerMask1 = LayerMask.GetMask("Tray");
    }

    [Command]
    private void CmdItemAdd()
    {
        RpcItemAdd();
    }

    [TargetRpc]
    private void RpcItemAdd()
    {
        GameObject CanvasObj = GameObject.FindGameObjectWithTag("ItemInfoCanvas");
        _document = CanvasObj.GetComponent<UIDocument>();
        //Bg = _document.rootVisualElement.Q<VisualElement>("Bg");
        Bg2 = _document.rootVisualElement.Q<ScrollView>("Bg");

        BreadVisuel = _document.rootVisualElement.Q<VisualElement>("Bread");
        RawMeatVisuel = _document.rootVisualElement.Q<VisualElement>("RawMeat");
        BakedMeatVisuel = _document.rootVisualElement.Q<VisualElement>("BakedMeat");
        TomatoVisual = _document.rootVisualElement.Q<VisualElement>("Tomato");
        LettuceVisual = _document.rootVisualElement.Q<VisualElement>("Lettuce");
        CheeseVisual = _document.rootVisualElement.Q<VisualElement>("Cheese");        
        CupVisual = _document.rootVisualElement.Q<VisualElement>("Cup");
        ColaVisual = _document.rootVisualElement.Q<VisualElement>("Cola");
        PantaVisual = _document.rootVisualElement.Q<VisualElement>("Panta");
        SodaVisual = _document.rootVisualElement.Q<VisualElement>("Soda");
        PatatoVisual = _document.rootVisualElement.Q<VisualElement>("Patato");
        PatatoPacketVisual = _document.rootVisualElement.Q<VisualElement>("PatatoPacket");
        // Menus
        Hamburger1 = _document.rootVisualElement.Q<VisualElement>("Hamburger1");
        Hamburger2 = _document.rootVisualElement.Q<VisualElement>("Hamburger2");
        Hamburger3 = _document.rootVisualElement.Q<VisualElement>("Hamburger3");
        Hamburger4 = _document.rootVisualElement.Q<VisualElement>("Hamburger4");        
        Hamburger5 = _document.rootVisualElement.Q<VisualElement>("Hamburger5");
        Hamburger6 = _document.rootVisualElement.Q<VisualElement>("Hamburger6");
        Hamburger7 = _document.rootVisualElement.Q<VisualElement>("Hamburger7");
        Hamburger8 = _document.rootVisualElement.Q<VisualElement>("Hamburger8");
        
        DrinkName = _document.rootVisualElement.Q<Label>("DrinkName_Label");
        BurgerName = _document.rootVisualElement.Q<Label>("FoodName_Label");
        SnackName = _document.rootVisualElement.Q<Label>("SnackName_Label");
        
        BurgerName.style.display =  DisplayStyle.None;
        DrinkName.style.display =  DisplayStyle.None;
        SnackName.style.display =  DisplayStyle.None;

        _ıtemName = _document.rootVisualElement.Q<Label>("ItemName");

        Bg2.style.display = DisplayStyle.None;
        BreadVisuel.style.display = DisplayStyle.None;
        RawMeatVisuel.style.display = DisplayStyle.None;
        BakedMeatVisuel.style.display = DisplayStyle.None;
        TomatoVisual.style.display = DisplayStyle.None;
        LettuceVisual.style.display = DisplayStyle.None;
        CheeseVisual.style.display = DisplayStyle.None;        
        CupVisual.style.display = DisplayStyle.None;
        ColaVisual.style.display = DisplayStyle.None;
        PantaVisual.style.display = DisplayStyle.None;
        SodaVisual.style.display = DisplayStyle.None;
        PatatoVisual.style.display = DisplayStyle.None;
        
        Hamburger1.style.display = DisplayStyle.None;
        Hamburger2.style.display = DisplayStyle.None;
        Hamburger3.style.display = DisplayStyle.None;        
        Hamburger4.style.display = DisplayStyle.None;
        Hamburger5.style.display = DisplayStyle.None;
        Hamburger6.style.display = DisplayStyle.None;
        Hamburger7.style.display = DisplayStyle.None;
        Hamburger8.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        if (!isLocalPlayer) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position,
                     Camera.main.transform.TransformDirection(Vector3.forward), out hit, hitRange, pickupLayerMask1))
        {
            CmdItemInfoTray(hit.collider.gameObject);
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward),
                out hit, hitRange, pickupLayerMask))
        {
            CmdItemInfo(hit.collider.gameObject);
        }
        else
        {
            CmdItemInfoDelete();
        }
    }

    [Command]
    private void CmdItemInfo(GameObject obj)
    {
        RpcItemInfo(obj);
    }

    [TargetRpc]
    private void RpcItemInfo(GameObject obj)
    {
        if (_document == null) return;
        Bg2.RemoveFromClassList("Scale-Opacity");
        Bg2.AddToClassList("Scale-OpacityReset");
        if (obj.GetComponent<ItemInteract>().itemName == "Hamburger Bread")
        {     
            Bg2.style.display = DisplayStyle.Flex;

            BreadVisuel.style.display = DisplayStyle.Flex;

            lastObj = obj;
            if (lastObj.GetComponent<CombineProduct>().Meat || lastObj.GetComponent<CombineProduct>().Tomato || lastObj.GetComponent<CombineProduct>().Lettuce || lastObj.GetComponent<CombineProduct>().Cheese)
            {
                for (int i = 0; i < lastObj.GetComponent<CombineProduct>().ItemDatas._foodData.Length; i++)
                {
                    switch (lastObj.GetComponent<CombineProduct>().ComplateItemName)
                    {
                        case "Hamburger - 1":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 2":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            TomatoVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 3":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            LettuceVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 4":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            CheeseVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 5":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            TomatoVisual.style.display = DisplayStyle.Flex;
                            LettuceVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 6":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            TomatoVisual.style.display = DisplayStyle.Flex;
                            CheeseVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 7":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            LettuceVisual.style.display = DisplayStyle.Flex;
                            CheeseVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 8":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            TomatoVisual.style.display = DisplayStyle.Flex;
                            LettuceVisual.style.display = DisplayStyle.Flex;
                            CheeseVisual.style.display = DisplayStyle.Flex;
                            break;
                    }
                }

                _ıtemName.text = lastObj.GetComponent<ItemName>()._ItemName;
            }
            else
            {
                _ıtemName.text = obj.GetComponent<ItemName>()._ItemName;
            }
        }
        else
        {
            if (obj.GetComponent<ItemName>()._ItemName != null)
            {
                Bg2.style.display = DisplayStyle.Flex;
                _ıtemName.text = obj.GetComponent<ItemName>()._ItemName;
                switch (obj.GetComponent<ItemName>()._ItemName)
                {
                    case "Raw Meat":
                        RawMeatVisuel.style.display = DisplayStyle.Flex;
                        break;
                    case "Baked Meat":
                        BakedMeatVisuel.style.display = DisplayStyle.Flex;
                        break;
                    case "Tomato":
                        TomatoVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Lettuce":
                        LettuceVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Cheese":
                        CheeseVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Cup":
                        CupVisual.style.display = DisplayStyle.Flex;
                        break;       
                    case "Cola":
                        ColaVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Panta":
                        PantaVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Soda":
                        SodaVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Patato":
                        PatatoVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Patato Packet":
                        PatatoPacketVisual.style.display = DisplayStyle.Flex;
                        break;
                }
            }
        }
    }

    [Command]
    private void CmdItemInfoTray(GameObject obj)
    {
        RpcItemInfoTray(obj);
    }

    [TargetRpc]
    private void RpcItemInfoTray(GameObject obj)
    {
        if (_document == null) return;

        Bg2.style.display = DisplayStyle.Flex;
        Bg2.AddToClassList("Scale-OpacityReset");
        Bg2.RemoveFromClassList("Scale-Opacity");
        _ıtemName = _document.rootVisualElement.Q<Label>("ItemName");
        
        Hamburger1.style.display = DisplayStyle.None;
        Hamburger2.style.display = DisplayStyle.None;
        Hamburger3.style.display = DisplayStyle.None;        
        Hamburger4.style.display = DisplayStyle.None;
        Hamburger5.style.display = DisplayStyle.None;
        Hamburger6.style.display = DisplayStyle.None;
        Hamburger7.style.display = DisplayStyle.None;
        Hamburger8.style.display = DisplayStyle.None;
        
        lastObj = obj;
        
         if(obj.GetComponent<Tray>()._name == null)return;
         _ıtemName.text = obj.GetComponent<Tray>()._name;

         if (obj.GetComponent<Tray>().Burger != null)
         {
             isBurger = true;
             BurgerName.style.display = DisplayStyle.Flex;
             BurgerName.text = obj.GetComponent<Tray>().Burger.GetComponent<ItemName>()._ItemName;
             if (obj.GetComponent<Tray>().Burger.GetComponent<ItemName>()._ItemName == "Hamburger - 1")
             {
                 Hamburger1.style.display = DisplayStyle.Flex;
             }
             if (obj.GetComponent<Tray>().Burger.GetComponent<ItemName>()._ItemName == "Hamburger - 2")
             {
                 Hamburger2.style.display = DisplayStyle.Flex;
             }
             if (obj.GetComponent<Tray>().Burger.GetComponent<ItemName>()._ItemName == "Hamburger - 3")
             {
                 Hamburger3.style.display = DisplayStyle.Flex;
             }
             if (obj.GetComponent<Tray>().Burger.GetComponent<ItemName>()._ItemName == "Hamburger - 4")
             {
                 Hamburger4.style.display = DisplayStyle.Flex;
             }
             if (obj.GetComponent<Tray>().Burger.GetComponent<ItemName>()._ItemName == "Hamburger - 5")
             {
                 Hamburger5.style.display = DisplayStyle.Flex;
             }
             if (obj.GetComponent<Tray>().Burger.GetComponent<ItemName>()._ItemName == "Hamburger - 6")
             {
                 Hamburger6.style.display = DisplayStyle.Flex;
             }
             if (obj.GetComponent<Tray>().Burger.GetComponent<ItemName>()._ItemName == "Hamburger - 7")
             {
                 Hamburger7.style.display = DisplayStyle.Flex;
             }
             if (obj.GetComponent<Tray>().Burger.GetComponent<ItemName>()._ItemName == "Hamburger - 8")
             {
                 Hamburger8.style.display = DisplayStyle.Flex;
             }
         }

         if (obj.GetComponent<Tray>().Drink != null)
         {
             DrinkName.style.display = DisplayStyle.Flex;
             DrinkName.text = obj.GetComponent<Tray>().Drink.GetComponent<ItemName>()._ItemName;
             if (obj.GetComponent<Tray>().Drink.GetComponent<ItemName>()._ItemName == "Cola")
             {
                 ColaVisual.style.display = DisplayStyle.Flex;
             }
             if (obj.GetComponent<Tray>().Drink.GetComponent<ItemName>()._ItemName == "Panta")
             {
                 PantaVisual.style.display = DisplayStyle.Flex;
             }
             if (obj.GetComponent<Tray>().Drink.GetComponent<ItemName>()._ItemName == "Soda")
             {
                 SodaVisual.style.display = DisplayStyle.Flex;
             }
             isDrink = true;
         }
         if (obj.GetComponent<Tray>().Snack != null)
         {
             SnackName.style.display = DisplayStyle.Flex;
             isSnack= true;
             SnackName.text = obj.GetComponent<Tray>().Snack.GetComponent<ItemName>()._ItemName;
             if (obj.GetComponent<Tray>().Snack.GetComponent<ItemName>()._ItemName == "Patato")
             {
                 PatatoVisual.style.display = DisplayStyle.Flex;
             }
         }
         
         if (isBurger && isDrink && isSnack)
         {
             Bg2.AddToClassList("viewSize_4");
             Bg2.RemoveFromClassList("viewSize");
             Bg2.RemoveFromClassList("viewSize_1");
             Bg2.RemoveFromClassList("viewSize_2");
             Bg2.RemoveFromClassList("viewSize_3");
         }
         else  if (isBurger || isDrink && isBurger || isSnack && isSnack || isDrink)
         {
             Bg2.AddToClassList("viewSize_3");
             Bg2.RemoveFromClassList("viewSize");
             Bg2.RemoveFromClassList("viewSize_1");
             Bg2.RemoveFromClassList("viewSize_2");
             Bg2.RemoveFromClassList("viewSize_4");
         }
         else if(isBurger || isDrink || isSnack)
         {
             Bg2.AddToClassList("viewSize_1");
             Bg2.RemoveFromClassList("viewSize");
             Bg2.RemoveFromClassList("viewSize_2");
             Bg2.RemoveFromClassList("viewSize_3");
             Bg2.RemoveFromClassList("viewSize_4");
         }
         else
         {
             Bg2.AddToClassList("viewSize");
             Bg2.RemoveFromClassList("viewSize_1");
             Bg2.RemoveFromClassList("viewSize_2");
             Bg2.RemoveFromClassList("viewSize_3");
             Bg2.RemoveFromClassList("viewSize_4");
             BurgerName.style.display =  DisplayStyle.None;
             DrinkName.style.display =  DisplayStyle.None;
             SnackName.style.display =  DisplayStyle.None;
         }
    }

    [Command]
    private void CmdItemInfoDelete()
    {
        RpcItemInfoDelete();
    }

    [TargetRpc]
    private void RpcItemInfoDelete()
    {
        if (_document == null) return;
        isBurger = false;
        isDrink = false;
        isSnack = false;

        BurgerName.style.display =  DisplayStyle.None;
        DrinkName.style.display =  DisplayStyle.None;
        SnackName.style.display =  DisplayStyle.None;
        
        Bg2.AddToClassList("viewSize");
        Bg2.RemoveFromClassList("viewSize_1");
        Bg2.RemoveFromClassList("viewSize_2");
        Bg2.RemoveFromClassList("viewSize_3");
        Bg2.RemoveFromClassList("viewSize_4");
        
        Bg2.RemoveFromClassList("Scale-OpacityReset");
        Bg2.AddToClassList("Scale-Opacity");
        Bg2.style.display = DisplayStyle.None;
        _ıtemName.text = "";
        BreadVisuel.style.display = DisplayStyle.None;
        RawMeatVisuel.style.display = DisplayStyle.None;
        BakedMeatVisuel.style.display = DisplayStyle.None;
        TomatoVisual.style.display = DisplayStyle.None;
        LettuceVisual.style.display = DisplayStyle.None;
        CheeseVisual.style.display = DisplayStyle.None;   
        CupVisual.style.display = DisplayStyle.None;
        ColaVisual.style.display = DisplayStyle.None;
        PantaVisual.style.display = DisplayStyle.None;
        SodaVisual.style.display = DisplayStyle.None;
        PatatoVisual.style.display = DisplayStyle.None;
        PatatoPacketVisual.style.display = DisplayStyle.None;
    }

    private void FinderMenu()
    {
        
    }
}
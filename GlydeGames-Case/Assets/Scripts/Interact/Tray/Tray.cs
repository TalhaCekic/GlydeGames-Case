using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Tray : NetworkBehaviour
{
    [SyncVar] public string _name;
    public Datas MenuData;

    public string inParentBurgerName;
    public string inParentDrinkName;
    public string inParentSnackName;

    [Header("Pos")]
    public Transform BurgerPos;
    public Transform DrinkPos;
    public Transform SnackPos;
    [Header("Obj")]
    public GameObject Burger;
    public GameObject Drink;
    public GameObject Snack;

    private void Update()
    {
        if (isServer)
        {
            ServerUpdate();
        }
    }

    [Server]
    private void ServerUpdate()
    {
        RpcUpdate();
    }
    [ClientRpc]
    private void RpcUpdate()
    {
        this.enabled = true;
    }
    [Server]
    public void ServerTrayAddItemName(string _Name, int index,GameObject obj)
    {
        RpcTrayAddItemNames(_Name,index,obj);
    }
    
    [ClientRpc]
    public void RpcTrayAddItemNames(string _Name, int index,GameObject obj)
    {
        switch (index)
        {
            case 1:
                inParentBurgerName = _Name;
                obj.GetComponent<ItemInteract>().ServerStateChange(true);
                obj.GetComponent<ItemInteract>().ItemParentObj(BurgerPos);
                obj.GetComponent<ItemInteract>().isStateFree = false;
                obj.GetComponent<ItemInteract>().RpcStateChange();
                Burger = obj;
                break;
            case 2:
                inParentDrinkName = _Name;
                obj.GetComponent<ItemInteract>().ServerStateChange(true);
                obj.GetComponent<ItemInteract>().ItemParentObj(DrinkPos);
                obj.GetComponent<ItemInteract>().isStateFree = false;
                obj.GetComponent<ItemInteract>().RpcStateChange();
                Drink = obj;
                break;
            case 3:
                inParentSnackName = _Name;
                obj.GetComponent<ItemInteract>().ServerStateChange(true);
                obj.GetComponent<ItemInteract>().ItemParentObj(SnackPos);
                obj.GetComponent<ItemInteract>().isStateFree = false;
                obj.GetComponent<ItemInteract>().RpcStateChange();
                Snack = obj;
                break;
        }

        StateName();
    }

    private void StateName()
    {
        if (inParentSnackName == "Patato")
        {
            if (inParentDrinkName == "Cola")
            {
                if (inParentBurgerName == "Hamburger - 1")
                {
                    _name = MenuData._Menu[0];
                }

                if (inParentBurgerName == "Hamburger - 2")
                {
                    _name = MenuData._Menu[1];
                }

                if (inParentBurgerName == "Hamburger - 3")
                {
                    _name = MenuData._Menu[2];
                }

                if (inParentBurgerName == "Hamburger - 4")
                {
                    _name = MenuData._Menu[3];
                }

                if (inParentBurgerName == "Hamburger - 5")
                {
                    _name = MenuData._Menu[4];
                }

                if (inParentBurgerName == "Hamburger - 6")
                {
                    _name = MenuData._Menu[5];
                }

                if (inParentBurgerName == "Hamburger - 7")
                {
                    _name = MenuData._Menu[6];
                }

                if (inParentBurgerName == "Hamburger - 8")
                {
                    _name = MenuData._Menu[7];
                }
            }

            if (inParentDrinkName == "Panta")
            {
                if (inParentBurgerName == "Hamburger - 1")
                {
                    _name = MenuData._Menu[8];
                }

                if (inParentBurgerName == "Hamburger - 2")
                {
                    _name = MenuData._Menu[9];
                }

                if (inParentBurgerName == "Hamburger - 3")
                {
                    _name = MenuData._Menu[10];
                }

                if (inParentBurgerName == "Hamburger - 4")
                {
                    _name = MenuData._Menu[11];
                }

                if (inParentBurgerName == "Hamburger - 5")
                {
                    _name = MenuData._Menu[12];
                }

                if (inParentBurgerName == "Hamburger - 6")
                {
                    _name = MenuData._Menu[13];
                }

                if (inParentBurgerName == "Hamburger - 7")
                {
                    _name = MenuData._Menu[14];
                }

                if (inParentBurgerName == "Hamburger - 8")
                {
                    _name = MenuData._Menu[15];
                }
            }

            if (inParentDrinkName == "Soda")
            {
                if (inParentBurgerName == "Hamburger - 1")
                {
                    _name = MenuData._Menu[16];
                }

                if (inParentBurgerName == "Hamburger - 2")
                {
                    _name = MenuData._Menu[17];
                }

                if (inParentBurgerName == "Hamburger - 3")
                {
                    _name = MenuData._Menu[18];
                }

                if (inParentBurgerName == "Hamburger - 4")
                {
                    _name = MenuData._Menu[19];
                }

                if (inParentBurgerName == "Hamburger - 5")
                {
                    _name = MenuData._Menu[20];
                }

                if (inParentBurgerName == "Hamburger - 6")
                {
                    _name = MenuData._Menu[21];
                }

                if (inParentBurgerName == "Hamburger - 7")
                {
                    _name = MenuData._Menu[22];
                }

                if (inParentBurgerName == "Hamburger - 8")
                {
                    _name = MenuData._Menu[23];
                }
            }
        }
    }

    public void Trash()
    {
        Destroy(Burger);
        Destroy(Drink);
        Destroy(Snack);

        _name = null;
        inParentBurgerName = null;
        inParentDrinkName = null;
        inParentSnackName = null;
    }
    
}
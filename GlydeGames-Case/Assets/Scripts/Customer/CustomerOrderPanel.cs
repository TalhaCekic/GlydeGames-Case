using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CustomerOrderPanel : NetworkBehaviour
{
    public Customer _customer;

    [Header("Spawn System")] public Transform SpawnIconPos;
    public GameObject Canvas;
    [SyncVar] public bool State;
    [SyncVar] public int values; 

    [Header("Sipariş Görsel")] public List<GameObject> SpawnOrderIcon;
    
    public Slider Slider;

    //public GameObject OrderIcon;

    void Start()
    {
        if (isServer)
        {
            ServerStart();
        }
    }

    [Server]
    private void ServerStart()
    {
        RpcStart();
    }

    [ClientRpc]
    private void RpcStart()
    {
        _customer = GetComponent<Customer>();
    }

    void Update()
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
        SelectIconAndCreate();
    }

    [ClientRpc]
    private void RpcUpdate()
    {
        OrderCanvas();
    }

    // Rpc
    private void OrderCanvas()
    {
        if (_customer.isStop&& !_customer.isFinish && !_customer.isEat)
        {
            if (_customer.customerQueue == 1 && !_customer.WaitingForOrder)
            {
                Canvas.SetActive(true);
                _customer.ServerCustomerTime();
            }
            else if (_customer.customerQueue > 0 && !_customer.WaitingForOrder)
            {
                Canvas.SetActive(false);
                _customer.ServerCustomerTime();
            }
            else if(_customer.WaitingForOrder)
            {
                Canvas.SetActive(true);
                _customer.ServerCustomerTime();
            }
        }
        else if (_customer.isFinish)
        {
            Canvas.SetActive(false);
        }
        else if(!_customer.isEat && _customer.isChair)
        {
            Canvas.SetActive(true);
        }
        else
        {
            Canvas.SetActive(false);
        }
    }

    // Server
    [Server]
    private void SelectIconAndCreate()
    {
        if (!State && _customer.isStop)
        {
            for (int i = 0; i < 3; i++)
            {
                foreach (var OrderName in _customer.orderItems)
                {
                    UIOrder uiOrder = _customer.RecipeData.prefabOrderCanvas.GetComponent<UIOrder>();
                    GameObject OrderIcon = Instantiate(uiOrder.gameObject);
                    NetworkServer.Spawn(OrderIcon);
                    values++;
                    RpcParentAndPosIcon(OrderIcon, OrderName.MealName,OrderName.DrinkName,OrderName.SnackName,values);
                    State = true;
                }
            }
        }
    }
    [ClientRpc]
    private void RpcParentAndPosIcon(GameObject obj, string mealName,string drinksName,string snackName,int value)
    {
        SpawnOrderIcon.Add(obj);
        obj.transform.SetParent(SpawnIconPos);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        obj.GetComponent<UIOrder>().value = value;

        switch (obj.GetComponent<UIOrder>().value)
        {
            case 1 :
                obj.GetComponent<UIOrder>().OrderImageName = mealName;
                break;
            case 2 :
                obj.GetComponent<UIOrder>().OrderImageName = drinksName;
                break;
            case 3 :
                obj.GetComponent<UIOrder>().OrderImageName = snackName;
                break;
        }
    }
}
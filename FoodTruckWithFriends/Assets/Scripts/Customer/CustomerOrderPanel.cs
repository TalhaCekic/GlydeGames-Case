using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CustomerOrderPanel : NetworkBehaviour
{
    public Customer _customer;

    [Header("Spawn System")] public Transform SpawnIconPos;
    public GameObject Canvas;
    [SyncVar] public bool State;

    [Header("Sipariş Görsel")] public List<GameObject> SpawnOrderIcon;

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
        if (_customer.isStop)
        {
            if (_customer.customerQueue == 1 && _customer.WaitingForOrder)
            {
                Canvas.SetActive(true);
                _customer.ServerCustomerTime();
            }
            else if (_customer.customerQueue > 0 && !_customer.WaitingForOrder)
            {
                Canvas.SetActive(true);
                _customer.ServerCustomerTime();
            }
            else
            {
                Canvas.SetActive(false);
            }
        }
        else
        {
            Canvas.SetActive(false);
        }
    }

    // Server
    private void SelectIconAndCreate()
    {
        if (!State && _customer.isStop)
        {
            foreach (var OrderName in _customer.orderItems)
            {
                UIOrder uiOrder = _customer.scribtableOrderItem.itemSprite.GetComponent<UIOrder>();
                GameObject OrderIcon = Instantiate(uiOrder.gameObject);
                NetworkServer.Spawn(OrderIcon);
                RpcParentAndPosIcon(OrderIcon, OrderName.itemName,OrderName.quantity);
                State = true;
            }
        }
    }
    [ClientRpc]
    private void RpcParentAndPosIcon(GameObject obj, string name,int number)
    {
        SpawnOrderIcon.Add(obj);
        obj.transform.SetParent(SpawnIconPos);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        
        obj.GetComponent<UIOrder>().OrderImageName = name;
        obj.GetComponent<UIOrder>().Number = number;
    }
}
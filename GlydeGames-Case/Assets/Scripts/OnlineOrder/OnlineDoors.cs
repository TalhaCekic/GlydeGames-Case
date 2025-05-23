using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class OnlineDoors : NetworkBehaviour
{
    public int DoorNo;
    public OnlineOrderCanvas OnlineOrderCanvas;
    public List<OnlineOrderItem> SelectOrders = new List<OnlineOrderItem>();
    
    public Datas _datas;
    public BurgerScreen _BurgerScreen;
    public SnackScreen _SnackScreen;
    public CommentCanvas _CommentCanvas;

    private void Update()
    {
        if (SelectOrders.Count > 0)
        {
           //print(SelectOrders[0].itemName); 
        }
    }
    [ClientRpc]
    public void RpcOrderGive(GameObject PlayerHandObj, GameObject player)
    {
        Tray packet = PlayerHandObj.GetComponent<Tray>();
        if (SelectOrders.Count > 0)
        {
            foreach (var orderItem in SelectOrders)
            {
                if (packet._name == orderItem.itemName)
                {
                    player.GetComponent<PlayerInteract>().HandFullTest(false);
                    player.GetComponent<PlayerInteract>().isHandRightTrue = false;
                    player.GetComponent<PlayerInteract>().InteractObj = null;

                    PlayerHandObj.transform.SetParent(null);
                    PlayerHandObj.transform.localRotation = Quaternion.identity;
                    PlayerHandObj.GetComponent<TrayInteract>().isInteract = false;
                    PlayerHandObj.GetComponent<TrayInteract>().collider.enabled = false;
                    PlayerHandObj.GetComponent<Tray>().Trash();
                    PlayerHandObj.transform.DOMove(transform.position, 0.6f)
                        .OnComplete(() => ComplitePlayerGiveProduct(orderItem, PlayerHandObj,orderItem.itemName,orderItem.OrderNo,orderItem.SellValue));
                }
            }
        }
    }
    private void ComplitePlayerGiveProduct(OnlineOrderItem value,GameObject obj,string itemName,int orderNo,float sellValue)
    {
        SelectOrders.Remove(value);
        OnlineOrderCanvas.RemoveViewElement(itemName,orderNo);
        _BurgerScreen.ListRemoveOnlineOn(value.MealName);
        _SnackScreen.ListRemoveOnlineOn(value.DrinkName);
        _SnackScreen.ListRemoveOnlineOn(value.SnackName);
        _CommentCanvas.AddCart(_datas.HappyCommentData[0]._CustomName,_datas.HappyCommentData[0]._CustomComment,_datas.HappyCommentData[0]._CustomIcon);
        GameManager.instance.MoneyAddAndRemove(true,sellValue,false,null);
        Destroy(obj);
    }
}

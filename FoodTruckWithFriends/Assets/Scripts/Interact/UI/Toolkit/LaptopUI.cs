using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class LaptopUI : NetworkBehaviour
{
    public ProductItem _ProductItem;
    public UIDocument _document;

    private Label _cardNameText1;
    private Label _cardNameText2;

    void Awake()
    {
        _cardNameText1 = _document.rootVisualElement.Q("CardNameText") as Label;
        _cardNameText2 = _document.rootVisualElement.Q("CardNameText") as Label;
    }
    public override void OnStartServer() {
        base.OnStartServer();
        // NameText.text = name;
        // AmountText.text = Amount.ToString();

        _cardNameText1.text = _ProductItem._productItemData.dataList[0]._Name;
        _cardNameText2.text = _ProductItem._productItemData.dataList[1]._Name;

    }

    public override void OnStartClient() {
        base.OnStartClient(); 
        _cardNameText1.text = _ProductItem._productItemData.dataList[0]._Name;
        _cardNameText2.text = _ProductItem._productItemData.dataList[1]._Name;
    }

    private void OnEnable()
    {
        //ButtonTest.UnregisterCallback<ClickEvent>(OnClickButton);
    }
    private void OnDisable()
    {
        //ButtonTest.UnregisterCallback<ClickEvent>(OnClickButton);
    }
    private void OnClickButton(ClickEvent evt)
    {
        if (isLocalPlayer)
        {
            COnClickButton(evt);
        }
    }
    [Command]
    private void COnClickButton(ClickEvent evt)
    {
        Debug.Log("aaaa");
        Test();
    }

    [ClientRpc]
    private void Test()
    {
        Debug.Log("bbbbb");
    }
}

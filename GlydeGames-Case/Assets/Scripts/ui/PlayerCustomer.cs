using System;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.Properties;

public class PlayerCustomer : NetworkBehaviour
{
    [Header("Doc")] public UIDocument _document;
    public MainMenuCanvas _MainMenuCanvas;

    public Transform CustomCamPos;
    public GameObject DefaultCamPos;

    public bool isCustom;
    [Header("ListCount")] public int viewsCount;
    [Header("Button")] private Button[] ViewsButton;
    private Button WebsiteUrlButton;

    private Button SteamUrlButton;

    private Button testty;
    // private Button InviteButton;
    // private Button LeaveButton;
    // private Button QuitButton;

    public override void OnStartLocalPlayer()
    {
        //if (SceneManager.GetActiveScene().buildIndex != 0)return;
    }

    public override void OnStartAuthority()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0) return;

        GameObject MenuCanvas = GameObject.Find("MainMenuDocUI");
        _MainMenuCanvas = MenuCanvas.GetComponent<MainMenuCanvas>();
        _MainMenuCanvas.CustomAdd(this);
    }

    public override void OnStartServer()
    {
        _document.enabled = false;
    }

    public override void OnStartClient()
    {
        _document.enabled = false;
        if (SceneManager.GetActiveScene().buildIndex != 0) return;

        DefaultCamPos = GameObject.Find("CamPos");
    }

    public void ServerCustomStart(bool value)
    {
        _document.enabled = value;
        if (value)
        {
            Camera.main.transform.DOMove(CustomCamPos.position, 1);
        }
        else
        {
            Camera.main.transform.DOMove(DefaultCamPos.transform.position, 1);
        }
    }

    private void t(bool a)
    {
        if(!isLocalPlayer)return;
        print(a);
        _document.enabled = a;
    }

    private void Awake()
    {
        AddVariable();
        ClickEvent();
    }

    private void AddVariable()
    {
        ViewsButton = new Button[viewsCount];
        var root = _document.rootVisualElement;

        for (int i = 0; i < viewsCount; i++)
        {
            ViewsButton[i] = root.Q<Button>($"View{i + 1}");
        }
    }

    private void ClickEvent()
    {
        for (int i = 0; i < viewsCount; i++)
        {
            int index = i;
            ViewsButton[i].clicked += () => viewsButton(index);
        }
    }
    
    // view List
    private void viewsButton(int index)
    {
        if (!isLocalPlayer) return;
        CommandViewsButton(index);
    }

    [Command]
    private void CommandViewsButton(int index)
    {
        RpcViewsButton(index);
    }

    [ClientRpc]
    private void RpcViewsButton(int index)
    {
        var root = _document.rootVisualElement;
        print(index);
    }
}
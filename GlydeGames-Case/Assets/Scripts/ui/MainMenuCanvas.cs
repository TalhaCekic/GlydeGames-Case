using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuCanvas : MonoBehaviour
{
    public static MainMenuCanvas instance;
    public LobbyController lobbyController;
    public SteamLobby steamLobby;
    public PlayerCustomer _PlayerCustomer;
    
    [Header("Doc")] 
    public UIDocument _document;
    
    [Header("Button")] 
    private Button DiscordUrlButton;
    private Button WebsiteUrlButton;
    private Button SteamUrlButton;
    public Button ReadyButton;
    private Button CustomButton;
    private Button InviteButton;
    private Button LeaveButton;
    private Button QuitButton;
    public Label lobbyCode;

    [Header("Url")] 
    public string DiscordUrl;
    public string WebsiteUrl;
    public string SteamUrl;

    public bool isCustomMenu;
    
    [Header("Sound")]
    public AudioSource ClickedButtonSound;
    public AudioSource BringOnSound;

    private void Awake()
    {
        GameObject uıDoc = GameObject.Find("MainMenuDocUI");
        _document = uıDoc.GetComponent<UIDocument>();
        
        DiscordUrlButton = _document.rootVisualElement.Q("Discord_URL_Button") as Button;
        WebsiteUrlButton = _document.rootVisualElement.Q("Website_URL_Button") as Button;
        SteamUrlButton = _document.rootVisualElement.Q("Steam_URL_Button") as Button;   
        ReadyButton = _document.rootVisualElement.Q("ReadyPlayer_Button") as Button;
        CustomButton = _document.rootVisualElement.Q("CustomizePlayer_Button") as Button;
        InviteButton = _document.rootVisualElement.Q("InvitePlayer_Button") as Button;
        LeaveButton = _document.rootVisualElement.Q("Leave_Button") as Button;
        QuitButton = _document.rootVisualElement.Q("Quit_Button") as Button;
        lobbyCode = _document.rootVisualElement.Q("LobbyCode") as Label;
        
        DiscordUrlButton.RegisterCallback<ClickEvent>(DiscordLoaddUrlButton);
        WebsiteUrlButton.RegisterCallback<ClickEvent>(WebsiteLoaddUrlButton);
        SteamUrlButton.RegisterCallback<ClickEvent>(SteamLoaddUrlButton);       
        ReadyButton.RegisterCallback<ClickEvent>(ReadyPlayerButton);
        CustomButton.RegisterCallback<ClickEvent>(CustomPlayerButton);
        InviteButton.RegisterCallback<ClickEvent>(FriendInviteButton);
        LeaveButton.RegisterCallback<ClickEvent>(LobbyLeaveButton);
        QuitButton.RegisterCallback<ClickEvent>(GameQuitButton);
    }
    private void OnDisable()
    {
        DiscordUrlButton.UnregisterCallback<ClickEvent>(DiscordLoaddUrlButton);
        WebsiteUrlButton.UnregisterCallback<ClickEvent>(WebsiteLoaddUrlButton);
        SteamUrlButton.UnregisterCallback<ClickEvent>(SteamLoaddUrlButton); 
        ReadyButton.UnregisterCallback<ClickEvent>(ReadyPlayerButton);
        CustomButton.UnregisterCallback<ClickEvent>(CustomPlayerButton);
        InviteButton.UnregisterCallback<ClickEvent>(FriendInviteButton);
        LeaveButton.UnregisterCallback<ClickEvent>(LobbyLeaveButton);
        QuitButton.UnregisterCallback<ClickEvent>(GameQuitButton);
    }
    void Start()
    {
        instance = this;
        lobbyCode.text = SteamLobby.instance.currentLobbyID.ToString();
    }

    private void DiscordLoaddUrlButton(ClickEvent evt)
    {
        Application.OpenURL(DiscordUrl);
    }
    private void WebsiteLoaddUrlButton(ClickEvent evt)
    {
        Application.OpenURL(WebsiteUrl);
    }
    private void SteamLoaddUrlButton(ClickEvent evt)
    {
        Application.OpenURL(SteamUrl);
    }
    private void ReadyPlayerButton(ClickEvent evt)
    {
        lobbyController.ReadyPlayer();
        ClickedButtonSound.Play();
    }
    private void CustomPlayerButton(ClickEvent evt)
    {
        isCustomMenu = !isCustomMenu;
        _PlayerCustomer.ServerCustomStart(isCustomMenu);
    }
    private void FriendInviteButton(ClickEvent evt)
    {
        lobbyController.InvitePlayer();
    }
    private void LobbyLeaveButton(ClickEvent evt)
    {
        SteamLobby.instance.leaving();
        lobbyCode.text = SteamLobby.instance.currentLobbyID.ToString();
    }
    private void GameQuitButton(ClickEvent evt)
    {
        Application.Quit();
    }
    
    // link eklemek için
    public void abc()
    {
        Application.OpenURL(DiscordUrl);
    }

    public void CustomAdd(PlayerCustomer obj)
    {
        _PlayerCustomer = obj;
    }
}
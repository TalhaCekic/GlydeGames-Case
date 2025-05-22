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
    
    [Header("Doc")] 
    public UIDocument _document;
    
    [Header("Button")] 
    private Button DiscordUrlButton;
    private Button WebsiteUrlButton;
    private Button SteamUrlButton;
    public Button ReadyButton;
    private Button InviteButton;
    private Button LeaveButton;
    private Button QuitButton;

    [Header("Url")] 
    public string DiscordUrl;
    public string WebsiteUrl;
    public string SteamUrl;

    private void Awake()
    {
        GameObject uıDoc = GameObject.Find("MainMenuDocUI");
        _document = uıDoc.GetComponent<UIDocument>();
        
        DiscordUrlButton = _document.rootVisualElement.Q("Discord_URL_Button") as Button;
        WebsiteUrlButton = _document.rootVisualElement.Q("Website_URL_Button") as Button;
        SteamUrlButton = _document.rootVisualElement.Q("Steam_URL_Button") as Button;   
        ReadyButton = _document.rootVisualElement.Q("ReadyPlayer_Button") as Button;
        InviteButton = _document.rootVisualElement.Q("InvitePlayer_Button") as Button;
        LeaveButton = _document.rootVisualElement.Q("Leave_Button") as Button;
        QuitButton = _document.rootVisualElement.Q("Quit_Button") as Button;
        
        DiscordUrlButton.RegisterCallback<ClickEvent>(DiscordLoaddUrlButton);
        WebsiteUrlButton.RegisterCallback<ClickEvent>(WebsiteLoaddUrlButton);
        SteamUrlButton.RegisterCallback<ClickEvent>(SteamLoaddUrlButton);       
        ReadyButton.RegisterCallback<ClickEvent>(ReadyPlayerButton);
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
        InviteButton.UnregisterCallback<ClickEvent>(FriendInviteButton);
        LeaveButton.UnregisterCallback<ClickEvent>(LobbyLeaveButton);
        QuitButton.UnregisterCallback<ClickEvent>(GameQuitButton);
    }
    void Start()
    {
        instance = this;
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
    }
    private void FriendInviteButton(ClickEvent evt)
    {
        lobbyController.InvitePlayer();
    }
    private void LobbyLeaveButton(ClickEvent evt)
    {
        steamLobby.leaving();
    }
    private void GameQuitButton(ClickEvent evt)
    {
        Application.Quit();
    }
    
    
    public void abc()
    {
        Application.OpenURL(DiscordUrl);
    }
}
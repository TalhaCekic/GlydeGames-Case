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
    public Button ReadyButton;
    private Button LeaveButton;
    private Button QuitButton;
    public Label lobbyCode;
    private Label _PlayerCountLabel;

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
          
        ReadyButton = _document.rootVisualElement.Q("ReadyPlayer_Button") as Button;
        LeaveButton = _document.rootVisualElement.Q("Leave_Button") as Button;
        QuitButton = _document.rootVisualElement.Q("Quit_Button") as Button;
        lobbyCode = _document.rootVisualElement.Q("LobbyCode") as Label;
        _PlayerCountLabel = _document.rootVisualElement.Q("PlayerCount_Label") as Label;

            
        ReadyButton.RegisterCallback<ClickEvent>(ReadyPlayerButton);
        LeaveButton.RegisterCallback<ClickEvent>(LobbyLeaveButton);
        QuitButton.RegisterCallback<ClickEvent>(GameQuitButton);
    }
    private void OnDisable()
    {
        ReadyButton.UnregisterCallback<ClickEvent>(ReadyPlayerButton);
        LeaveButton.UnregisterCallback<ClickEvent>(LobbyLeaveButton);
        QuitButton.UnregisterCallback<ClickEvent>(GameQuitButton);
    }
    void Start()
    {
        instance = this;
    }

    private void ReadyPlayerButton(ClickEvent evt)
    {
        lobbyController.ReadyPlayer();
        ClickedButtonSound.Play();
    }
    private void LobbyLeaveButton(ClickEvent evt)
    {
        SteamLobby.instance.leaving();
        SteamLobby.instance.LeaveLobbyAndReturnToMenu();
    }
    private void GameQuitButton(ClickEvent evt)
    {
        Application.Quit();
    }
    public void UpdatePlayerCount(int playerCount)
    {
        _PlayerCountLabel.text = playerCount.ToString();
    }
}
using System;
using Mirror;
using UnityEngine;
using Player.Manager;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace Player.PlayerMenu
{
    public class PlayerMenuManager : NetworkBehaviour
    {
        [Header("Pause")] public bool isPause;
        public GameObject pauseMenu;
        //private InputManager _inputManager;
        private PlayerInteract _playerInteract;
        
        public UIDocument Doc;
        private VisualElement _bgElemet;
        private Button _restartButton;
        private Button _settingsButtom;
        private Button _mainMenuButton;

        public bool mouseActivity;
        public override void OnStartServer()
        {
            if(!isLocalPlayer)return;
            AddCanvasElements();
        }

        public override void OnStartClient()
        {
            if(!isLocalPlayer)return;
            AddCanvasElements();
        }

        private void AddCanvasElements()
        {
            if(!isLocalPlayer)return;
            GameObject _doc = GameObject.Find("PauseDocUI");
            Doc = _doc.GetComponent<UIDocument>();
            
            _bgElemet = Doc.rootVisualElement.Q<VisualElement>("Bg");
            _restartButton = Doc.rootVisualElement.Q<Button>("Restart_Button");
            _settingsButtom = Doc.rootVisualElement.Q<Button>("Settings_Button");
            _mainMenuButton = Doc.rootVisualElement.Q<Button>("Main_Button");
            
            _playerInteract = GetComponent<PlayerInteract>();
            isPause = false;
            pauseMenu.SetActive(false);

            OnPauseSelect();
            _restartButton.clicked += () => ClickedRestartButton();
            _mainMenuButton.clicked += () => ClickedMainMenuButton();
            _settingsButtom.clicked += () => ClickedSettingsButton();
        }
    
        private void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)return;
            if (isLocalPlayer)
            {
                CmdCursorState();
            }
        }
       // [Command]
       public void OnPauseSelect()
       {
           if (SceneManager.GetActiveScene().buildIndex == 0)
           {
               mouseActivity = false;
               _bgElemet.style.display = DisplayStyle.None;
               Cursor.lockState = CursorLockMode.None;
               Cursor.visible = true;
           }
           else
           {
               if (isPause)
               {
                   mouseActivity = true;
                   _bgElemet.style.display = DisplayStyle.Flex;
                   Cursor.lockState = CursorLockMode.None;
                   Cursor.visible = true;
               }
               else
               {
                   mouseActivity = false;
                   _bgElemet.style.display = DisplayStyle.None;
                   Cursor.visible = false;
                   Cursor.lockState = CursorLockMode.Locked;
                   //pauseMenu.SetActive(false);
               }
           }
       }

        //CursorSettings
        [Command]
        public void CmdCursorState()
        {
            CursorState();
        }
        [TargetRpc]
        private void CursorState()
        {
            //_playerInteract = GetComponent<PlayerInteract>();
            if (mouseActivity)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                //InGameHud.instance.RpcCross(true);
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                //InGameHud.instance.RpcCross(false);
            }
        }

        private void ClickedRestartButton()
        {
            print("res");
            SteamLobby.instance.Restart();
        }
        private void ClickedMainMenuButton()
        {
            print("menu");
            SteamLobby.instance.leaving();
        }
        private void ClickedSettingsButton()
        {
            print("setting");
        }
    }
}

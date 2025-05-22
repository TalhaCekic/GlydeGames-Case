using Mirror;
using UnityEngine;
using Player.Manager;

namespace Player.PlayerMenu
{
    public class PlayerMenuManager : NetworkBehaviour
    {
        [Header("Pause")] public bool isPause;
        public GameObject pauseMenu;
        public bool chande;
        private InputManager _inputManager;
        private PlayerInteract _playerInteract;
    
        void Start()
        {
            chande = false;
            isPause = false;
            pauseMenu.SetActive(false);
            _inputManager = GetComponent<InputManager>();
            _playerInteract = GetComponent<PlayerInteract>();
            
            if(!isLocalPlayer)return;
            OnPauseSelect();
        }
    
        private void Update()
        {
            if (isLocalPlayer)
            {
                CursorState();
            }
        }
    
       public void OnPauseSelect()
        {
            if (isPause)
            {
                _playerInteract.mouseActivity = true;
                pauseMenu.SetActive(true);
            }
            else
            {
                _playerInteract.mouseActivity = false;
                pauseMenu.SetActive(false);
            }
        }
    
        public void RestartButton()
        {
            if (isServer)
            {
                SteamLobby.instance.Restart();
            }
        }
    
        public void MainMenuButton()
        {
            SteamLobby.instance.leaving();
            //SteamLobby.instance.MainMenu();
        }
    
        //CursorSettings
        private void CursorState()
        {
            if (_playerInteract.mouseActivity)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}

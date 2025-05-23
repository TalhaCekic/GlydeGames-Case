using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using Player.PlayerMenu;

namespace Player.Manager
{
	public class InputManager : NetworkBehaviour
	{
		public PlayerInteract playerInteract;

		[SerializeField] private PlayerInput PlayerInput;
		[SerializeField] private InputActionAsset InputAssets;
		[SerializeField] private PlayerMenuManager _playerMenuManager;

		public Vector2 Move { get; private set; }
		public Vector2 Look { get; private set; }
		public bool Run { get; private set; }
		public bool Jump { get; private set; }
		public bool Crouch { get; private set; }
		public bool Interact { get; private set; }
		public bool Drop { get; private set; }
		public bool BoxCover { get; private set; }
		public bool Pause { get; private set; }
		public bool Mouse0Click { get; private set; }
		public bool Mouse0Hold { get; private set; }
		public bool Mouse1 { get; private set; }
		public bool Throw { get; private set; }

		private InputActionMap _currentMap;
		private InputActionMap _menuCurrentMap;
		private InputAction _moveAction;
		private InputAction _lookAction;
		private InputAction _runAction;
		private InputAction _jumpAction;
		private InputAction _crouchAction;
		private InputAction _ınteractAction;
		private InputAction _dropAction;
		private InputAction _pauseAction;
		private InputAction _mouse0ClickAction;
		private InputAction _mouse0HoldAction;
		private InputAction _mouse1Action;
		private InputAction _throwAction;

		private void Awake() {
			playerInteract = GetComponent<PlayerInteract>();

			_currentMap = PlayerInput.currentActionMap;
			_menuCurrentMap = InputAssets.FindActionMap("Menu");
			_moveAction = _currentMap.FindAction("Move");
			_lookAction = _currentMap.FindAction("Look");
			_runAction = _currentMap.FindAction("Run");
			_jumpAction = _currentMap.FindAction("Jump");
			_crouchAction = _currentMap.FindAction("Crouch");
			_ınteractAction = _currentMap.FindAction("Interact");
			_dropAction = _currentMap.FindAction("Drop");
			_mouse0ClickAction = _currentMap.FindAction("Mouse0Click");
			_mouse0HoldAction = _currentMap.FindAction("Mouse0Hold");
			_mouse1Action = _currentMap.FindAction("Mouse1");
			_throwAction = _currentMap.FindAction("Throw");
			_pauseAction = _menuCurrentMap.FindAction("Pause");
		
			_moveAction.performed += onMove;
			_lookAction.performed += onLook;
			_runAction.performed += onRun;
			_jumpAction.performed += onJump;
			_crouchAction.started += onCrouch;
			_ınteractAction.started += onInteract;
			_dropAction.started += onDrop;
			_throwAction.started += onThrow;   // fırlatma
			_mouse0ClickAction.started += onMouse0Click;//   (sol Click)
			_mouse1Action.started += onMouse1;//   (sağ Click)
			_pauseAction.started += onPause;
			
			_moveAction.canceled += onMove;
			_lookAction.canceled += onLook;
			_runAction.canceled += onRun;
			_jumpAction.canceled += onJump;
			_crouchAction.canceled += onCrouch;
			_ınteractAction.canceled += onInteract;
			_dropAction.canceled += onDrop;
			_throwAction.canceled += onThrow; // fırlatma
			_mouse0ClickAction.canceled += onMouse0ClickCanceled; // sol click iptali
			_pauseAction.canceled += onPause;
			
		}
		private void Update() {
			// if (_mouse0HoldAction.IsPressed()) {
			// 	Mouse0Hold= true;
			// 	if (isLocalPlayer) {
			// 		playerInteract.BoxInteractShelftoBoxHold();
			// 	}
			// } else {
			// 	Mouse0Hold = false;
			// }

			// if (_mouse1Action.IsPressed()) {
			// 	Mouse1 = true;
			// 	if (isLocalPlayer) {
			// 		playerInteract.BoxInteractBoxtoShelf();
			// 	}
			// } else {
			// 	Mouse1 = false;
			// }
		}
		private void HideCursor() {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void onMove(InputAction.CallbackContext context) {
			Move = context.ReadValue<Vector2>();
		}

		private void onLook(InputAction.CallbackContext context) {
			Look = context.ReadValue<Vector2>();
		}

		private void onRun(InputAction.CallbackContext context) {
			Run = context.ReadValueAsButton();
		}

		private void onJump(InputAction.CallbackContext context) {
			Jump = context.ReadValueAsButton();
		}

		private void onCrouch(InputAction.CallbackContext context) {
			Crouch = context.ReadValueAsButton();
		}

		private void onInteract(InputAction.CallbackContext context) {
			Interact = context.ReadValueAsButton();

			if (!isLocalPlayer) return;
			playerInteract.ServerInteract();
		}
		private void onDrop(InputAction.CallbackContext context) {
			Drop = context.ReadValueAsButton();

			if (!isLocalPlayer) return;
			playerInteract.DropedInteract();
		}

		private void onMouse0Click(InputAction.CallbackContext context) {
			Mouse0Click = context.ReadValueAsButton();

			if (!isLocalPlayer) return;
			if (!playerInteract.mouseActivity)
			{
				playerInteract.BoxInteractShelftoBoxHold();
			}
		}	
		private void onMouse0ClickCanceled(InputAction.CallbackContext context) {
			Mouse0Click = context.ReadValueAsButton();

			if (!isLocalPlayer) return;
		}	
		private void onMouse1(InputAction.CallbackContext context) {
			Mouse1 = context.ReadValueAsButton();

			if (!isLocalPlayer) return;
		}
		private void onThrow(InputAction.CallbackContext context) {
			Throw = context.ReadValueAsButton();

			if (!isLocalPlayer) return;
			playerInteract.Throw();
		}

		private void onPause(InputAction.CallbackContext context) {
			Pause = context.ReadValueAsButton();

			if (!isLocalPlayer) return;
			_playerMenuManager.isPause = !_playerMenuManager.isPause;
			playerInteract.Interact();
			_playerMenuManager.OnPauseSelect();

		}

		private void OnEnable() {
			_currentMap.Enable();
			_menuCurrentMap.Enable();
		}

		private void OnDisable() {
			_currentMap.Disable();
			_menuCurrentMap.Disable();
		}

	}
}

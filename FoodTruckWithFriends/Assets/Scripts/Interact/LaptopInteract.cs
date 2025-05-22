using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class LaptopInteract : NetworkBehaviour
{
	public GameManager gameManager;
	NetworkIdentity Identity;
	[SyncVar] public bool isInteractPC;
	public GameObject CamPos;
	public Button OrderButton; 
	[SyncVar] public bool isOrderPanel;
	public Button ManagementButton;
	[SyncVar] public bool isManagementPanel;

	public Image ShopStateImage;
	public TMP_Text ShopStateText;

	[SerializeField] private GameObject OrderPanelObj;
	[SerializeField] private GameObject ManagementPanelObj;

	void Start() {
		isOrderPanel = false;
		isManagementPanel = false;

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
		//gameManager = GameManager.instance;
	}
	void Update() {
		if (isServer)
		{
			ServerCheckPanel();
		}
	}

	[Server]
	private void ServerCheckPanel() {
		RpcCheckPanel();
	}
	[ClientRpc]
	private void RpcCheckPanel() {
		if (isOrderPanel)
		{
			OrderPanelObj.SetActive(true);
		}
		else
		{
			OrderPanelObj.SetActive(false);
		}

		if (isManagementPanel)
		{
			ManagementPanelObj.SetActive(true);
		}
		else
		{
			ManagementPanelObj.SetActive(false);
		}
	}
	
	// oyuncunun ekrana odaklanmasını sağlar
	public void SetCameraPosition() {
		if (Camera.main != null && CamPos != null)
		{
			Camera.main.transform.DOMove(CamPos.transform.position, 1);
			Camera.main.transform.rotation = CamPos.transform.rotation;
		}
	}
	
	public void CmdSetInteractPC() {
		isInteractPC = !isInteractPC;
	}	
	
	//[Server]
	public void ServerOrderPanelOpen() {
		isOrderPanel = true;
	}

	// dükkan açılışı
	//[Server]
	public void ShopIsOpenState(Button StateButton) {
		gameManager.ShopState(StateButton);
		gameManager.ServerDayState(true);
	}
	public void ShopIsOpenStateChange(Button StateButton) {
		gameManager.StateChange(StateButton);
	}
	[Server]
	public void BackButton() {
		isOrderPanel = false;
		isManagementPanel = false;
	}
}

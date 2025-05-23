using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class LaptopInteract : NetworkBehaviour
{
	public GameManager gameManager;
	[SyncVar] public bool isInteractPC;
	public GameObject CamPos;

	[SerializeField] private GameObject OrderPanelObj;
	[SerializeField] private GameObject ManagementPanelObj;

	void Start() {

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
	
	// dükkan açılışı
	//[Server]
	public void ShopIsOpenState(Button StateButton) {
		gameManager.ShopState(StateButton);
		if(dayManager.instance== null)return;
		if (dayManager.instance.day != 23)
		{
			gameManager.ServerDayState(true);
		}
		
	}
	public void ShopIsOpenStateChange(Button StateButton) {
		gameManager.StateChange(StateButton);
	}
}

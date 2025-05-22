using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class GrillProduct : NetworkBehaviour
{
	public ItemInteract _ıtemInteract;
	[SerializeField] [Min(0)] private float hitRange;
	public GameObject item; 

	[Header("Cook")]
	[SyncVar] public bool isCooking;
	[SyncVar] public float downCookDelay;
	[SyncVar] public bool isDownCooked;
	[SyncVar] public float upCookDelay;
	[SyncVar] public bool isUpCooked;
	public Renderer _renderer;
	public Material RawMaterial;
	public Material BakedMaterial;
	public Transform UpRay;
	public Transform DownRay;

	int CookLayerMask;
	void Start() {
		CookLayerMask = LayerMask.GetMask("Grill");
		if (isServer)
		{
			ServerFreePos();
		}
	}

	void Update() {
		if (isServer && _ıtemInteract.isStateFree)
		{
			RayTest();
			ServerCook();
		}
	}
	// PİŞİRME EYLEMLERİ ////////////////
	[Server]
	private void RayTest() {
		if (_ıtemInteract.isStateFree && isCooking)
		{
			RaycastHit hit;
			if (Physics.Raycast(UpRay.position, UpRay.TransformDirection(Vector3.up), out hit, hitRange, CookLayerMask))
			{
				if (hit.transform.CompareTag("Cook"))
				{
					ServerUpCook();
				}
			}
			else if (Physics.Raycast(DownRay.position, DownRay.TransformDirection(Vector3.down), out hit, hitRange, CookLayerMask))
			{
				if (hit.transform.CompareTag("Cook"))
				{
					ServerDownCook();
				}
			}
			else
			{
				ServerFreePos();
			}
		}
	}
	[Server]
	private void ServerUpCook() {
		upCookDelay -= Time.deltaTime;
		if (upCookDelay < 0)
		{
			isUpCooked = true;
		}
	}
	[Server]
	private void ServerDownCook() {
		downCookDelay -= Time.deltaTime;
		if (downCookDelay < 0)
		{
			isDownCooked = true;
		}
	}
	[Server]
	private void ServerCook() {
		RpcCook();
	}
	[ClientRpc]
	private void RpcCook() {
		if (isDownCooked && isUpCooked)
		{
			_ıtemInteract.isReadyProduct = true;
			if (_ıtemInteract.isReadyProduct)
			{
				_renderer.material = BakedMaterial;
			}
		}

		// ele alırken cook işlemini false yap
		if (!_ıtemInteract.isStateFree)
		{
			isCooking = false;
		}
	}
	[Server]
	private void ServerFreePos() {
		RpcFreePos();
	}
	[ClientRpc]
	private void RpcFreePos() {
		_renderer.material = RawMaterial;
	}

	[ServerCallback]
	public void OnTriggerStay(Collider other) {
		if (other.CompareTag("Cook"))
		{
			isCooking = true;
		}
	}
	[ServerCallback]
	public void OnTriggerExit(Collider other) {
		if (other.CompareTag("Cook"))
		{
			isCooking = false;
		}
	}
	
	// çevirmeli item
	public void ItemRotation() {
		_ıtemInteract.ServerStateChange(false);
		transform.DOMove(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z), 0.5f)
		    .OnComplete((() => complateMove()));
		//item.transform.DORotate(new Vector3(item.transform.eulerAngles.x + 180, item.transform.eulerAngles.y, item.transform.eulerAngles.z), 0.7f, RotateMode.FastBeyond360);
		transform.transform.DORotate(new Vector3(transform.eulerAngles.x + 180, transform.eulerAngles.y, transform.eulerAngles.z), 0.7f, RotateMode.FastBeyond360);
	}
	private void complateMove() {
		_ıtemInteract.ServerStateChange(true);
	}
}

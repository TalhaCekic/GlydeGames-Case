using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class RegisterCardItem : NetworkBehaviour
{
  public GameObject ContentSpawnLocation;
	[SyncVar] public bool isSpawn;
	[SyncVar] public bool isDestroy;

	public Image itemPP;
	public GameObject itemPPObj;
	
	public string itemName;
	public int itemAmount;
	public int itemAmountCurrent;
	public int itemCount;

	public TMP_Text itemNameText;
	public TMP_Text itemAmountText;
	public TMP_Text itemCountText;
	void Start() {
		ContentSpawnLocation = GameObject.FindWithTag("CardList");
	}
	void Update() {
		if (isServer)
		{
			Server();
		}
	}
	[Server]
	private void Server() {
		RpcCount();
		if (isDestroy)
		{
			//Destroy();
		}
	}
	[ClientRpc]
	private void RpcCount() {
		itemCountText.text = itemCount.ToString();
		itemAmountText.text = itemAmountCurrent.ToString();
	}

	[Server]
	public void ServerParentSpawn(GameObject SpawnTransform) {
		if (SpawnTransform != null)
		{
			this.transform.SetParent(SpawnTransform.transform, false);
			isSpawn = false;
		}
	}
	
	[Server]
	public void Destroy()
	{
		//RegisterAddToCartManager.instance.DeleteCartItems(itemAmountCurrent);
		RegisterAddToCartManager.instance.ItemCardItemList.RemoveAll(item => item.name == itemName);
		Destroy(this.gameObject);
	}
	[Server]
	public void DestroyToBuy()
	{
		
		RegisterAddToCartManager.instance.ItemCardItemList.RemoveAll(item => item.name == itemName);
		Destroy(this.gameObject);
	}
}

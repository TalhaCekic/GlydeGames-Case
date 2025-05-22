using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
	
	public void Initialize(string name, int amount, Image pp) {
		itemName = name;
		itemAmount = amount;
		itemAmountCurrent = amount;
		itemNameText.text = itemName;
		itemAmountText.text = itemAmount.ToString();
		itemPP = pp;
		itemPPObj.GetComponent<Image>().sprite= pp.sprite;
		itemCount++;
	}
	public void ParentSpawn(GameObject SpawnTransform) {
		if (isServer)
		{
			ServerParentSpawn(SpawnTransform);
		}
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
	public void ServerAddCount() {
		itemCount++;
		itemAmountCurrent += itemAmount;
	}
	[Server]
	public void Destroy()
	{
		RegisterAddToCartManager.instance.DeleteCartItems(itemAmountCurrent);
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

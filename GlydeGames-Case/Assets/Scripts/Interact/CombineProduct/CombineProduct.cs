using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class CombineProduct : NetworkBehaviour
{
	public ItemInteract _ıtemInteract;
	public string ComplateItemName;
	
	[Header("İçerik")]
	[SyncVar] public bool Meat;
	[SyncVar] public bool Cheese;
	[SyncVar] public bool Tomato;
	[SyncVar] public bool Lettuce;
	
	[Header("İçerik Konumları")]
	public Transform MeatPos;
	public Transform CheesePos;
	public Transform TomatoPos;
	public Transform LettucePos;
	private void Update()
	{
		if (isServer)
		{
			
		}
	}

	[Server]
	private void ItemComplate()
	{
		if (Meat && Cheese && Tomato && Lettuce)
		{
			_ıtemInteract.itemName = ComplateItemName;
			gameObject.name = ComplateItemName;
		}
	}

	public void Combine(string name, GameObject handObj, GameObject BurgerObj) {
		if (name == "Meat")
		{
			Meat = true;
			handObj.transform.SetParent(BurgerObj.transform);
			handObj.transform.rotation = Quaternion.identity;
			handObj.transform.DOMove(MeatPos.position, 0.2f)
			       .SetEase(Ease.OutQuad)
			       .OnComplete(() => parent(handObj, MeatPos));
		}	
		if (name == "Cheese")
		{
			Cheese = true;
			handObj.transform.SetParent(BurgerObj.transform);
			handObj.transform.rotation = Quaternion.identity;
			handObj.transform.DOMove(MeatPos.position, 0.2f)
			       .SetEase(Ease.OutQuad)
			       .OnComplete(() => parent(handObj, CheesePos));
		}	
		if (name == "Tomato")
		{
			Tomato = true;
			handObj.transform.SetParent(BurgerObj.transform);
			handObj.transform.rotation = Quaternion.identity;
			handObj.transform.DOMove(MeatPos.position, 0.2f)
			       .SetEase(Ease.OutQuad)
			       .OnComplete(() => parent(handObj, TomatoPos));
		}	
		if (name == "Lettuce")
		{
			Lettuce = true;
			handObj.transform.SetParent(BurgerObj.transform);
			handObj.transform.rotation = Quaternion.identity;
			handObj.transform.DOMove(MeatPos.position, 0.2f)
			       .SetEase(Ease.OutQuad)
			       .OnComplete(() => parent(handObj, LettucePos));
		}
		ItemComplate();
	}
	private void parent(GameObject handObj,Transform locPos) {
		handObj.transform.position = locPos.position;
	}
}

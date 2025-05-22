using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderItem")]
public class ScribtableOrderItem : ScriptableObject
{
	public String[] OrderItemName;

	public GameObject[] itemPrefab;
	
	public GameObject itemSprite;
	public Sprite[] itemSpriteUI;
}

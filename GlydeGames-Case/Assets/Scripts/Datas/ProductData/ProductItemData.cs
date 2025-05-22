using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewData", menuName = "ScriptableObjects/Data")]
public class ProductItemData : ScriptableObject
{
    public string _Name;
    public int _PurchaseAmount;
    public int _CurrentAmount;
    public Texture2D _Image;

    public RecipeItemData _RecipeItemData;
}

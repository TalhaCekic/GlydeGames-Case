using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodList", menuName = "ScriptableObjects/FoodList")]
public class FoodData : ScriptableObject
{
    public string _name;
    public int _saleValue;
    public Texture2D _image;
    public Sprite _sprite;
}

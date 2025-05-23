using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DrinkData", menuName = "ScriptableObjects/DrinkData")]
public class DrinkData : ScriptableObject
{
    public string _name;
    public int _saleValue;
    public Texture2D _image;
    public Sprite _sprite;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SnackList", menuName = "ScriptableObjects/SnackList")]
public class SnackData : ScriptableObject
{
    public string _name;
    public int _saleValue;
    public Texture2D _image;
    public Sprite _sprite;
}

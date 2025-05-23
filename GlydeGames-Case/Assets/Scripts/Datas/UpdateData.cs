using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpdateList", menuName = "ScriptableObjects/UpdateList")]
public class UpdateData : ScriptableObject
{
    public string _name;
    public int _saleValue;
    public Texture2D _image;
    public Sprite _sprite;
    
    public bool _active;
}

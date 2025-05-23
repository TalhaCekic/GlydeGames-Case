using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "ScriptableObjects/CategoryList")]
public class CategoryData : ScriptableObject
{
    public string _name;
    public int _buyValue;
    //public int _LearnedValue;
    public int _amount;
    public Texture2D _image;
    //public Texture2D _recipeImage;
    
    //public bool _isLearned;

    public GameObject ObjPrefab;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeItem", menuName = "Recipe/RecipeAndItem")]
public class RecipeItemData : ScriptableObject
{
    public string _name;
    public Texture2D _ımage;
    public int _sellAmount;
    public int _currentAmount;
    public Texture2D _recipePath;
    public bool _isLearn;

}

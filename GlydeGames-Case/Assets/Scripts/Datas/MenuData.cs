using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MenuData", menuName = "ScriptableObjects/MenuData")]
public class MenuData : ScriptableObject
{
    public string _name;
    public string _MealName;
    public string _DrinkName;
    public string _SnackName;
    public int _SellValue;
    
}

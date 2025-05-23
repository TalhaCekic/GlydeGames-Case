using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataList", menuName = "ScriptableObjects/DataList")]
public class Datas : ScriptableObject
{
    public CategoryData[] _categoryData;
    public FoodData[] _foodData;
    public DrinkData[] _drinkData;
    public SnackData[] _snackData;

    public MenuData[] _MenuDatas;
    
    public string[] _Menu;
    
    public GameObject prefabOrderCanvas;
    
    public UpdateData[] updateData;
    
    public CommentData[] HappyCommentData;
}

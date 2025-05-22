using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeList", menuName = "Recipe/RecipeAndItem")]
public class RecipeDataList : ScriptableObject
{
    public RecipeItemData[] _ScribtableLearnableItems;
}

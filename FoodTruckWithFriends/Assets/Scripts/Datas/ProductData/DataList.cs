using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataList", menuName = "ScriptableObjects/DataList")]
public class DataList : ScriptableObject
{
    public List<ProductItemData> dataList;
}

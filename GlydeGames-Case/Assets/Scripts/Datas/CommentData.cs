using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "ScriptableObjects/CustomerData")]
public class CommentData : ScriptableObject
{
    public string _CustomName;
    public string _CustomComment;
    public Texture2D _CustomIcon;
}

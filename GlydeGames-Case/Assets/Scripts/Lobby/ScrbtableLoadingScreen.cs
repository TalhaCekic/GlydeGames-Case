using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Loading Screen")]
public class ScribtableLoadingScreen : ScriptableObject
{
    public List<Image> LoadingScreens;
    public List<String> LoadingScreenTips;
}

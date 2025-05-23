using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class OnlineOrderCanvas : MonoBehaviour
{
    public UIDocument doc;

    [Header("OnlineUI")] public VisualTreeAsset OnlineUICard;
    private ScrollView OnlineView;
    private VisualElement onlineCard;
    
    public BurgerScreen burgerScreen;
    public SnackScreen snackScreen;


    public void AddViewElement(string cardName, int cadNo)
    {
        OnlineView = doc.rootVisualElement.Q<ScrollView>("OnlineScrollView");
        onlineCard = OnlineUICard.CloneTree();

        onlineCard.Q<VisualElement>("Card"); // Sepetteki kartı bul
        Label CardName = onlineCard.Q<Label>("OrderName_Label"); // isim  text bul
        Label Cardumara = onlineCard.Q<Label>("OrderNo_Label"); // Toplam Tutar Text bul

        onlineCard.name = cardName+cadNo;
        CardName.text = cardName;
        Cardumara.text = "No : " + cadNo.ToString();
        OnlineView.Add(onlineCard);
        
        OnlineView.schedule.Execute(() => {
            OnlineView.ScrollTo(onlineCard);
        }).ExecuteLater(10);
    }

    public void RemoveViewElement(string cardName, int cadNo)
    {
        foreach (var obj in OnlineView.Children())
        {
            if (obj.name == cardName + cadNo)
            {
                OnlineView.Remove(obj);
                break;
            }
        }
    }
}
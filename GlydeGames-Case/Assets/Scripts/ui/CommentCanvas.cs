using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;
public class CommentCanvas : NetworkBehaviour
{
    public UIDocument Doc;
    public VisualElement Cart;
    public VisualTreeAsset _template;
    public ScrollView CartList;

    [SyncVar] public float CommentDelay;
    [SyncVar] public bool isAddDomment;

    void Start()
    {
        
    }

    void Update()
    {
        if(!isServer)return;
        ServerDelay();
    }

    [Server]
    private void ServerDelay()
    {
        if (isAddDomment)
        {
            if (CommentDelay > 3)
            {
                RpcAddList();
                isAddDomment = false;
                CommentDelay = 0;
            }
            else
            {
                RpcRemoveList();
                CommentDelay += Time.deltaTime;
            }
        }
    }

    [ClientRpc]
    private void RpcAddList()
    {
        CartList.AddToClassList("LeftToRight");
        CartList.RemoveFromClassList("LeftToRightReset");
        CartList.Clear();
    } [ClientRpc]
    private void RpcRemoveList()
    {
        CartList.AddToClassList("LeftToRightReset");
        CartList.RemoveFromClassList("LeftToRight");
    }

    public void AddCart(string name,string content,Texture2D texture)
    {
        isAddDomment = true;
        CartList = Doc.rootVisualElement.Q<ScrollView>("CommentScrollView");
        Cart = _template.CloneTree();

        Cart.Q<VisualElement>("Card"); // Sepetteki kartı bul
        VisualElement Avatar = Cart.Q<VisualElement>("Avatar"); // avatar bul
        Label Comment = Cart.Q<Label>("Comment_Label"); // avatar bul
        Label Name = Cart.Q<Label>("Name_Label"); // avatar bul
        
        Cart.name = name;
        Avatar.style.backgroundImage = texture;
        Comment.text = content;
        Name.text = name;

        CartList.Add(Cart);
        //CartList.schedule.Execute(() => { CartList.ScrollTo(Cart); }).ExecuteLater(10);
    }
}

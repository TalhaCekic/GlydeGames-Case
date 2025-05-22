using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Mirror;

public class PlayerProfileCanvas : NetworkBehaviour
{
    public UIDocument _document;

    private Button ButtonTest;
    //private Button ButtonTest2;
    
    
    void Awake()
    {
         GameObject uıDoc = GameObject.Find("SpaceWorldTest");
        _document = uıDoc.GetComponent<UIDocument>();
 
        ButtonTest = _document.rootVisualElement.Q("Button1") as Button;
        //ButtonTest2 = _document.rootVisualElement.Q("button2") as Button;
        ButtonTest.RegisterCallback<ClickEvent>(OnClickButton);
        //ButtonTest2.RegisterCallback<ClickEvent>(OnClickButton);
        
    }
    void OnEnable()
    {
        _document.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) =>
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(cameraRay.origin,cameraRay.direction*100,Color.magenta);

            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit, 100f, LayerMask.GetMask("UI")))
            {
                Debug.Log("invalidPos");
                return invalidPosition;
            }

            Vector2 pixelUV = hit.textureCoord;
            
            pixelUV.y = 1 - pixelUV.y;
            pixelUV.x *=  _document.panelSettings.targetTexture.width;
            pixelUV.y *=  _document.panelSettings.targetTexture.height;
            
            // var cursor = _document.rootVisualElement.Q<VisualElement>("Cursor");
            // if (cursor != null)
            // {
            //     cursor.style.left = pixelUV.x;
            //     cursor.style.top = pixelUV.y;
            // }
            return pixelUV;
        });
    }
    private void OnDisable()
    {
        ButtonTest.UnregisterCallback<ClickEvent>(OnClickButton);
    }
    private void OnClickButton(ClickEvent evt)
    {
        if (isLocalPlayer)
        {
            COnClickButton(evt);
        }
    }
    
    [Command]
    private void COnClickButton(ClickEvent evt)
    {
        Debug.Log("aaaa");
        Test();
    }

    [ClientRpc]
    private void Test()
    {
        Debug.Log("bbbbb");
    }
}

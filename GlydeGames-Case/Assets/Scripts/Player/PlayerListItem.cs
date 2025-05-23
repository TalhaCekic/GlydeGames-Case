using System;
using Mirror;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;

public class PlayerListItem : NetworkBehaviour
{
    [Header("Datas")] public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    private bool AvatarReceived;

    public TMP_Text PlayerNameText;
    public RawImage PlayerIcon;
    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    [Header("Ready System")] public string ReadyTextString;
    public string notReadyTextString;
    public bool isReady;
    public TMP_Text ReadyText;

    public Image bg;
    
    public void ChangeReadyStatus()
    {
        if (isReady)
        {
            //ReadyText.text = ReadyTextString;
            ReadyText.text = null;
            //ReadyText.color = Color.green;
            bg.color = Color.green;
        }
        else
        {
            //ReadyText.text = notReadyTextString;
            ReadyText.text = null;
            //ReadyText.color = Color.red;
            bg.color =Color.red;
        }
    }

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }
    
    public void SetPlayerValues()
    {
        //PlayerNameText.text = PlayerName;
        PlayerNameText.text = null;
        ChangeReadyStatus();
        if (!AvatarReceived)
        {
            GetPlayerIcon();
        }
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if (ImageID == -1)
        {
            return;
        }

        PlayerIcon.texture = GetSteamImageAsTexture(ImageID);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));
            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        AvatarReceived = true;
        return texture;
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == PlayerSteamID)
        {
            PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
        {
            return;
        }
    }
}
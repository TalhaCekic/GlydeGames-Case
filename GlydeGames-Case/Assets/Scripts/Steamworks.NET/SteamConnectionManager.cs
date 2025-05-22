using UnityEngine;
using Steamworks;

public class SteamConnectionManager : MonoBehaviour
{
    private Callback<SteamNetConnectionStatusChangedCallback_t> m_ConnectionStatusChanged;

    private void OnEnable()
    {
        m_ConnectionStatusChanged = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnSteamNetConnectionStatusChanged);
    }

    private void OnSteamNetConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t param)
    {
        if (param.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
        {
            Debug.Log("Ba�lant� ba�ar�l�.");
        }
        else if (param.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer || param.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally)
        {
            Debug.Log("Ba�lant� kesildi, tekrar denenecek...");
            // Burada tekrar ba�lanma i�lemini ger�ekle�tirebilirsiniz.
            ConnectToServer(); // Ba�lant�y� yeniden ba�latan �rnek bir fonksiyon.
        }
    }

    private void ConnectToServer()
    {
        // Ba�lant�y� yeniden ba�latmak i�in gerekli i�lemleri ger�ekle�tirin.
        // Bu fonksiyon, Steam sunucusuna tekrar ba�lanmay� denemelidir.
       // SteamManager.Instance.Initialize();
    }
}

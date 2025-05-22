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
            Debug.Log("Baðlantý baþarýlý.");
        }
        else if (param.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer || param.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally)
        {
            Debug.Log("Baðlantý kesildi, tekrar denenecek...");
            // Burada tekrar baðlanma iþlemini gerçekleþtirebilirsiniz.
            ConnectToServer(); // Baðlantýyý yeniden baþlatan örnek bir fonksiyon.
        }
    }

    private void ConnectToServer()
    {
        // Baðlantýyý yeniden baþlatmak için gerekli iþlemleri gerçekleþtirin.
        // Bu fonksiyon, Steam sunucusuna tekrar baðlanmayý denemelidir.
       // SteamManager.Instance.Initialize();
    }
}

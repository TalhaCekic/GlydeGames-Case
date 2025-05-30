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
            Debug.Log("Bağlantı başarılı.");
        }
        else if (param.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer || param.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally)
        {
            Debug.Log("Bağlantı kesildi, tekrar denenecek...");
            // Burada tekrar bağlanma işlemini gerçekleştirebilirsiniz.
            ConnectToServer(); // Bağlantıyı yeniden başlatan örnek bir fonksiyon.
        }
    }

    private void ConnectToServer()
    {
        // Bağlantıyı yeniden başlatmak için gerekli işlemleri gerçekleştirin.
        // Bu fonksiyon, Steam sunucusuna tekrar bağlanmayı denemelidir.
       // SteamManager.Instance.Initialize();
    }
}

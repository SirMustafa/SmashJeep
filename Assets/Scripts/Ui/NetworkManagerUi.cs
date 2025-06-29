using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUi : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Hide();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Hide();
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
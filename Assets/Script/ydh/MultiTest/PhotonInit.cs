using Photon.Pun;
using UnityEngine;

public class PhotonInit : MonoBehaviour
{
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("[PhotonInit] 씬 동기화 비활성화");
    }
}
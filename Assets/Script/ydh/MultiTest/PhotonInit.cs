using Photon.Pun;
using UnityEngine;

public class PhotonInit : MonoBehaviour
{
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        Debug.Log("[PhotonInit] 씬 동기화 설정 완료");
    }
}
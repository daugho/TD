using Photon.Pun;
using UnityEngine;

public class PhotonInit : MonoBehaviour
{
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("[PhotonInit] �� ����ȭ ��Ȱ��ȭ");
    }
}
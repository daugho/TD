using Photon.Pun;
using UnityEngine;

public class PhotonInit : MonoBehaviour
{
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        Debug.Log("[PhotonInit] �� ����ȭ ���� �Ϸ�");
    }
}
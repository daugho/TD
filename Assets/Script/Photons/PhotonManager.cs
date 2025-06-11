using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public const byte BID_EVENT = 1;
    public const byte AUCTION_COMPLETE_EVENT = 3;

    private void Start()
    {
        // Photon ���� ����
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("���� ������ ������ �����Ͽ����ϴ�.");
        PhotonNetwork.JoinRandomRoom();
        // ���� �뿡 �����ϰų� ���ο� ���� ����
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("�� ������ �����Ͽ����ϴ�. ���� ���� ����ϴ�.");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        //Debug.Log("�뿡 �����Ͽ����ϴ�.");

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("�뿡 �����Ͽ����ϴ�.");

            //for (int x = 0; x < 4; x++)
            //{
            //    for (int z = 0; z < 4; z++)
            //    {
            //        Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
            //        PhotonNetwork.Instantiate("Tile", pos, Quaternion.identity);
            //    }
            //}
        }
    }
    // Instantiate - ������
    // Destroy - �ı���
    // RPC -> Remote Procedure call 
    // RaiseEvent
    // ���� -> RPC = ���� �ش� ��ü�� PhotonView�� ���� ȣ��
    // ���� -> RaiseEvent = �̺�Ʈ ��� ( Photon ������ ���� ���޵� )
}

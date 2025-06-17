using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
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
        }
    }
    // Instantiate - ������
    // Destroy - �ı���
    // RPC -> Remote Procedure call 
    // RaiseEvent
    // ���� -> RPC = ���� �ش� ��ü�� PhotonView�� ���� ȣ��
    // ���� -> RaiseEvent = �̺�Ʈ ��� ( Photon ������ ���� ���޵� )
}

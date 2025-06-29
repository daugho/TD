using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public const byte BID_EVENT = 1;
    public const byte AUCTION_COMPLETE_EVENT = 3;

    private void Start()
    {
        // Photon 서버 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("포톤 마스터 서버에 연결하였습니다.");
        PhotonNetwork.JoinRandomRoom();
        // 랜덤 룸에 참가하거나 새로운 룸을 생성
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방 참가에 실패하였습니다. 방을 새로 만듭니다.");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        //Debug.Log("룸에 접속하였습니다.");

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("룸에 접속하였습니다.");
        }
    }
    // Instantiate - 생성자
    // Destroy - 파괴자
    // RPC -> Remote Procedure call 
    // RaiseEvent
    // 서버 -> RPC = 직접 해당 객체의 PhotonView를 통해 호출
    // 서버 -> RaiseEvent = 이벤트 기반 ( Photon 서버를 거쳐 전달됨 )
}

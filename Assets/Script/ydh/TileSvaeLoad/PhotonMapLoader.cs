using Photon.Pun;
using UnityEngine;

public class PhotonMapLoader : MonoBehaviourPunCallbacks
{
    [SerializeField] private FirebaseMapDownloader downloader;
    [SerializeField] private GridManager gridManager;

    public async void LoadMapAndSync(string mapName)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        string fileName = mapName + ".json";
        string json = await downloader.DownloadMap(fileName);

        if (!string.IsNullOrEmpty(json))
        {
            photonView.RPC(nameof(RPC_LoadMap), RpcTarget.AllBuffered, json);//현재 클라이언트 + 이후 입장할 클라이언트에게도 적용됨 (버퍼에 저장됨)
        }
    }

    [PunRPC]
    private void RPC_LoadMap(string json)
    {
        gridManager.LoadMapFromFirebase(json);
    }
}

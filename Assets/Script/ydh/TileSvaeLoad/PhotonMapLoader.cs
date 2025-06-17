using Photon.Pun;
using UnityEngine;
using System.Collections;

public class PhotonMapLoader : MonoBehaviourPunCallbacks
{
    [SerializeField] private FirebaseMapDownloader downloader;
    [SerializeField] private GridManager gridManager;

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("MapName"))
        {
            string mapName = propertiesThatChanged["MapName"] as string;
            Debug.Log($"[PhotonMapLoader] MapName 변경 감지: {mapName}");
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(DelayedMapLoadAndSync(mapName));
            }
        }
    }
    private IEnumerator DelayedMapLoadAndSync(string mapName)
    {
        yield return new WaitForSeconds(1.5f); // 씬 로드 완료 후 딜레이

        var downloadTask = downloader.DownloadMap(mapName + ".json");
        while (!downloadTask.IsCompleted) yield return null;

        string json = downloadTask.Result;
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("[PhotonMapLoader] 맵 데이터가 비어 있음");
            yield break;
        }

        // RPC 호출로 클라이언트도 타일 동기화
        photonView.RPC(nameof(RPC_LoadMap), RpcTarget.AllBuffered, json);
    }
    public IEnumerator LoadMapAndSync(string mapName)
    {
        var downloadTask = downloader.DownloadMap(mapName + ".json");
        while (!downloadTask.IsCompleted) yield return null;

        string json = downloadTask.Result;
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("맵 데이터가 비어 있음");
            yield break;
        }

        photonView.RPC(nameof(RPC_LoadMap), RpcTarget.AllBuffered, json);
    }

    [PunRPC]
    private void RPC_LoadMap(string json)
    {
        gridManager.LoadMapFromFirebase(json);
    }
}

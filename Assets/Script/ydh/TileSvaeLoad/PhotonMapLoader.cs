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
            Debug.Log($"[PhotonMapLoader] MapName ���� ����: {mapName}");
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(LoadMapAndSync(mapName));
            }
        }
    }
    public IEnumerator LoadMapAndSync(string mapName)
    {
        yield return new WaitForSeconds(0.2f);

        var downloadTask = downloader.DownloadMap(mapName + ".json");
        while (!downloadTask.IsCompleted) yield return null;

        string json = downloadTask.Result;
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("�� �����Ͱ� ��� ����");
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

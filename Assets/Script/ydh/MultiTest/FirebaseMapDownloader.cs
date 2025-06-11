using Firebase.Storage;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseMapDownloader : MonoBehaviour
{
    private FirebaseStorage storage;
    private StorageReference mapFolderRef;

    private void Awake()
    {
        storage = FirebaseStorage.DefaultInstance;
        mapFolderRef = storage.GetReference("maps");
    }

    public async Task<string> DownloadMap(string fileName)
    {
        var fileRef = mapFolderRef.Child(fileName);

        // 1. 다운로드 URL 가져오기
        Uri downloadUri;
        try
        {
            downloadUri = await fileRef.GetDownloadUrlAsync();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"? 다운로드 URL 얻기 실패: {ex.Message}");
            return null;
        }

        // 2. UnityWebRequest로 JSON 파일 다운로드
        using var request = UnityWebRequest.Get(downloadUri.ToString());
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"? 맵 다운로드 실패: {request.error}");
            return null;
        }

        // 3. 정상적으로 다운로드된 JSON 텍스트 반환
        Debug.Log($"? 맵 다운로드 성공: {fileName}");
        return request.downloadHandler.text;

    }

}

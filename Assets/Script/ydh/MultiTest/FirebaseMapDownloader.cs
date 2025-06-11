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

        // 1. �ٿ�ε� URL ��������
        Uri downloadUri;
        try
        {
            downloadUri = await fileRef.GetDownloadUrlAsync();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"? �ٿ�ε� URL ��� ����: {ex.Message}");
            return null;
        }

        // 2. UnityWebRequest�� JSON ���� �ٿ�ε�
        using var request = UnityWebRequest.Get(downloadUri.ToString());
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"? �� �ٿ�ε� ����: {request.error}");
            return null;
        }

        // 3. ���������� �ٿ�ε�� JSON �ؽ�Ʈ ��ȯ
        Debug.Log($"? �� �ٿ�ε� ����: {fileName}");
        return request.downloadHandler.text;

    }

}

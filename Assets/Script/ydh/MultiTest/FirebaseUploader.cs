using Firebase.Storage;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseMapUploader : MonoBehaviour
{
    private FirebaseStorage storage;
    private StorageReference mapFolderRef;

    private void Awake()
    {
        storage = FirebaseStorage.DefaultInstance;
        mapFolderRef = storage.GetReference("maps"); // Firebase Storage > maps ����
    }

    public async Task UploadMap(string fileName, string jsonContent)
    {
        var storage = FirebaseStorage.DefaultInstance;
        var mapFolderRef = storage.GetReference("maps");
        var fileRef = mapFolderRef.Child(fileName);

        byte[] bytes = Encoding.UTF8.GetBytes(jsonContent);

        try
        {
            // ?? ���⼭ �ٷ� ���ε�
            await fileRef.PutBytesAsync(bytes);
            Debug.Log($"? �� ���ε� �Ϸ�: {fileName}");

            // ? (����) ���ε� �� �ٿ�ε� URL ��û
            // Uri uri = await fileRef.GetDownloadUrlAsync();
            // Debug.Log($"?? �ٿ�ε� ��ũ: {uri.AbsoluteUri}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"? �� ���ε� ����: {ex.Message}");
        }
    }
}

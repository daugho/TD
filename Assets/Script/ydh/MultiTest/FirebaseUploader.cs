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
        mapFolderRef = storage.GetReference("maps"); // Firebase Storage > maps 폴더
    }

    public async Task UploadMap(string fileName, string jsonContent)
    {
        var storage = FirebaseStorage.DefaultInstance;
        var mapFolderRef = storage.GetReference("maps");
        var fileRef = mapFolderRef.Child(fileName);

        byte[] bytes = Encoding.UTF8.GetBytes(jsonContent);

        try
        {
            // ?? 여기서 바로 업로드
            await fileRef.PutBytesAsync(bytes);
            Debug.Log($"? 맵 업로드 완료: {fileName}");

            // ? (선택) 업로드 후 다운로드 URL 요청
            // Uri uri = await fileRef.GetDownloadUrlAsync();
            // Debug.Log($"?? 다운로드 링크: {uri.AbsoluteUri}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"? 맵 업로드 실패: {ex.Message}");
        }
    }
}

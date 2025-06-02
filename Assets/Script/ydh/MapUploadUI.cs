using UnityEngine;
using TMPro;

public class MapUploadUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField fileNameInput;
    [SerializeField] private FirebaseMapUploader uploader;
    [SerializeField] private GridManager gridManager;

    public async void OnClickUploadButton()
    {
        string fileName = fileNameInput.text;
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Debug.LogWarning("파일 이름이 비어 있습니다.");
            return;
        }

        string json = gridManager.GetCurrentMapJson();
        await uploader.UploadMap(fileName + ".json", json);
    }
}

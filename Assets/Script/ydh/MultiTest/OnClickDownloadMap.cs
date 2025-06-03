using TMPro;
using UnityEngine;

public class OnClickDownloadMap : MonoBehaviour
{
    [SerializeField] private FirebaseMapDownloader downloader;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private TMP_InputField fileNameInput;

    public async void OnClickDownloadFromFirebase()
    {
        string fileName = fileNameInput.text + ".json";
        string json = await downloader.DownloadMap(fileName);

        if (!string.IsNullOrEmpty(json))
        {
            gridManager.LoadMapFromFirebase(json);
        }
    }

}

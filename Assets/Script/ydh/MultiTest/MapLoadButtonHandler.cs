using TMPro;
using UnityEngine;

public class MapLoadButtonHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField fileNameInput;
    [SerializeField] private PhotonMapLoader photonMapLoader;

    public void OnClickLoadButton()
    {
        string mapName = fileNameInput.text;
        photonMapLoader.LoadMapAndSync(mapName);
    }
}

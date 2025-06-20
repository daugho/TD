using TMPro;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class StageButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;

    public void OnClickStage()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        string mapName = buttonText.text.Trim(); // 버튼에 써있는 맵 이름 사용

        Hashtable props = new Hashtable
        {
            { "MapName", mapName }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        PhotonNetwork.LoadLevel("InGameScene");
    }

}


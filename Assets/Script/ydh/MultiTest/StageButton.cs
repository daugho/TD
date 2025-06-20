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

        string mapName = buttonText.text.Trim(); // ��ư�� ���ִ� �� �̸� ���

        Hashtable props = new Hashtable
        {
            { "MapName", mapName }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        PhotonNetwork.LoadLevel("InGameScene");
    }

}


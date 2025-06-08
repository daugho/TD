using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button startButton;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            startButton.interactable = false; // 버튼 비활성화
        }
    }
}

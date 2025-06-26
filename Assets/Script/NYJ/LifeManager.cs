using UnityEngine;
using Photon.Pun;

public class LifeManager : MonoBehaviourPun
{
    public static LifeManager Instance;

    private int _currentLife = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    [PunRPC]
    public void DecreaseLifeRPC()
    {
        _currentLife = Mathf.Max(0, _currentLife - 1);
        Lifepoint.Instance.UpdateLifeUI(_currentLife);

        GameResultData.Instance.SetLife(_currentLife);
    }

    public void RequestDecrease()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("DecreaseLifeRPC", RpcTarget.AllBuffered);
        }
    }
}

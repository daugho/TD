using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonIDSwapper : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _ydhAppId;
    [SerializeField] private string _nyjAppId;
    [SerializeField] private string _lshAppId;

    private string _targetSceneName;

    public void UseydhAppId()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = _ydhAppId;
        _targetSceneName = "daughoPlayTest";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void UsenyjAppId()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = _nyjAppId;
        _targetSceneName = "YongJuScene";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void UselshAppId()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = _lshAppId;
        _targetSceneName = "YDHScene";
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to Photon Master Server. Loading scene: {_targetSceneName}");
        SceneManager.LoadScene(_targetSceneName);
    }
}

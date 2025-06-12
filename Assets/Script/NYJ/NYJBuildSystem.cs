using UnityEngine;
using Photon.Pun;
using UnityEditor.PackageManager;
public class NYJBuildSystem : MonoBehaviour
{
    private Vector3 _testPos = new Vector3(3, 1, -2);
    private Vector3 _testPos1 = new Vector3(4, 1, -2);
    private Material _material;
    private void Awake()
    {
        _material = Resources.Load<Material>("Materials/Weapons_5");
    }
    public void OnClickBuildButton()
    {
        GameObject turretInstance;
        TowerTypes towerType;

        if (PhotonNetwork.IsMasterClient)
        {
            turretInstance = PhotonNetwork.Instantiate("Prefabs/Turrets/MissileTower_Master", _testPos, Quaternion.identity);
            towerType = TowerTypes.MissileTower;
        }
        else
        {
            turretInstance = PhotonNetwork.Instantiate("Prefabs/Turrets/GrenadeTower_Client", _testPos1, Quaternion.identity);
            towerType = TowerTypes.GrenadeTower;
        }

        PhotonView view = turretInstance.GetComponent<PhotonView>();
        view.RPC("OnBuildComplete", RpcTarget.AllBuffered, (int)towerType);
    }
}

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
            turretInstance = PhotonNetwork.Instantiate("Prefabs/Turrets/MachinegunTower_Master", _testPos, Quaternion.identity);
            towerType = TowerTypes.MachinegunTower;
        }
        else
        {
            turretInstance = PhotonNetwork.Instantiate("Prefabs/Turrets/RailgunTower_Client", _testPos1, Quaternion.identity);
            towerType = TowerTypes.RailgunTower;
        }

        PhotonView view = turretInstance.GetComponent<PhotonView>();
        view.RPC("OnBuildComplete", RpcTarget.AllBuffered, (int)towerType);
    }
}

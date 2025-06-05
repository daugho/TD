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
        if(PhotonNetwork.IsMasterClient)
        {
            GameObject masterTurret = PhotonNetwork.Instantiate
                ("Prefabs/Turrets/MachinegunTower_Master", _testPos, Quaternion.identity);
            Turret turret = masterTurret.GetComponent<Turret>();

            TowerTypes tower = TowerTypes.MachinegunTower;
            turret.InitTurret(tower);
        }
        else
        { // 나중에 타워설치랑 연동 수정  
            GameObject clientTurret = PhotonNetwork.Instantiate
                ("Prefabs/Turrets/RailgunTower_Client", _testPos1, Quaternion.identity);
            Turret turret = clientTurret.GetComponent<Turret>();

            TowerTypes tower = TowerTypes.RailgunTower;
            turret.InitTurret(tower);
        }
    }
}

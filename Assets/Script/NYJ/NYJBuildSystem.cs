using UnityEngine;
using Photon.Pun;
using UnityEditor.PackageManager;
public class NYJBuildSystem : MonoBehaviour
{
    private void Awake()
    {
    }
    public void OnClickBuildButton()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.Instantiate("Prefabs/Turrets/MachinegunTower", )
        }
    }
}

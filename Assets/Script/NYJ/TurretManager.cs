using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TurretManager : MonoBehaviour
{
    [SerializeField] private string[] _turretNames;

    private Dictionary<TowerTypes, List<GameObject>> _masterTurretPools = new Dictionary<TowerTypes, List<GameObject>>();
    private Dictionary<TowerTypes, List<GameObject>> _clientTurretPools = new Dictionary<TowerTypes, List<GameObject>>();

    private int _poolSize = 5;
    private PhotonView _photonView;

    public static TurretManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            InitializeMasterPools();
        }
        else
        {
            InitializeClientPools();
        }
    }

    private void InitializeMasterPools()
    {
        foreach (string turretName in _turretNames)
        {
            TowerTypes type = (TowerTypes)System.Enum.Parse(typeof(TowerTypes), turretName);

            _masterTurretPools[type] = new List<GameObject>();
    
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject turret = PhotonNetwork.Instantiate($"Prefabs/Turrets/{turretName}" + "_Master"
                    , Vector3.zero, Quaternion.identity);
                PhotonView view = turret.GetComponent<PhotonView>();
               
                view.RPC("OnBuildComplete", RpcTarget.AllBuffered, type);
                _masterTurretPools[type].Add(turret);
            }
        }
        TowerTypes test = TowerTypes.RifleTower;
        Debug.Log(_masterTurretPools[test].Count);
    }

    private void InitializeClientPools()
    {
        foreach (string turretName in _turretNames)
        {
            TowerTypes type = (TowerTypes)System.Enum.Parse(typeof(TowerTypes), turretName);

            _clientTurretPools[type] = new List<GameObject>();

            for (int i = 0; i < _poolSize; i++)
            {
                GameObject turret = PhotonNetwork.Instantiate($"Prefabs/Turrets/{turretName}" + "_Client"
                    , Vector3.zero, Quaternion.identity);
                PhotonView view = turret.GetComponent<PhotonView>();
                
                view.RPC("OnBuildComplete", RpcTarget.AllBuffered, type);
                _clientTurretPools[type].Add(turret);
            }
        }
    }

    public GameObject GetAvailableTurret(TowerTypes type)
    {
        Dictionary<TowerTypes, List<GameObject>> pool =
        PhotonNetwork.IsMasterClient ? _masterTurretPools : _clientTurretPools;
       
        GameObject turret = pool[type].FirstOrDefault(t => !t.activeSelf);

        return turret;
    }

    public void ReturnTurret(GameObject turret)
    {
        turret.SetActive(false);
    }
}

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    protected Monster _target;
    protected GameObject _fireEffectPrefab;
    private GameObject _activeFireEffect;

    private float _searchInterval = 0.2f;
    private float _searchTimer = 0.0f;

    protected TurretData _turretData = new TurretData();
    protected TurretHead _turretHead;
    protected Bullet _bullet;
    [SerializeField] protected Vector3 _firePosition = Vector3.forward;

    protected float _spawnTimer = 0.0f;
    protected float _bulletSpawnTimer = 1.0f;

    private float _rangeSqr;

    private PhotonView _photonView;
    private TurretRarity _turretRarity;

    private void Awake()
    {
        _turretHead = GetComponentInChildren<TurretHead>();
        _photonView = GetComponentInChildren<PhotonView>();
    }


    protected virtual void Update()
    {
        _searchTimer += Time.deltaTime;
        if (_searchTimer >= _searchInterval)
        {
            _searchTimer = 0.0f;
            FindTarget();
        }

        _spawnTimer += _turretData.AtkSpeed * Time.deltaTime;


        if (PhotonNetwork.IsMasterClient)
        {
            _spawnTimer += _turretData.AtkSpeed * Time.deltaTime;

            if (_target)
            {
                if (_spawnTimer >= _bulletSpawnTimer)
                {
                    _spawnTimer -= _bulletSpawnTimer;

                    Vector3 worldPosition = _turretHead.transform.TransformPoint(_firePosition);
                    Vector3 targetPos = _target.transform.position;

                    if (_turretData.Type == "Direct")
                    {
                        if (_turretData.Name == TowerTypes.FlameTower)
                        {
                            AttackFlame();
                        }
                        else
                        {
                            _photonView.RPC("RPC_SpawnBullet", RpcTarget.All, worldPosition, targetPos);
                        }
                    }
                    else
                    {
                        _photonView.RPC("RPC_SpawnBullet", RpcTarget.All, worldPosition, targetPos);
                    }
                }
            }
        }
        else
        {
            if (_activeFireEffect != null && _activeFireEffect.activeSelf && _target == null)
            {
                _activeFireEffect.SetActive(false);
            }
        }
    }
    public void FindTarget()
    {
        if (_target != null)
        {
            float distSqr = (_target.transform.position - transform.position).sqrMagnitude;
            if (distSqr > _rangeSqr)
            {
                _target = null;
            }
            else
            {
                return; 
            }
        }

        List<Monster> allMonsters = MonsterManager.Monsters;
        float closestDist = float.MaxValue;
        Monster closest = null;

        foreach (Monster m in allMonsters)
        {
            if (m == null) continue;

            float distSqr = (m.transform.position - transform.position).sqrMagnitude;
            if (distSqr <= _rangeSqr && distSqr < closestDist)
            {
                closest = m;
                closestDist = distSqr;
            }
        }

        _target = closest;
        _turretHead.SetTarget(_target);
    }

    public bool GetTarget()
    {
        return _target != null;
    }

    [PunRPC]
    private void RPC_SpawnBullet(Vector3 firePosition, Vector3 targetPosition)
    {
        if (_activeFireEffect == null)
        {
            _activeFireEffect = Instantiate(_fireEffectPrefab, firePosition, Quaternion.identity, _turretHead.transform);
        }
        else
        {
            _activeFireEffect.transform.position = firePosition;
            if (!_activeFireEffect.activeSelf)
                _activeFireEffect.SetActive(true);
        }

        Bullet bulletInstance = Instantiate(_bullet, firePosition, Quaternion.identity);
        //bulletInstance.SetBulletTargetPosition(targetPosition); // 타겟이 아닌 위치로
        bulletInstance.SetBulletTarget(_target);
        bulletInstance.SetBullet(_turretData.BulletSpeed, _turretData.Atk, _turretData.HitEffectPath);
    }

    protected virtual void AttackTarget()
    {
        Vector3 worldPosition = _turretHead.transform.TransformPoint(_firePosition);
       
        if (_activeFireEffect == null)
        {
            _activeFireEffect = Instantiate(_fireEffectPrefab, worldPosition, Quaternion.identity, _turretHead.transform);
        }
        else if (!_activeFireEffect.activeSelf)
        {
            _activeFireEffect.transform.position = worldPosition;
            _activeFireEffect.SetActive(true);
        }

        Bullet bulletInstance = Instantiate(_bullet, worldPosition, Quaternion.identity);
        bulletInstance.SetBulletTarget(_target);
        bulletInstance.SetBullet(_turretData.BulletSpeed, _turretData.Atk, _turretData.HitEffectPath);
    }

    private void AttackFlame()
    {
        Vector3 worldPosition = _turretHead.transform.TransformPoint(_firePosition);

        //if (_activeFlameEffect == null)
        //{
        //    GameObject prefab = Resources.Load<GameObject>("Prefabs/HitEffects/WFXMR_FlameThrower Big Alt Looped");
        //    _activeFlameEffect = Instantiate(prefab, worldPosition, transform.rotation, _turretHead.transform);
        //}
        //else
        //{
        //    _activeFlameEffect.transform.position = worldPosition;
        //    _activeFlameEffect.transform.rotation = transform.rotation;
        //
        //    if (!_activeFlameEffect.activeSelf)
        //    {
        //        _activeFlameEffect.SetActive(true);
        //    }
        //}
    }

    [PunRPC]
    public void OnBuildComplete(int towerTypeInt)
    {
        TowerTypes type = (TowerTypes)towerTypeInt;
        InitTurret(type);
    }
    public void InitTurret(TowerTypes type)
    {
        _turretData = DataManager.Instance.GetTurretData(type);

        GameObject rarityPrefab = Resources.Load<GameObject>("Prefabs/Turrets/RarityCircle");
        GameObject instance = Instantiate(rarityPrefab, transform);
        _turretRarity = instance.GetComponent<TurretRarity>();
        _turretRarity.SetRarityVisual(_turretData.Rarity);

        _rangeSqr = _turretData.Range * _turretData.Range;  

        _bullet = Resources.Load<Bullet>("Prefabs/Bullets/" + _turretData.Bullet);
        _fireEffectPrefab = Resources.Load<GameObject>("Prefabs/FireEffects/" + _turretData.FireEffectPath);
    }
    public void BuildTurret(string turretName, Vector3 position)
    {
        GameObject turretPrefab = Resources.Load<GameObject>("Prefabs/Turrets/" + turretName);

        GameObject turretInstance = PhotonNetwork.Instantiate("Prefabs/Turrets/" + turretName, position, Quaternion.identity);

        PhotonView turretView = turretInstance.GetComponent<PhotonView>();
        if (turretView.IsMine)
        {
            turretView.RPC("OnBuildComplete", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void OnBuildComplete()
    {
        // 설치 시 시각 효과, 초기 타겟 스캔, UI 연결 등 처리
        Debug.Log($"{gameObject.name} 터렛이 설치 완료됨");
    }
    protected virtual void OnDrawSelectedGizmos()
    {
        _turretHead = GetComponentInChildren<TurretHead>();
        Vector3 worldPosition = _turretHead.transform.TransformPoint(_firePosition);

        Gizmos.DrawSphere(worldPosition, 0.01f);
    }
    private void OnDrawGizmos() // 범위 체크 
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(_turretData.Range));
    }
}

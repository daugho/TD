using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    protected Monster _target;
    protected GameObject _fireEffectPrefab;
    private GameObject _fireEffectInstance;

    private float _searchInterval = 0.2f;
    private float _searchTimer = 0.0f;

    public TurretData MyTurretData = new TurretData();
    protected TurretHead _turretHead;
    protected Bullet _bullet;
    [SerializeField] protected Vector3 _firePosition = Vector3.forward;

    protected float _spawnTimer = 0.0f;
    protected float _bulletSpawnTimer = 1.0f;

    protected float _rangeSqr;

    protected PhotonView _photonView;
    public TurretRarity TurretRarity;
    public TowerTypes TurretType;
    public float AtkRange;
    private int _baseAtk;

    public bool SetTurretBuildEarly = false;
    private TileBehaviour _tile;

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

        if (PhotonNetwork.IsMasterClient)
        {
            if (_target)
            {
                _spawnTimer += Time.deltaTime;

                if (_spawnTimer >= _bulletSpawnTimer)
                {
                    _spawnTimer -= _bulletSpawnTimer;

                    Vector3 worldPosition = _turretHead.transform.TransformPoint(_firePosition);
                    Vector3 targetPos = _target.transform.position;

                    _photonView.RPC("RPC_SpawnBullet", RpcTarget.All, worldPosition, targetPos);

                    switch (TurretType)
                    {
                        case TowerTypes.RifleTower:
                            SoundManager.Instance.PlaySFX("RifleSound", 0.05f, false);
                            break;
                        case TowerTypes.MachinegunTower:
                            SoundManager.Instance.PlaySFX("MachinegunSound", 0.05f, false);
                            break;
                        case TowerTypes.FlameTower:
                            break;
                        case TowerTypes.MissileTower:
                            SoundManager.Instance.PlaySFX("NormalShootSound", 0.05f, false);
                            break;
                        case TowerTypes.RailgunTower:
                            SoundManager.Instance.PlaySFX("RailgunSound", 0.05f, false);
                            break;
                        case TowerTypes.GravityTower:
                            SoundManager.Instance.PlaySFX("NormalShootSound", 0.05f, false);
                            break;
                        case TowerTypes.GrenadeTower:
                            SoundManager.Instance.PlaySFX("NormalShootSound", 0.05f, false);
                            break;
                        case TowerTypes.ElectricTower:
                            break;
                        case TowerTypes.LaserTower:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    protected virtual void FindTarget()
    {
        if (_target != null)
        {
            
            float distSqr = (_target.transform.position - transform.position).sqrMagnitude;
            if (distSqr > _rangeSqr)
            {
                _target = null;
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

    public void SetMyTile(TileBehaviour tile)
    {
        _tile = tile;
    }

    public void SetMyTileStateInit()
    {
        bool isMaster = PhotonNetwork.IsMasterClient;
        TileAccessType access = isMaster ? TileAccessType.MasterOnly : TileAccessType.ClientOnly;

        TileState originalState = TileState.Installable;

        _tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,(int)originalState, (int)access);

        _tile = null;
    }

    [PunRPC]
    protected virtual void RPC_SpawnBullet(Vector3 firePosition, Vector3 targetPosition)
    {
        Bullet bulletInstance = Instantiate(_bullet, firePosition, Quaternion.identity);
        //bulletInstance.SetBulletTargetPosition(targetPosition); // 타겟이 아닌 위치로
        bulletInstance.SetBulletTarget(_target);
        bulletInstance.SetBullet(MyTurretData.BulletSpeed, MyTurretData.Atk, MyTurretData.HitEffectPath, MyTurretData.Name);

        if (_fireEffectInstance == null)
        {
            _fireEffectInstance = Instantiate(_fireEffectPrefab, firePosition, Quaternion.identity);
            _fireEffectInstance.transform.SetParent(_turretHead.transform);
            _fireEffectInstance.transform.localPosition = Vector3.zero;
            _fireEffectInstance.transform.forward = _turretHead.transform.forward;
        }

        if (!_fireEffectInstance.activeSelf)
        {
            _fireEffectInstance.transform.position = firePosition;
            _fireEffectInstance.transform.forward = _turretHead.transform.forward;
            _fireEffectInstance.SetActive(true);

            StartCoroutine(DisableEffect(_fireEffectInstance, 1.0f));
        }
    }

    private IEnumerator DisableEffect(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effect != null)
            effect.SetActive(false);
    }

    [PunRPC]
    public void OnBuildComplete(TowerTypes type)
    {
        TurretType = type;
        gameObject.SetActive(false);
        InitTurret(type);
    }

    [PunRPC]
    public void ActivateTurret(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    [PunRPC]
    public void SetTurretPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public void UpgradeTurret()
    {
        int maxLevel = 10;
        if (MyTurretData.Level >= maxLevel)
        {
            Debug.Log("최대 레벨에 도달했습니다.");
            return;
        }

        MyTurretData.Level++;

        int upgradeAtk = MyTurretData.Upgrade * MyTurretData.Level;

        float newAtkSpeed = MyTurretData.AtkSpeed + 0.1f;
        _bulletSpawnTimer = 1.0f / newAtkSpeed;

        MyTurretData.Atk = _baseAtk + upgradeAtk;
        MyTurretData.AtkSpeed = newAtkSpeed;
    }
    public void InitTurret(TowerTypes type)
    {
        MyTurretData = DataManager.Instance.GetTurretData(type);
        _bulletSpawnTimer = 1.0f / MyTurretData.AtkSpeed;

        _baseAtk = MyTurretData.Atk;

        GameObject rarityPrefab = Resources.Load<GameObject>("Prefabs/Turrets/RarityCircle");
        GameObject instance = Instantiate(rarityPrefab, transform);
        TurretRarity = instance.GetComponent<TurretRarity>();
        TurretRarity.SetRarityVisual(MyTurretData.Rarity);

        _rangeSqr = MyTurretData.Range * MyTurretData.Range;
        AtkRange = MyTurretData.Range;
        _turretHead.SetTurretType(MyTurretData.Type);

        _bullet = Resources.Load<Bullet>("Prefabs/Bullets/" + MyTurretData.Bullet);
        _fireEffectPrefab = Resources.Load<GameObject>("Prefabs/FireEffects/" + MyTurretData.FireEffectPath);
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
        _turretHead = GetComponentInChildren<TurretHead>();

        //Gizmos.color = Color.cyan;
        //Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(MyTurretData.Range));
        Vector3 worldPosition = _turretHead.transform.TransformPoint(_firePosition);
        //Vector3 targetPos = _target.transform.position;

        Gizmos.DrawWireSphere(worldPosition, 0.1f);

    }

    public void SetStateWhenBuildTurret()
    {
        SetTurretBuildEarly = true; 
        StartCoroutine(DelayedSetStateCoroutine());
    }

    private IEnumerator DelayedSetStateCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        SetTurretBuildEarly = false;
    }
}

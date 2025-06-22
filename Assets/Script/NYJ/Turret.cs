using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    protected Monster _target;
    protected GameObject _fireEffectPrefab;

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
    private TurretRarity _turretRarity;
    public TowerTypes TurretType;
    public float AtkRange;

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
            _spawnTimer += Time.deltaTime;

            if (_target)
            {
                if (_spawnTimer >= _bulletSpawnTimer)
                {
                    _spawnTimer -= _bulletSpawnTimer;

                    Vector3 worldPosition = _turretHead.transform.TransformPoint(_firePosition);
                    Vector3 targetPos = _target.transform.position;

                    _photonView.RPC("RPC_SpawnBullet", RpcTarget.All, worldPosition, targetPos);
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

    [PunRPC]
    protected virtual void RPC_SpawnBullet(Vector3 firePosition, Vector3 targetPosition)
    {
        Bullet bulletInstance = Instantiate(_bullet, firePosition, Quaternion.identity);
        //bulletInstance.SetBulletTargetPosition(targetPosition); // Ÿ���� �ƴ� ��ġ��
        bulletInstance.SetBulletTarget(_target);
        bulletInstance.SetBullet(MyTurretData.BulletSpeed, MyTurretData.Atk, MyTurretData.HitEffectPath);
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
    public void InitTurret(TowerTypes type)
    {
        MyTurretData = DataManager.Instance.GetTurretData(type);
        _bulletSpawnTimer = 1.0f / MyTurretData.AtkSpeed;

        GameObject rarityPrefab = Resources.Load<GameObject>("Prefabs/Turrets/RarityCircle");
        GameObject instance = Instantiate(rarityPrefab, transform);
        _turretRarity = instance.GetComponent<TurretRarity>();
        _turretRarity.SetRarityVisual(MyTurretData.Rarity);

        _rangeSqr = MyTurretData.Range * MyTurretData.Range;
        AtkRange = MyTurretData.Range;
        _turretHead.SetTurretType(MyTurretData.Type);

        _bullet = Resources.Load<Bullet>("Prefabs/Bullets/" + MyTurretData.Bullet);
        _fireEffectPrefab = Resources.Load<GameObject>("Prefabs/FireEffects/" + MyTurretData.FireEffectPath);
    }
    [PunRPC]
    public void OnBuildComplete()
    {
        // ��ġ �� �ð� ȿ��, �ʱ� Ÿ�� ��ĵ, UI ���� �� ó��
        Debug.Log($"{gameObject.name} �ͷ��� ��ġ �Ϸ��");
    }
    protected virtual void OnDrawSelectedGizmos()
    {
        _turretHead = GetComponentInChildren<TurretHead>();
        Vector3 worldPosition = _turretHead.transform.TransformPoint(_firePosition);

        Gizmos.DrawSphere(worldPosition, 0.01f);
    }
    private void OnDrawGizmos() // ���� üũ 
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(MyTurretData.Range));
    }
}

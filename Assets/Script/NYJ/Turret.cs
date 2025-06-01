using UnityEngine;

public class Turret : MonoBehaviour
{
    protected Monster _target;
    protected GameObject _fireEffectPrefab;
    private GameObject _activeFireEffect;

    protected TurretData _turretData = new TurretData();
    protected TurretHead _turretHead;
    protected Bullet _bullet;
    [SerializeField] protected Vector3 _firePosition = Vector3.forward;

    protected float _spawnTimer = 0.0f;
    protected float _bulletSpawnTimer = 1.0f;

    private TurretRarity _turretRarity;

    private void Awake()
    {
        _turretHead = GetComponentInChildren<TurretHead>();
    }

    private void Start()
    {
        string name = gameObject.name.Replace("(Clone)", "").Trim();
        _turretData = DataManager.Instance.GetTurretData(name);

        GameObject rarityPrefab = Resources.Load<GameObject>("Prefabs/Turrets/RarityCircle");
        GameObject instance = Instantiate(rarityPrefab, transform);
        _turretRarity = instance.GetComponent<TurretRarity>();
        _turretRarity.SetRarityVisual(_turretData.Rarity);
       
        _bullet = Resources.Load<Bullet>("Prefabs/Bullets/" + _turretData.Bullet);
        _fireEffectPrefab = Resources.Load<GameObject>("Prefabs/FireEffects/" + _turretData.fireEffectPath);
    }

    protected virtual void Update()
    {
        _spawnTimer += _turretData.AtkSpeed * Time.deltaTime;

        if (_target)
        {
            if (_spawnTimer >= _bulletSpawnTimer)
            {
                _spawnTimer -= _bulletSpawnTimer;
                if(_turretData.Type == "Direct") //직사
                {
                    if(_turretData.Name == "FlameTower")
                    {
                        AttackFlame();
                    }
                    else
                    {
                        AttackTarget(); //직사
                    }
                }
                else
                {
                    AttackTarget(); //곡사
                }
                   
            }
        }
        else
        {
            if (_activeFireEffect != null && _activeFireEffect.activeSelf)
            {
                _activeFireEffect.SetActive(false);
            }
        }
    }

    public void SetTarget(Monster target)
    {
        _target = target;
        _turretHead.SetTarget(target);
    }

    public bool GetTarget()
    {
        return _target != null;
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
        bulletInstance.SetBullet(_turretData.BulletSpeed, _turretData.Atk, _turretData.hitEffectPath);
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

    protected virtual void OnDrawGizmos()
    {
        _turretHead = GetComponentInChildren<TurretHead>();
        Vector3 worldPosition = _turretHead.transform.TransformPoint(_firePosition);

        Gizmos.DrawSphere(worldPosition, 0.01f);
    }
}

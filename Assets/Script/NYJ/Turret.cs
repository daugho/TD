using UnityEngine;

public class Turret : MonoBehaviour
{
    private GameObject _target;
    private TurretData _turretData = new TurretData();
    private TurretHead _turretHead;
    private Bullet _bullet;

    private float _spawnTimer = 0.0f;
    private float _bulletSpawnTimer = 1.0f;

    private void Awake()
    {
        _turretHead = GetComponentInChildren<TurretHead>();
    }

    private void Start()
    {
        string name = gameObject.name.Replace("(Clone)", "").Trim();
        _turretData = DataManager.Instance.GetTurretData(name);

        _bullet = Resources.Load<Bullet>("Prefabs/Bullets/" + _turretData.Bullet);
    }

    private void Update()
    {
        _spawnTimer += _turretData.AtkSpeed * Time.deltaTime;

        if (_target)
        {
            if (_spawnTimer >= _bulletSpawnTimer)
            {
                _spawnTimer -= _bulletSpawnTimer;
                AttackTarget();
            }
        }
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
        _turretHead.SetTarget(target);
    }

    public bool GetTarget()
    {
        return _target;
    }
    private void AttackTarget()
    {
        Bullet bulletInstance = Instantiate(_bullet, transform.position, Quaternion.identity);
        bulletInstance.SetBulletTarget(_target);
        bulletInstance.SetBullet(_turretData.AtkSpeed, _turretData.EffectPath, _turretData.EffectPath);
    }
}

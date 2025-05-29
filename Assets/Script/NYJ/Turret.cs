using UnityEngine;

public class Turret : MonoBehaviour
{
    protected GameObject _target;
    protected TurretData _turretData = new TurretData();
    protected TurretHead _turretHead;
    protected Bullet _bullet;

    protected float _spawnTimer = 0.0f;
    protected float _bulletSpawnTimer = 1.0f;

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
    protected virtual void AttackTarget()
    {
        Bullet bulletInstance = Instantiate(_bullet, transform.position, Quaternion.identity);
        bulletInstance.SetBulletTarget(_target);
        bulletInstance.SetBullet(_turretData.BulletSpeed, _turretData.EffectPath, _turretData.EffectPath);
    }

    private void AttackFlame()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Effects/WFXMR_FlameThrower Big Alt Looped");
        GameObject flame = Instantiate(prefab, transform.position,Quaternion.identity);

    }
}

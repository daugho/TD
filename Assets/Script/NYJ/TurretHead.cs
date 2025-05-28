using UnityEngine;

public class TurretHead : MonoBehaviour
{
    private GameObject _target;
    [SerializeField] private float _rotationSpeed = 5.0f;
    private float _spawnTimer = 0.0f;
    private float _bulletSpawnTimer = 1.0f;

    private Bullet _bulletPrefabs;
    private void Awake()
    {
        _bulletPrefabs = Resources.Load<Bullet>("Prefabs/Bullets/Meteorite_Missile");
    }

    private void Update()
    {
        if (_target == null)
            return;

        Vector3 direction = _target.transform.position - transform.position;
        direction.y = 0f;

        if (direction == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

        _spawnTimer += Time.deltaTime;

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
    }

    public GameObject GetTarget()
    {
        return _target;
    }
    private void AttackTarget()
    {
        Bullet bulletInstance = Instantiate(_bulletPrefabs, transform.position, Quaternion.identity);
        bulletInstance.SetBulletTarget(_target);
    }
}

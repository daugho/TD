using UnityEngine;

public class GrenadeTurret : Turret
{
    [SerializeField] private Vector3[] _grenadeStorages;
    private void Awake()
    {
       
    }

    protected override void Update()
    {
        base.Update();
    }

    //protected override void AttackTarget()
    //{
    //    if (_target == null)
    //        return;
    //
    //    foreach(Vector3 bulletPosition in _grenadeStorages)
    //    {
    //        Bullet bulletInstance = Instantiate(_bullet, transform.position, Quaternion.identity);
    //        bulletInstance.transform.position = bulletPosition;
    //        bulletInstance.SetBulletTarget(_target);
    //        bulletInstance.SetBullet(_turretData.BulletSpeed, _turretData.Atk, _turretData.HitEffectPath);
    //    }
    //}

    protected override void OnDrawSelectedGizmos()
    {
        _turretHead = GetComponentInChildren<TurretHead>(); // 나중에 지우기 
        foreach (Vector3 bulletPosition in _grenadeStorages)
        {
            Vector3 worldPosition = _turretHead.transform.TransformPoint(bulletPosition);

            Gizmos.DrawSphere(worldPosition, 0.01f);
        }
    }
}

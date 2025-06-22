using Photon.Pun;
using UnityEngine;

public class FlameTurret : Turret
{
    private GameObject _activeFlameEffect;
    private float _flameTickTimer = 0f;
    private float _flameTickInterval = 0.2f;
    private PhotonView _targetView;

    [SerializeField] private float _flameAngle = 45f;
    [SerializeField] private float _testLength = 5.0f;

    protected override void Update()
    {
        base.Update();

        if (_target == null)
        {
            if (_activeFlameEffect != null && _activeFlameEffect.activeSelf)
            {
                _activeFlameEffect.SetActive(false);
            }
            return;
        }

        if (_activeFlameEffect.activeSelf)
        {
            _flameTickTimer += Time.deltaTime;
            if (_flameTickTimer >= _flameTickInterval)
            {
                _flameTickTimer -= _flameTickInterval;
                ApplyFlameConeDamage();
            }
        }
    }
    [PunRPC]
    protected override void RPC_SpawnBullet(Vector3 firePosition, Vector3 targetPosition)
    {
        Vector3 worldPosition = _turretHead.transform.TransformPoint(_firePosition);
        _targetView = _target.GetComponent<PhotonView>();

        if (_activeFlameEffect == null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(_turretHead.transform.forward);
            GameObject prefab = Resources.Load<GameObject>("Prefabs/HitEffects/FlameHit");
            _activeFlameEffect = Instantiate(prefab, worldPosition, lookRotation, _turretHead.transform);
        }
        else
        {
            _activeFlameEffect.transform.position = worldPosition;
            _activeFlameEffect.transform.rotation = _turretHead.transform.rotation;

            if (!_activeFlameEffect.activeSelf)
            {
                _activeFlameEffect.SetActive(true);
            }
        }
    }

    private void ApplyFlameConeDamage()
    {
        Vector3 origin = _turretHead.transform.position;
        Vector3 forward = _turretHead.transform.forward;

        Collider[] hits = Physics.OverlapSphere(origin, _testLength, LayerMask.GetMask("Monster"));

        foreach (Collider hit in hits)
        {
            Vector3 toTarget = hit.transform.position - origin;
            float angle = Vector3.Angle(forward, toTarget);

            if (angle <= _flameAngle)
            {
                PhotonView view = hit.GetComponent<PhotonView>();
                if (view != null)
                {
                    view.RPC("TakeDamage", RpcTarget.AllBuffered, MyTurretData.Atk);
                }
            }
        }
    }
}

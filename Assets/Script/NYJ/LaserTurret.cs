using Photon.Pun;
using UnityEngine;
public class LaserTurret : Turret
{
    private LineRenderer _lineRenderer;
    private float _flameTickTimer = 0f;
    private float _flameTickInterval = 0.2f;
    private void Awake()
    {
        _turretHead = GetComponentInChildren<TurretHead>();
        _photonView = GetComponentInChildren<PhotonView>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void Update()
    {
        base.Update();

        if (_target == null)
        {
            _lineRenderer.enabled = false;
            return;
        }

        _lineRenderer.enabled = true;

        Vector3 start = _turretHead.transform.TransformPoint(_firePosition);
        Vector3 end = _target.transform.position;

        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);

        _flameTickTimer += Time.deltaTime;
        if (_flameTickTimer >= _flameTickInterval)
        {
            _flameTickTimer -= _flameTickInterval;
            ApplyLaserDamage();
        }
    }

    [PunRPC]
    protected override void RPC_SpawnBullet(Vector3 firePosition, Vector3 targetPosition)
    {
    }

    private void ApplyLaserDamage()
    {
        PhotonView targetView = _target.GetComponent<PhotonView>();
        if (targetView != null)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            targetView.RPC(nameof(Monster.TakeDamage), RpcTarget.MasterClient, MyTurretData.Atk, actorNumber);
        }
    }
}

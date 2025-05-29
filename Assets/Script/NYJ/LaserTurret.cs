using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LaserTurret : Turret
{
    private LineRenderer _lineRenderer;
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void AttackTarget()
    {
        if (_target == null)
            return;

        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _target.transform.position);
        _lineRenderer.enabled = true;

        //_target.TakeDamage(_turretData.Atk * Time.deltaTime); 

        // 일정 시간 후 레이저 끄기
        //Invoke(nameof(DisableLaser), _laserDuration);
    }

}

using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _moveSpeed = 5.0f;
    protected int _atk; 
    private float _excuteRange = 0.5f;
    protected GameObject _explosionEffect;
    protected Monster _target;
    private float _hitThreshold = 0.1f;
    private PhotonView _targetView;
    private TowerTypes _type;

    protected Vector3 _targetPosition;
    private bool _isUsingTargetTransform = false;
 
    private void Awake()
    {
    }
    protected virtual void Update()
    {
        Vector3 destination;

        if (_isUsingTargetTransform && _target != null)
        {
            _targetPosition = _target.transform.position;
            destination = _targetPosition;
        }
        else
        {
            destination = _targetPosition;
        }

        Vector3 dir = destination - transform.position;
        dir.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = targetRotation;
        transform.position += dir * _moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, destination) <= _excuteRange)
        {
            ExecuteAttack();

            switch (_type)
            {
                case TowerTypes.MissileTower:
                case TowerTypes.GrenadeTower:
                    SoundManager.Instance.PlaySFX("BombHitSound", 0.05f, false);
                    break;
                case TowerTypes.RailgunTower:
                    SoundManager.Instance.PlaySFX("RailgunHitSound", 0.05f, false);
                    break;
            }

            Destroy(gameObject);
        }
    }

    protected virtual void ExecuteAttack()
    {
        if (_targetView != null)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

            _targetView.RPC(nameof(Monster.TakeDamage), RpcTarget.MasterClient, _atk, actorNumber);
        }

        if (_explosionEffect != null)
        {
            Instantiate(_explosionEffect, transform.position, transform.rotation);
        }
    }

    public void SetBullet(float speed, int atk, string hitEffectPath, TowerTypes type)
    {
        _moveSpeed = speed;
        _atk = atk;
        _type = type;
        _explosionEffect = Resources.Load<GameObject>("Prefabs/HitEffects/" + hitEffectPath);

    }
    public void SetBulletTarget(Monster target)
    {
        _target = target;
        _targetView = target.GetComponent<PhotonView>();
        _isUsingTargetTransform = true;
        _targetPosition = target.transform.position;
    }

    public void SetBulletTargetPosition(Vector3 targetPos)
    {
        _target = null;
        _targetView = null;
        _targetPosition = targetPos;
        _isUsingTargetTransform = false;
    }
}

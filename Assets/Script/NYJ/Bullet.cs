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

    private void Awake()
    {
    }
    protected virtual void Update()
    {
        if (!_target)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 dir = _target.transform.position - transform.position;
        dir.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = targetRotation;
        transform.position += dir * _moveSpeed * Time.deltaTime;

        if (Vector3.Distance(_target.transform.position, transform.position) <= _excuteRange)
        {
            ExecuteAttack();
        }
    }

    protected virtual void ExecuteAttack()
    {
        if (_targetView != null)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

            _targetView.RPC(nameof(Monster.TakeDamage), RpcTarget.MasterClient, _atk, actorNumber);
        }


        Destroy(gameObject);
        
        GameObject explosionPrefab = Instantiate<GameObject>(_explosionEffect, transform.position, transform.rotation);
    }

    public void SetBullet(float speed, int atk, string hitEffectPath)
    {
        _moveSpeed = speed;
        _atk = atk;
        _explosionEffect = Resources.Load<GameObject>("Prefabs/HitEffects/" + hitEffectPath);
    }
    public void SetBulletTarget(Monster target)
    {
        _target = target;
        _targetView = target.GetComponent<PhotonView>();
    }
}

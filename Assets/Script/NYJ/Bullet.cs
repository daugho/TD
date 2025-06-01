using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _moveSpeed = 5.0f;
    private float _atk; 
    private float _excuteRange = 0.5f;
    private GameObject _explosionEffect;
    private Monster _target;
    private float _hitThreshold = 0.1f;

    private void Awake()
    {
    }
    private void Update()
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

    private void ExecuteAttack()
    {
        Destroy(gameObject);
        
        _target.GetDamaged(5); 
        GameObject explosionPrefab = Instantiate<GameObject>(_explosionEffect, transform.position, transform.rotation);
    }

    public void SetBullet(float speed, float atk, string hitEffectPath)
    {
        _moveSpeed = speed;
        _atk = atk;
        _explosionEffect = Resources.Load<GameObject>("Prefabs/HitEffects/" + hitEffectPath);
    }
    public void SetBulletTarget(Monster target)
    {
        _target = target;
    }
}

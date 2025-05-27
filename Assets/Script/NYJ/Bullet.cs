using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _moveSpeed = 5.0f;
    private float _excuteRange = 0.5f;
    private GameObject _explosionEffect;
    private GameObject _gunFireEffect;
    [SerializeField] private GameObject _target;

    private void Awake()
    {
        _explosionEffect = Resources.Load<GameObject>("Prefabs/Effects/WFX_ExplosiveSmoke Big");
        _gunFireEffect = Resources.Load<GameObject>("Prefabs/Effects/WFX_Explosion");
    }
    private void Update()
    {
        Vector3 dir = _target.transform.position - transform.position;
        dir.Normalize();

        transform.position += dir * _moveSpeed * Time.deltaTime;

        ExecuteAttack();
    }

    private void ExecuteAttack()
    {
        if(Vector3.Distance(_target.transform.position, transform.position) <= _excuteRange)
        {
            Destroy(gameObject);
            GameObject explosionPrefab = Instantiate<GameObject>(_explosionEffect, transform.position, transform.rotation);
        }
    }

}

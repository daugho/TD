using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GravityBullet : Bullet
{
    protected override void Update()
    {
        base.Update();
    }
    protected override void ExecuteAttack()
    {
        Destroy(gameObject);

        GameObject explosionPrefab = Instantiate<GameObject>(_explosionEffect, transform.position, transform.rotation);
        DebuffSystem debuff = explosionPrefab.GetComponent<DebuffSystem>();
        debuff.DebuffSlow(transform, _atk);
    }
}

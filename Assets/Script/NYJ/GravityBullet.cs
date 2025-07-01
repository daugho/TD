using UnityEngine;
public class GravityBullet : Bullet
{
    protected override void Update()
    {
        base.Update();
    }
    protected override void ExecuteAttack()
    {
        Destroy(gameObject);

        GameObject explosionPrefab = Instantiate<GameObject>(_explosionEffect, transform.position, Quaternion.identity);
        DebuffSystem debuff = explosionPrefab.GetComponent<DebuffSystem>();
        debuff.DebuffSlow(transform, _atk);
    }
}

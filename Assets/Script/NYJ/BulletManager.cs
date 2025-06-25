using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private Transform _spawnPlayerPos;
    private Transform _spawnEnemyPos;

    private Transform _spawnContent;


    private GameObject _spawnBtnPrefab;

    [SerializeField] private float _enemySpawnInterval = 5.0f;

    private int _bulletPoolSize = 50;

    private void Awake()
    {
        GameObject _bulletPrefab = Resources.Load<GameObject>("Prefabs/Cat");

        PoolingManager.Instance.CreatePool("Bullet", _bulletPrefab, _bulletPoolSize);
    }

    private void OnSpawnBullet(int key, bool isPlayerBase)
    {
        GameObject obj = PoolingManager.Instance.Pop("Bullet");

        Transform spawnPos = isPlayerBase ? _spawnPlayerPos : _spawnEnemyPos;

        Bullet bullet = obj.GetComponent<Bullet>();

        if (bullet != null)
        {
            //bullet.SetBullet();
        }

    }
    public void ReleaseBullet(GameObject bullet)
    {
        //bullet.GetComponent<Bullet>().ResetBullet();
        PoolingManager.Instance.Release("Bullet", bullet);
    }
}


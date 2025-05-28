using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private EnemyPath _enemyPath;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private Turret[] _turrets;

    private GameObject _enemyPrefab;
    private float _timer = 0f;
    private void Awake()
    {
        _enemyPrefab = Resources.Load<GameObject>("Prefabs/Monsters/Monster");
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _spawnInterval)
        {
            _timer -= _spawnInterval;
            EnemySpawn();
        }
    }
    private void EnemySpawn()
    {
        if (_enemyPrefab == null || _enemyPath == null) return;

        GameObject enemy = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        Monster enemyScript = enemy.GetComponent<Monster>();

        if (enemyScript != null)
        {
            enemyScript.Init(_enemyPath, 3.0f);

            if (!_turrets[0].GetTarget())
            {
                //Test
                _turrets[0].SetTarget(enemy);
                _turrets[1].SetTarget(enemy);
                _turrets[2].SetTarget(enemy);
                _turrets[3].SetTarget(enemy);
                _turrets[4].SetTarget(enemy);
                _turrets[5].SetTarget(enemy);
                _turrets[6].SetTarget(enemy);
                _turrets[7].SetTarget(enemy);
                _turrets[8].SetTarget(enemy);
            }
        }
    }
}

using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private EnemyPath _enemyPath;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private Turret[] _turrets;
    [SerializeField] private Transform _monsterGuiCanvas;
    private HPBar _hpBar;


    private Monster _enemyPrefab;
    private float _timer = 0f;
    private void Awake()
    {
        _enemyPrefab = Resources.Load<Monster>("Prefabs/Monsters/Monster");
        _hpBar = Resources.Load<HPBar>("Prefabs/Monsters/HPBar");
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

        HPBar hpBar = Instantiate(_hpBar, _monsterGuiCanvas);

        Monster enemy = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        Monster enemyScript = enemy.GetComponent<Monster>();

        if (enemyScript != null)
        {
            enemyScript.Init(_enemyPath, 3.0f, hpBar);

            for(int i = 0; i < _turrets.Length; i++)    
            {
                if (!_turrets[i].GetTarget())
                    _turrets[i].SetTarget(enemy);
            }
        }
    }
}

using System.Collections;
using UnityEditor.Overlays;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private EnemyPath _enemyPath;
    [SerializeField] private Turret[] _turrets;
    [SerializeField] private Transform _monsterGuiCanvas;
    
    private HPBar _hpBar;
    private int _stage = 1;
    private int _wave = 1;


    private void Awake()
    {
        _hpBar = Resources.Load<HPBar>("Prefabs/Monsters/HPBar");
    }

    private void Start()
    {
        RoundData roundData = DataManager.Instance.GetRoundData((_stage, _wave));

        StartCoroutine(SpawnWaveCoroutine(roundData));
    }

    private void Update()
    {

    }

    private IEnumerator SpawnWaveCoroutine(RoundData roundData)
    {
        foreach (MonsterSpawnInfo monster in roundData.Monsters)
        {
            for (int i = 0; i < monster.Count; i++)
            {
                EnemySpawn(monster.Type);
                yield return new WaitForSeconds(0.2f); 
            }
        }
    }
    private void EnemySpawn(DroneTypes type)
    {
        if (_enemyPath == null) return;
        
        RoundData roundData = DataManager.Instance.GetRoundData((_stage, _wave));

        MonsterData monsterData = DataManager.Instance.GetMonsterData((int)type);
        Monster monsterPrefab = Resources.Load<Monster>("Prefabs/Monsters/" + monsterData.PrefabPath); 
        
        HPBar hpBar = Instantiate(_hpBar, _monsterGuiCanvas);

        Monster enemy = Instantiate(monsterPrefab, transform.position, Quaternion.identity);

        if (enemy != null)
        {
            enemy.Init(monsterData, _enemyPath, roundData.SpeedMultiplier, 
                roundData.HpMultiplier, hpBar); // 나중에 _hpBar 풀링 

            for(int i = 0; i < _turrets.Length; i++)    
            {
                if (!_turrets[i].GetTarget())
                    _turrets[i].SetTarget(enemy);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using Photon.Pun;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private Turret[] _turrets;

    public static List<Monster> Monsters = new List<Monster>();

    private HPBar _hpBar;
    private int _stage = 1;
    private int _wave = 2;


    private void Awake()
    {
        _hpBar = Resources.Load<HPBar>("Prefabs/Monsters/HPBar");
    }


    public void OnClickSpawn()
    {
        RoundData roundData = DataManager.Instance.GetRoundData((_stage, _wave));

        StartCoroutine(SpawnWaveCoroutine(roundData));
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
        RoundData roundData = DataManager.Instance.GetRoundData((_stage, _wave));

        MonsterData monsterData = DataManager.Instance.GetMonsterData((int)type);

        object[] instData = new object[]
       {
            (int)type,
            roundData.SpeedMultiplier,
            roundData.HpMultiplier
       };

        GameObject enemy = PhotonNetwork.Instantiate("Prefabs/Monsters/" + monsterData.PrefabPath, 
            transform.position, Quaternion.identity, 0, instData);
    }
}

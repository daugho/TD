using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private PathManager _pathManager;

    public void SpawnEnemy()
    {
        var path = _pathManager.CalculatePath();
        if (path == null || path.Count < 2) return;

        Vector3 spawnPosition = path[0].transform.position + Vector3.up * 0.5f;
        GameObject enemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
        //GameObject enemy = Instantiate(_enemyPrefab, path[0].transform.position, Quaternion.identity);

        var mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.SetPath(path);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private TileContext _tileContext;
    [SerializeField] private GameObject _enemyPrefab;

    public void SpawnEnemy()
    {
        TileBehaviour startTile = null;
        TileBehaviour endTile = null;
        List<TileBehaviour> pathTiles = new();

        foreach (Transform child in _tileContext.TileParent)
        {
            var tile = child.GetComponent<TileBehaviour>();
            if (tile == null) continue;

            switch (tile._tileState)
            {
                case TileState.StartPoint:
                    startTile = tile;
                    break;
                case TileState.EndPoint:
                    endTile = tile;
                    break;
                case TileState.Installable: // ? 경로가 될 타일
                    pathTiles.Add(tile);
                    break;
            }
        }

        if (startTile == null || endTile == null)
        {
            Debug.LogWarning("StartPoint 또는 EndPoint가 설정되지 않았습니다.");
            return;
        }

        pathTiles = pathTiles
            .OrderBy(t => Vector3.Distance(startTile.transform.position, t.transform.position))
            .ToList();

        pathTiles.Add(endTile);

        GameObject enemy = Instantiate(_enemyPrefab, startTile.transform.position, Quaternion.identity);
        var mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.SetPath(pathTiles);
        }
    }

    private IEnumerator MoveAlongPath(GameObject enemy, List<TileBehaviour> pathTiles)
    {
        foreach (var tile in pathTiles)
        {
            Vector3 target = tile.transform.position;
            while (Vector3.Distance(enemy.transform.position, target) > 0.1f)
            {
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, target, Time.deltaTime * 2f);
                yield return null;
            }
        }

        Debug.Log("적이 목표에 도착했습니다.");
    }
}

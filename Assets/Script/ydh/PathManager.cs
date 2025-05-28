using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PathManager : MonoBehaviour
{
    [SerializeField] private TileContext _tileContext;

    public List<TileBehaviour> CalculatePath()
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
                case TileState.Installable://경로가 될 타일
                    pathTiles.Add(tile);
                    break;
            }
        }

        if (startTile == null || endTile == null)
        {
            Debug.LogWarning("StartPoint 또는 EndPoint가 설정되지 않았습니다.");
            return null;
        }

        pathTiles = pathTiles
            .OrderBy(t => Vector3.Distance(startTile.transform.position, t.transform.position))// 시작점에서 가까운 순서로 정렬
            .ToList();

        pathTiles.Insert(0, startTile);
        pathTiles.Add(endTile);
        return pathTiles;
    }
}

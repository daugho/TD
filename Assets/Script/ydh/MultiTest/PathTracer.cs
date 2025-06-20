using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTracer : MonoBehaviour
{
    private TileBehaviour[,] tiles;
    private int width, height;
    private Coroutine moveCoroutine;
    private GridManager gridManager;
    private TrailRenderer trail;
    private Vector3? startWorldPosition = null;
    public OwnerRole Role { get; private set; }

    private void Awake()
    {
        gridManager = GameObject.FindFirstObjectByType<GridManager>();
        trail = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        TileBehaviour.OnAnyTileChanged += HandleTileChanged;
    }

    private void OnDisable()
    {
        TileBehaviour.OnAnyTileChanged -= HandleTileChanged;
    }

    private void HandleTileChanged()
    {
        StopCurrentMovement();

        if (startWorldPosition.HasValue)
        {
            transform.position = startWorldPosition.Value;
        }

        MoveByPathfindingLoop();
    }

    public void Initialize(OwnerRole role)
    {
        Role = role;
    }

    private void StopCurrentMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    public void MoveByPathfindingLoop()
    {
        StopCurrentMovement();
        MoveFromStartPoint();
    }

    private void MoveFromStartPoint()
    {
        width = gridManager.GetWidth();
        height = gridManager.GetHeight();
        tiles = gridManager.GetTileArray();

        Vector2Int? start = FindMyStartPoint();
        Vector2Int? end = FindEndPoint();

        if (start == null || end == null)
        {
            Debug.LogWarning("[PathTracer] 시작 또는 끝 타일을 찾을 수 없습니다.");
            return;
        }

        AStarPathfinder pathfinder = new AStarPathfinder(tiles, width, height);
        List<Vector2Int> path = pathfinder.FindPath(start.Value, end.Value);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[PathTracer] A* 경로를 찾지 못했습니다.");
            return;
        }

        startWorldPosition = new Vector3(start.Value.x + 0.5f, 0.5f, start.Value.y + 0.5f);
        moveCoroutine = StartCoroutine(FollowPath(path));
    }

    private IEnumerator FollowPath(List<Vector2Int> path)
    {
        List<Vector3> worldPath = new();
        foreach (var tilePos in path)
        {
            worldPath.Add(new Vector3(tilePos.x + 0.5f, 0.5f, tilePos.y + 0.5f));
        }

        //trail.enabled = true;

        foreach (var targetPos in worldPath)
        {
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 5f * Time.deltaTime);
                yield return null;
            }
        }

        yield return new WaitForSeconds(3f);
        //trail.Clear();
        //trail.enabled = false;

        // 도착 후 시작점으로 순간이동
        if (startWorldPosition.HasValue)
        {
            transform.position = startWorldPosition.Value;
        }
        else
        {
            Debug.LogWarning("[PathTracer] 기록된 시작 위치가 없어 복귀하지 못함.");
            yield break;
        }

        yield return new WaitForSeconds(0.5f);
        MoveByPathfindingLoop();
    }

    private Vector2Int? FindMyStartPoint()
    {
        bool isMaster = PhotonNetwork.IsMasterClient;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var tile = tiles[x, z];
                if (tile == null || tile._tileState != TileState.StartPoint)
                    continue;

                if (tile._accessType == TileAccessType.Everyone ||
                    (tile._accessType == TileAccessType.MasterOnly && isMaster) ||
                    (tile._accessType == TileAccessType.ClientOnly && !isMaster))
                {
                    return new Vector2Int(x, z);
                }
            }
        }

        return null;
    }

    private Vector2Int? FindEndPoint()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var tile = tiles[x, z];
                if (tile != null && tile._tileState == TileState.EndPoint)
                    return new Vector2Int(x, z);
            }
        }
        return null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OwnerRole
{
    Master,
    Client
}

public class MonsterMover : MonoBehaviour
{
    public OwnerRole Owner { get; private set; }

    private TileContext tileContext;
    private GridManager gridManager;
    private TileBehaviour[,] tiles;
    private int width, height;
    private Coroutine moveCoroutine;

    private void Awake()
    {
        tileContext = GameObject.Find("GridSystem").GetComponent<TileContext>();
        gridManager = GameObject.FindFirstObjectByType<GridManager>();
    }

    private void OnEnable()
    {
        TileBehaviour.OnAnyTileChanged += HandleTileChanged;
    }

    private void OnDisable()
    {
        TileBehaviour.OnAnyTileChanged -= HandleTileChanged;
    }

    public void Initialize(OwnerRole owner)
    {
        Owner = owner;
    }

    private void HandleTileChanged()
    {
        StopCurrentMovement();
        MoveByPathfinding();
    }

    public void MoveByPathfinding()
    {
        width = gridManager.GetWidth();
        height = gridManager.GetHeight();
        tiles = gridManager.GetTileArray();

        // 현재 위치 → 시작점으로 사용
        Vector3 pos = transform.position;
        int startX = Mathf.FloorToInt(pos.x);
        int startZ = Mathf.FloorToInt(pos.z);
        Vector2Int start = new(startX, startZ);

        // 도착 타일 탐색
        Vector2Int? end = null;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (tiles[x, z] != null && tiles[x, z]._tileState == TileState.EndPoint)
                {
                    end = new Vector2Int(x, z);
                    break;
                }
            }
            if (end.HasValue) break;
        }

        if (end == null)
        {
            Debug.LogWarning("[MonsterMover] 도착 타일을 찾지 못했습니다.");
            return;
        }

        // 경로 탐색
        AStarPathfinder pathfinder = new AStarPathfinder(tiles, width, height);
        List<Vector2Int> path = pathfinder.FindPath(start, end.Value);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[MonsterMover] 경로를 찾지 못했습니다.");
            return;
        }

        // 경로를 월드 좌표로 변환
        List<Vector3> worldPath = new();
        foreach (var tilePos in path)
        {
            worldPath.Add(new Vector3(tilePos.x + 0.5f, 0.5f, tilePos.y + 0.5f));
        }

        // 이동 시작
        moveCoroutine = StartCoroutine(FollowPath(worldPath));
    }

    private IEnumerator FollowPath(List<Vector3> path)
    {
        foreach (var targetPos in path)
        {
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 2f * Time.deltaTime);
                yield return null;
            }
        }
    }

    private void StopCurrentMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public enum OwnerRole
//{
//    Master,
//    Client
//}

//public class MonsterMover : MonoBehaviour
//{
//    public OwnerRole Owner { get; private set; }
//    private TileContext tileContext;
//    private TileBehaviour[,] tiles;
//    private int width, height;
//    private Coroutine moveCoroutine;


//    private void Awake()
//    {
//        tileContext = GameObject.Find("GridSystem").GetComponent<TileContext>();
//    }

//    private void OnEnable()
//    {
//        TileBehaviour.OnAnyTileChanged += HandleTileChanged;
//    }

//    private void OnDisable()
//    {
//        TileBehaviour.OnAnyTileChanged -= HandleTileChanged;
//    }
//    public void Initialize(OwnerRole owner)
//    {
//        Owner = owner;
//    }

//    private void HandleTileChanged()
//    {
//        StopCurrentMovement();
//        MoveByPathfinding();
//    }

//    public void MoveByPathfinding()
//    {
//        width = tileContext.Width;
//        height = tileContext.Height;
//        tiles = new TileBehaviour[width, height];

//        // 현재 위치 → 시작점으로 사용
//        Vector3 pos = transform.position;
//        int startX = Mathf.FloorToInt(pos.x);
//        int startZ = Mathf.FloorToInt(pos.z);
//        Vector2Int start = new(startX, startZ);

//        foreach (Transform tile in tileContext.TileParent)
//        {
//            var tileBehaviour = tile.GetComponent<TileBehaviour>();
//            if (tileBehaviour == null) continue;
//            Debug.LogWarning($"[x] : {width},[z] : {height}");
//            string[] parts = tile.name.Split('_');
//            int x = int.Parse(parts[1]);
//            int z = int.Parse(parts[2]);

//            tiles[x, z] = tileBehaviour;
//        }


//        // 도착 타일 탐색
//        Vector2Int? end = null;
//        for (int x = 0; x < width; x++)
//        {
//            for (int z = 0; z < height; z++)
//            {
//                if (tiles[x, z]._tileState == TileState.EndPoint)
//                {
//                    end = new Vector2Int(x, z);
//                    break;
//                }
//            }
//            if (end != null) break;
//        }

//        if (end == null)
//        {
//            Debug.LogWarning("[MonsterMover] 도착 타일을 찾지 못했습니다.");
//            return;
//        }

//        // 경로 탐색
//        AStarPathfinder pathfinder = new AStarPathfinder(tiles, width, height);
//        List<Vector2Int> path = pathfinder.FindPath(start, end.Value);

//        if (path == null)
//        {
//            Debug.LogWarning("[MonsterMover] 경로를 찾지 못했습니다.");
//            return;
//        }

//        // 경로를 월드 좌표로 변환
//        List<Vector3> worldPath = new();
//        foreach (var tilePos in path)
//        {
//            Vector3 world = new Vector3(tilePos.x + 0.5f, 0.5f, tilePos.y + 0.5f);
//            worldPath.Add(world);
//        }

//        // 이동 시작
//        moveCoroutine = StartCoroutine(FollowPath(worldPath));
//    }

//    private IEnumerator FollowPath(List<Vector3> path)
//    {
//        foreach (var targetPos in path)
//        {
//            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
//            {
//                transform.position = Vector3.MoveTowards(transform.position, targetPos, 2f * Time.deltaTime);
//                yield return null;
//            }
//        }
//    }

//    private void StopCurrentMovement()
//    {
//        if (moveCoroutine != null)
//        {
//            StopCoroutine(moveCoroutine);
//            moveCoroutine = null;
//        }
//    }
//}

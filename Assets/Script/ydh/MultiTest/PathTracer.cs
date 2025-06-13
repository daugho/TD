using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
            Debug.LogWarning("[PathTracer] ���� �Ǵ� �� Ÿ���� ã�� �� �����ϴ�.");
            return;
        }

        AStarPathfinder pathfinder = new AStarPathfinder(tiles, width, height);
        List<Vector2Int> path = pathfinder.FindPath(start.Value, end.Value);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[PathTracer] A* ��θ� ã�� ���߽��ϴ�.");
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

        trail.enabled = true;

        foreach (var targetPos in worldPath)
        {
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 5f * Time.deltaTime);
                yield return null;
            }
        }

        yield return new WaitForSeconds(3f);
        trail.Clear();
        trail.enabled = false;

        // ���� �� ���������� �����̵�
        if (startWorldPosition.HasValue)
        {
            transform.position = startWorldPosition.Value;
        }
        else
        {
            Debug.LogWarning("[PathTracer] ��ϵ� ���� ��ġ�� ���� �������� ����.");
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



//using Photon.Pun;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using UnityEngine;

//public class PathTracer : MonoBehaviour
//{
//    private TileBehaviour[,] tiles;
//    private int width, height;
//    private Coroutine moveCoroutine;
//    private GridManager gridManager;
//    public OwnerRole Role { get; private set; }

//    private void Awake()
//    {
//        gridManager = GameObject.FindFirstObjectByType<GridManager>();
//    }

//    private void OnEnable()
//    {
//        TileBehaviour.OnAnyTileChanged += HandleTileChanged;
//    }

//    private void OnDisable()
//    {
//        TileBehaviour.OnAnyTileChanged -= HandleTileChanged;
//    }

//    private void HandleTileChanged()
//    {
//        StopCurrentMovement();
//        MoveFromStartPoint();
//    }
//    public void Initialize(OwnerRole role)
//    {
//        Role = role;
//    }

//    private void StopCurrentMovement()
//    {
//        if (moveCoroutine != null)
//        {
//            StopCoroutine(moveCoroutine);
//            moveCoroutine = null;
//        }
//    }
//    public void MoveByPathfindingLoop()
//    {
//        StopCurrentMovement();
//        MoveFromStartPoint();
//    }

//    private void MoveFromStartPoint()
//    {
//        width = gridManager.GetWidth();
//        height = gridManager.GetHeight();
//        tiles = gridManager.GetTileArray();

//        Vector2Int? start = FindMyStartPoint();
//        Vector2Int? end = FindEndPoint();

//        if (start == null || end == null)
//        {
//            Debug.LogWarning("[PathTracer] ���� �Ǵ� �� Ÿ���� ã�� �� �����ϴ�.");
//            return;
//        }

//        transform.position = new Vector3(start.Value.x + 0.5f, 0.5f, start.Value.y + 0.5f);

//        AStarPathfinder pathfinder = new AStarPathfinder(tiles, width, height);
//        List<Vector2Int> path = pathfinder.FindPath(start.Value, end.Value);

//        if (path == null || path.Count == 0)
//        {
//            Debug.LogWarning("[PathTracer] A* ��θ� ã�� ���߽��ϴ�.");
//            return;
//        }

//        List<Vector3> worldPath = new();
//        foreach (var tilePos in path)
//        {
//            worldPath.Add(new Vector3(tilePos.x + 0.5f, 0.5f, tilePos.y + 0.5f));
//        }

//        moveCoroutine = StartCoroutine(FollowPath(worldPath, start.Value));
//    }

//    private IEnumerator FollowPath(List<Vector3> path, Vector2Int startPoint)
//    {
//        foreach (var targetPos in path)
//        {
//            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
//            {
//                transform.position = Vector3.MoveTowards(transform.position, targetPos, 2f * Time.deltaTime);
//                yield return null;
//            }
//        }

//        // ���� �� �ٽ� ���������� �̵�
//        yield return new WaitForSeconds(0.5f);

//        List<Vector3> returnPath = new();
//        for (int i = path.Count - 2; i >= 0; i--) // ������ ������ �����ϰ� ������ ��� ����
//        {
//            returnPath.Add(path[i]);
//        }

//        foreach (var targetPos in returnPath)
//        {
//            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
//            {
//                transform.position = Vector3.MoveTowards(transform.position, targetPos, 2f * Time.deltaTime);
//                yield return null;
//            }
//        }

//        yield return new WaitForSeconds(0.5f);
//        MoveFromStartPoint(); // �ݺ� ��ȯ
//    }

//    private Vector2Int? FindMyStartPoint()
//    {
//        bool isMaster = PhotonNetwork.IsMasterClient;

//        for (int x = 0; x < width; x++)
//        {
//            for (int z = 0; z < height; z++)
//            {
//                var tile = tiles[x, z];
//                if (tile == null || tile._tileState != TileState.StartPoint)
//                    continue;

//                if (tile._accessType == TileAccessType.Everyone ||
//                    (tile._accessType == TileAccessType.MasterOnly && isMaster) ||
//                    (tile._accessType == TileAccessType.ClientOnly && !isMaster))
//                {
//                    return new Vector2Int(x, z);
//                }
//            }
//        }

//        return null;
//    }

//    private Vector2Int? FindEndPoint()
//    {
//        for (int x = 0; x < width; x++)
//        {
//            for (int z = 0; z < height; z++)
//            {
//                var tile = tiles[x, z];
//                if (tile != null && tile._tileState == TileState.EndPoint)
//                    return new Vector2Int(x, z);
//            }
//        }
//        return null;
//    }
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PathTracer : MonoBehaviour
//{
//    public OwnerRole Owner { get; private set; }
//    private GridManager gridManager;
//    private TileBehaviour[,] tiles;
//    private int width, height;
//    private Coroutine moveCoroutine;
//    private bool goingToEnd = true;
//    private TrailRenderer trail;
//    private Vector3? startWorldPosition = null;
//    private Vector3? endWorldPosition = null;
//    private void Awake()
//    {
//        gridManager = GameObject.FindFirstObjectByType<GridManager>();
//        trail = GetComponent<TrailRenderer>();
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
//        // ���� �̵� �ߴ� �� ��� �ٽ� ���
//        if (moveCoroutine != null)
//        {
//            StopCoroutine(moveCoroutine);
//            moveCoroutine = null;
//        }
//        trail?.Clear();
//        MoveByPathfindingLoop();
//    }

//    public void MoveByPathfindingLoop()
//    {
//        width = gridManager.GetWidth();
//        height = gridManager.GetHeight();
//        tiles = gridManager.GetTileArray();

//        Vector3 pos = transform.position;
//        Vector2Int current = new(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
//        Vector2Int? target = null;

//        for (int x = 0; x < width; x++)
//        {
//            for (int z = 0; z < height; z++)
//            {
//                if (tiles[x, z] != null && tiles[x, z]._tileState == TileState.EndPoint)
//                {
//                    target = new Vector2Int(x, z);
//                    break;
//                }
//            }
//            if (target.HasValue) break;
//        }

//        if (!target.HasValue)
//        {
//            Debug.LogWarning("[PathTracer] EndPoint�� ã�� ���߽��ϴ�.");
//            return;
//        }

//        //��� ��� ��� �б�
//        List<Vector3> worldPath = new();

//        if (goingToEnd)
//        {
//            var pathfinder = new AStarPathfinder(tiles, width, height);
//            var path = pathfinder.FindPath(current, target.Value);

//            if (path == null || path.Count == 0)
//            {
//                Debug.LogWarning("[PathTracer] A* ��θ� ã�� ���߽��ϴ�.");
//                return;
//            }

//            foreach (var tilePos in path)
//                worldPath.Add(new Vector3(tilePos.x + 0.5f, 0.5f, tilePos.y + 0.5f));

//            // ? �����, ������ ���
//            startWorldPosition = new Vector3(current.x + 0.5f, 0.5f, current.y + 0.5f);
//            endWorldPosition = new Vector3(target.Value.x + 0.5f, 0.5f, target.Value.y + 0.5f);
//        }
//        else
//        {
//            if (startWorldPosition.HasValue)
//            {
//                worldPath.Add(startWorldPosition.Value); //  �׻� ��ϵ� ��������� ���ư���
//            }
//            else
//            {
//                Debug.LogWarning("[PathTracer] ��ϵ� ������� ���� �������� ����.");
//                return;
//            }
//        }

//        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
//        moveCoroutine = StartCoroutine(FollowPathLoop(worldPath));
//    }

//    private IEnumerator FollowPathLoop(List<Vector3> path)
//    {
//        trail.enabled = goingToEnd;

//        foreach (var pos in path)
//        {
//            while (Vector3.Distance(transform.position, pos) > 0.05f)
//            {
//                transform.position = Vector3.MoveTowards(transform.position, pos, 5f * Time.deltaTime);
//                yield return null;
//            }
//        }

//        if (goingToEnd)
//        {
//            // ? ���� �� 3�� ���, �� �� ������ ����
//            yield return new WaitForSeconds(3f);
//            trail.Clear();
//        }

//        goingToEnd = !goingToEnd;
//        yield return new WaitForSeconds(0.5f);
//        MoveByPathfindingLoop();
//    }
//}

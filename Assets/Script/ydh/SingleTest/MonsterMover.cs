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
    private float _moveSpeed;

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
    
    public void SetMonsterSpeed(float speed)
    {
        _moveSpeed = speed; 
    }    

    private void HandleTileChanged()
    {
        StopCurrentMovement();
        MoveByPathfinding();
    }
    private void StopCurrentMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }
    public void MoveByPathfinding()
    {
        width = gridManager.GetWidth();
        height = gridManager.GetHeight();
        tiles = gridManager.GetTileArray();

        // ���� ��ġ �� ���������� ���
        Vector3 pos = transform.position;
        int startX = Mathf.FloorToInt(pos.x);
        int startZ = Mathf.FloorToInt(pos.z);
        Vector2Int start = new(startX, startZ);

        // ���� Ÿ�� Ž��
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
            Debug.LogWarning("[MonsterMover] ���� Ÿ���� ã�� ���߽��ϴ�.");
            return;
        }

        // ��� Ž��
        AStarPathfinder pathfinder = new AStarPathfinder(tiles, width, height);
        List<Vector2Int> path = pathfinder.FindPath(start, end.Value);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[MonsterMover] ��θ� ã�� ���߽��ϴ�.");
            return;
        }

        // ��θ� ���� ��ǥ�� ��ȯ
        List<Vector3> worldPath = new();
        foreach (var tilePos in path)
        {
            worldPath.Add(new Vector3(tilePos.x + 0.5f, 0.5f, tilePos.y + 0.5f));
        }

        // �̵� ����
        moveCoroutine = StartCoroutine(FollowPath(worldPath));
    }

    private IEnumerator FollowPath(List<Vector3> path)
    {
        foreach (var targetPos in path)
        {
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                Vector3 direction = (targetPos - transform.position).normalized;
                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(direction);

                transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);

                yield return null;
            }
        }

        moveCoroutine = null;
    }
}
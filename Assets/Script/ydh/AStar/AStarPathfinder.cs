using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder
{
    private readonly TileBehaviour[,] tiles;
    private readonly int width, height;

    public AStarPathfinder(TileBehaviour[,] tiles, int width, int height)
    {
        this.tiles = tiles;
        this.width = width;
        this.height = height;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        Debug.Log($"[A*] Start: {start}, End: {end}");
        Debug.Log($"[A*] 타일배열 크기: {width} x {height}");

        HashSet<Vector2Int> closedSet = new();
        PriorityQueue<Node> openSet = new();
        Dictionary<Vector2Int, Node> allNodes = new();

        // 시작 타일 상태 확인
        TileState startState = tiles[start.x, start.y]._tileState;
        TileState endState = tiles[end.x, end.y]._tileState;
        Debug.Log($"[A*] StartTile 상태: {startState}, EndTile 상태: {endState}");

        // 시작 타일이 비정상 상태일 경우 경로 생성하지 않음
        if (startState != TileState.Installable && startState != TileState.StartPoint)
        {
            Debug.LogWarning("[A*] 시작 타일이 Installable 또는 StartPoint가 아닙니다.");
            return null;
        }

        Node startNode = new Node(start, 0f, Heuristic(start, end), null);
        openSet.Enqueue(startNode);
        allNodes[start] = startNode;

        Debug.Log($"[A*] OpenSet 초기 Count: {openSet.Count}");

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();

            if (current.Position == end)
                return ReconstructPath(current);

            closedSet.Add(current.Position);

            foreach (var dir in GetDirections())
            {
                Vector2Int neighborPos = current.Position + dir;
                if (!IsValid(neighborPos)) continue;
                if (closedSet.Contains(neighborPos)) continue;

                TileState state = tiles[neighborPos.x, neighborPos.y]._tileState;

                // 경로로 사용 가능한 상태인지 확인 (Installable 또는 Start/End 포함)
                if (state != TileState.Installable &&
                    state != TileState.StartPoint &&
                    state != TileState.EndPoint)
                    continue;

                float g = current.G + ((dir.x == 0 || dir.y == 0) ? 1f : 1.4142f);
                float h = Heuristic(neighborPos, end);
                float f = g + h;

                if (allNodes.TryGetValue(neighborPos, out Node existing) && existing.F <= f)
                    continue;

                Node neighbor = new Node(neighborPos, g, h, current);
                allNodes[neighborPos] = neighbor;
                openSet.Enqueue(neighbor);
            }
        }

        Debug.LogWarning("[A*] 유효한 경로를 찾지 못했습니다.");
        return null; // 경로 없음
    }


    private static float Heuristic(Vector2Int a, Vector2Int b) => Vector2Int.Distance(a, b);

    private static List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> path = new();
        while (node != null)
        {
            path.Add(node.Position);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }

    private static Vector2Int[] GetDirections() => new Vector2Int[]
    {
        new(0, 1),   // 위
        new(1, 0),   // 오른쪽
        new(0, -1),  // 아래
        new(-1, 0),  // 왼쪽
        //new(0, 1), new(1, 0), new(0, -1), new(-1, 0),
        //new(1, 1), new(-1, 1), new(1, -1), new(-1, -1)
    };

    private bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
    }
}
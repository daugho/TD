using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Vector2Int Position;
    public float G, H;
    public Node Parent;
    public int HeapIndex { get; set; }

    public Node(Vector2Int pos, float g, float h, Node parent)
    {
        Position = pos;
        G = g;//시작점 → 현재 노드까지 실제로 온 거리
        H = h;//현재 노드 → 목표 노드까지의 예상 거리 (휴리스틱)
        Parent = parent;
    }

    public float F => G + H;//가장 낮은 F를 기준으로 탐색 우선순위 결정

    public int CompareTo(Node other)
    {
        return F.CompareTo(other.F);
    }
}
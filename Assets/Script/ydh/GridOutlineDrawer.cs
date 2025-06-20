using UnityEngine;

public class GridOutlineDrawer : MonoBehaviour
{
    public Material testLineMaterial;

    void Start()
    {
        for (int i = 0; i <= 10; i++)
        {
            DrawGridLine(new Vector3(i, 0.6f, 0), new Vector3(i, 0.6f, 10));
            DrawGridLine(new Vector3(0, 0.6f, i), new Vector3(10, 0.6f, i));
        }
    }

    void DrawGridLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        var lr = lineObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { start, end });
        lr.startWidth = lr.endWidth = 0.05f;
        lr.material = testLineMaterial ?? new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.red;
    }
}
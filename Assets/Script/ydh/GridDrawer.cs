using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public Material lineMaterial;     // 반드시 커스텀 셰이더나 Unlit 셰이더 사용
    public int gridSize = 20;
    public float tileSize = 1f;

    private void OnRenderObject()
    {
        if (lineMaterial == null)
        {
            Debug.LogWarning("Line Material이 설정되지 않았습니다.");
            return;
        }

        lineMaterial.SetPass(0); // 머티리얼 적용
        GL.Begin(GL.LINES);
        GL.Color(Color.green); // 선 색상

        // 가로선
        for (int z = 0; z <= gridSize; z++)
        {
            GL.Vertex3(0, 0.01f, z * tileSize);
            GL.Vertex3(gridSize * tileSize, 0.01f, z * tileSize);
        }

        // 세로선
        for (int x = 0; x <= gridSize; x++)
        {
            GL.Vertex3(x * tileSize, 0.01f, 0);
            GL.Vertex3(x * tileSize, 0.01f, gridSize * tileSize);
        }

        GL.End();
    }
}
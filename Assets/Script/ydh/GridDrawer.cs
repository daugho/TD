using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public Material lineMaterial;     // �ݵ�� Ŀ���� ���̴��� Unlit ���̴� ���
    public int gridSize = 20;
    public float tileSize = 1f;

    private void OnRenderObject()
    {
        if (lineMaterial == null)
        {
            Debug.LogWarning("Line Material�� �������� �ʾҽ��ϴ�.");
            return;
        }

        lineMaterial.SetPass(0); // ��Ƽ���� ����
        GL.Begin(GL.LINES);
        GL.Color(Color.green); // �� ����

        // ���μ�
        for (int z = 0; z <= gridSize; z++)
        {
            GL.Vertex3(0, 0.01f, z * tileSize);
            GL.Vertex3(gridSize * tileSize, 0.01f, z * tileSize);
        }

        // ���μ�
        for (int x = 0; x <= gridSize; x++)
        {
            GL.Vertex3(x * tileSize, 0.01f, 0);
            GL.Vertex3(x * tileSize, 0.01f, gridSize * tileSize);
        }

        GL.End();
    }
}
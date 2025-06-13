using UnityEngine;

public class GridResetButton : MonoBehaviour
{
    [SerializeField] private GridGenerator gridGenerator;

    public void OnClickResetGrid()
    {
        if (gridGenerator == null)
        {
            Debug.LogError("GridGenerator ������ �����ϴ�.");
            return;
        }

        gridGenerator.GenerateGrid(); // ���� Ÿ�� ���� �� ���� ����
        Debug.Log("?? �� �ʱ�ȭ �Ϸ�");
    }
}

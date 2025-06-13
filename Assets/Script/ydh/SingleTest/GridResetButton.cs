using UnityEngine;

public class GridResetButton : MonoBehaviour
{
    [SerializeField] private GridGenerator gridGenerator;

    public void OnClickResetGrid()
    {
        if (gridGenerator == null)
        {
            Debug.LogError("GridGenerator 참조가 없습니다.");
            return;
        }

        gridGenerator.GenerateGrid(); // 기존 타일 제거 후 새로 생성
        Debug.Log("?? 맵 초기화 완료");
    }
}

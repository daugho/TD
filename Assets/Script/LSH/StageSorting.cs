using UnityEngine;

public class StageSorting : MonoBehaviour
{
    public float verticalOffset = 50f; // 위아래로 이동할 거리

    void Start()
    {
        // Content의 자식 객체 가져오기
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        // Zigzag 배치 적용
        for (int i = 0; i < children.Length; i++)
        {
            RectTransform rectTransform = children[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // 기존 위치를 기반으로 조정
                Vector2 originalPosition = rectTransform.anchoredPosition;
                float yOffset = (i % 2 == 0) ? -verticalOffset : verticalOffset; // 짝수는 위, 홀수는 아래
                rectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + yOffset);
            }
        }
    }
}

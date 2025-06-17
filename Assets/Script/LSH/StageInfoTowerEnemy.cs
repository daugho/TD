using UnityEngine;
using UnityEngine.UI;

public class AddChildImages : MonoBehaviour
{
    [SerializeField] private Image _parentImage; // 부모 이미지
    [SerializeField] private Sprite[] _childSprites; // 추가할 스프라이트 배열
    [SerializeField] private float _spacing = 100f; // 각 자식 이미지 간의 x 간격
    [SerializeField] private float _fixedY = 0f; // y 값 고정
    [SerializeField] private Vector2 _childImageSize = new Vector2(100, 100); // 자식 이미지 크기
    [SerializeField] private Vector2 _firstImageOffset = new Vector2(-300, 0); // 첫 번째 이미지의 위치

    void Start()
    {
        AddChildren();
    }

    public void AddChildren()
    {
        for (int i = 0; i < _childSprites.Length; i++)
        {
            // Child Image 생성
            GameObject childImageObj = new GameObject($"Child Image {i + 1}");
            childImageObj.transform.SetParent(_parentImage.transform, false);

            // Child Image 컴포넌트 추가
            Image childImage = childImageObj.AddComponent<Image>();
            childImage.sprite = _childSprites[i];

            // RectTransform 설정
            RectTransform rectTransform = childImageObj.GetComponent<RectTransform>();

            // 첫 번째 이미지 위치
            if (i == 0)
            {
                rectTransform.anchoredPosition = _firstImageOffset;
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(_firstImageOffset.x + i * _spacing, _fixedY);
            }

            rectTransform.sizeDelta = _childImageSize;
        }
    }
}

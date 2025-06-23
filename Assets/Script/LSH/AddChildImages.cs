using UnityEngine;
using UnityEngine.UI;

public class AddChildImages : MonoBehaviour
{
    [SerializeField] private Image _parentImage; // �θ� �̹���
    [SerializeField] private Sprite[] _childSprites; // �߰��� ��������Ʈ �迭
    [SerializeField] private float _spacing = 100f; // �� �ڽ� �̹��� ���� x ����
    [SerializeField] private float _fixedY = 0f; // y �� ����
    [SerializeField] private Vector2 _childImageSize = new Vector2(100, 100); // �ڽ� �̹��� ũ��
    [SerializeField] private Vector2 _firstImageOffset = new Vector2(-300, 0); // ù ��° �̹����� ��ġ

    void Start()
    {
        AddChildren();
    }

    public void AddChildren()
    {
        for (int i = 0; i < _childSprites.Length; i++)
        {
            // Child Image ����
            GameObject childImageObj = new GameObject($"Child Image {i + 1}");
            childImageObj.transform.SetParent(_parentImage.transform, false);

            // Child Image ������Ʈ �߰�
            Image childImage = childImageObj.AddComponent<Image>();
            childImage.sprite = _childSprites[i];

            // RectTransform ����
            RectTransform rectTransform = childImageObj.GetComponent<RectTransform>();

            // ù ��° �̹��� ��ġ
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

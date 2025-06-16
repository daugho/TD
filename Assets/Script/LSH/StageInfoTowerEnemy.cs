using UnityEngine;
using UnityEngine.UI;

public class AddChildImages : MonoBehaviour
{
    [SerializeField] private Image parentImage; // �θ� �̹���
    [SerializeField] private Sprite[] childSprites; // �߰��� ��������Ʈ �迭
    [SerializeField] private float spacing = 50f; // �� �ڽ� �̹��� ���� x ����
    [SerializeField] private float fixedY = 0f; // y �� ����
    [SerializeField] private Vector2 childImageSize = new Vector2(50, 50); // �ڽ� �̹��� ũ��

    public Vector2 firstImageOffset = new Vector2(10, 0); // ù ��° �̹����� ��ġ

    void Start()
    {
        AddChildren();
    }

    public void AddChildren()
    {
        for (int i = 0; i < childSprites.Length; i++)
        {
            // Child Image ����
            GameObject childImageObj = new GameObject($"Child Image {i + 1}");
            childImageObj.transform.SetParent(parentImage.transform, false);

            // Child Image ������Ʈ �߰�
            Image childImage = childImageObj.AddComponent<Image>();
            childImage.sprite = childSprites[i];

            // RectTransform ����
            RectTransform rectTransform = childImageObj.GetComponent<RectTransform>();

            // ù ��° �̹��� ��ġ
            if (i == 0)
            {
                rectTransform.anchoredPosition = firstImageOffset;
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(firstImageOffset.x + i * spacing, fixedY);
            }

            rectTransform.sizeDelta = childImageSize;
        }
    }
}

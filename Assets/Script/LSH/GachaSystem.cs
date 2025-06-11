using UnityEngine;
using UnityEngine.UI;

public class GachaSystem : MonoBehaviour
{
    [SerializeField]
    private Sprite circleSprite;
    [SerializeField]
    private Sprite triangleSprite;
    [SerializeField]
    private Sprite squareSprite;
    [SerializeField]
    private Sprite starSprite;

    [SerializeField]
    private Transform content; // 보관함(Content)
    [SerializeField]
    private GameObject shapeButtonPrefab; // 버튼 Prefab

    public void OnGachaButtonClick()
    {
        int randomValue = Random.Range(1, 101);
        Sprite selectedSprite = null;

        if (randomValue <= 40)
            selectedSprite = circleSprite;
        else if (randomValue <= 70)
            selectedSprite = triangleSprite;
        else if (randomValue <= 90)
            selectedSprite = squareSprite;
        else
            selectedSprite = starSprite;

        CreateShapeButton(selectedSprite);
    }

    private void CreateShapeButton(Sprite shapeSprite)
    {
        // 버튼 생성
        GameObject newButton = Instantiate(shapeButtonPrefab, content);

        // 버튼의 이미지 설정
        Image buttonImage = newButton.GetComponent<Image>();
        buttonImage.sprite = shapeSprite;
    }
}

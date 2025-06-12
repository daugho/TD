using UnityEngine;
using UnityEngine.UI;

public class GachaSystem : MonoBehaviour
{
    [SerializeField]
    private Sprite _circleSprite;
    [SerializeField]
    private Sprite _triangleSprite;
    [SerializeField]
    private Sprite _squareSprite;
    [SerializeField]
    private Sprite _starSprite;

    [SerializeField]
    private Transform _content; // 보관함(Content)
    [SerializeField]
    private GameObject _shapeButtonPrefab; // 버튼 Prefab
    [SerializeField]
    private int _invenMaxCount = 10;

    private int _curButtonCount = 0;

    public void OnGachaButtonClick()
    {
        if(_curButtonCount >= _invenMaxCount)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return;
        }

        int randomValue = Random.Range(1, 101);
        Sprite selectedSprite = null;

        if (randomValue <= 40)
            selectedSprite = _circleSprite;
        else if (randomValue <= 70)
            selectedSprite = _triangleSprite;
        else if (randomValue <= 90)
            selectedSprite = _squareSprite;
        else
            selectedSprite = _starSprite;

        CreateShapeButton(selectedSprite);
    }

    private void CreateShapeButton(Sprite shapeSprite)
    {
        // 버튼 생성
        GameObject newButton = Instantiate(_shapeButtonPrefab, _content);

        // 버튼의 이미지 설정
        Image buttonImage = newButton.GetComponent<Image>();
        buttonImage.sprite = shapeSprite;

        _curButtonCount++;
        Debug.Log($"버튼 생성 완료! 현재 버튼 수: {_curButtonCount}/{_invenMaxCount}");
    }
}

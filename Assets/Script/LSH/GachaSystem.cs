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
    private Transform _content; // ������(Content)
    [SerializeField]
    private GameObject _shapeButtonPrefab; // ��ư Prefab
    [SerializeField]
    private int _invenMaxCount = 10;

    private int _curButtonCount = 0;

    public void OnGachaButtonClick()
    {
        if(_curButtonCount >= _invenMaxCount)
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�.");
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
        // ��ư ����
        GameObject newButton = Instantiate(_shapeButtonPrefab, _content);

        // ��ư�� �̹��� ����
        Image buttonImage = newButton.GetComponent<Image>();
        buttonImage.sprite = shapeSprite;

        _curButtonCount++;
        Debug.Log($"��ư ���� �Ϸ�! ���� ��ư ��: {_curButtonCount}/{_invenMaxCount}");
    }
}

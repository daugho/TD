using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TowerPlaceBtn : MonoBehaviour
{
    private Image _image;
    private Button _button;

    private void Awake()
    {
        _image = GetComponent<Image>(); 
        _button = GetComponent<Button>();
    }
    public void SetImage(string path)
    {
        string imagePath = "Prefabs/UI/Tower/" + path; 
        Sprite towerSprite = Resources.Load<Sprite>(imagePath);

        _image.sprite = towerSprite;

        //_curButtonCount++;
        //Debug.Log($"버튼 생성 완료! 현재 버튼 수: {_curButtonCount}/{_invenMaxCount}");
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TowerPlaceBtn : MonoBehaviour
{
    private Image _image;
    [SerializeField]
    private Transform _content;


    private UnityEvent _event;
    private Button _button;

    private void Awake()
    {
        _image = GetComponent<Image>(); 
        _button = GetComponent<Button>();
    }
    public void SetEvent(UnityEvent eventData)
    {
        _button.onClick.AddListener(() =>
        {
            if (_event != null)
            {
                eventData.Invoke();
            }
            else
            {
                Debug.LogWarning("이벤트가 설정되지 않았습니다.");
            }
        });
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

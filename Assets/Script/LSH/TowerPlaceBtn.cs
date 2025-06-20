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
                Debug.LogWarning("�̺�Ʈ�� �������� �ʾҽ��ϴ�.");
            }
        });
    }

    public void SetImage(string path)
    {
        string imagePath = "Prefabs/UI/Tower/" + path; 
        Sprite towerSprite = Resources.Load<Sprite>(imagePath);

        _image.sprite = towerSprite;

        //_curButtonCount++;
        //Debug.Log($"��ư ���� �Ϸ�! ���� ��ư ��: {_curButtonCount}/{_invenMaxCount}");
    }
}

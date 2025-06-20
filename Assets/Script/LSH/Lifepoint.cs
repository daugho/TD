using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Lifepoint : MonoBehaviour
{
    public static Lifepoint Instance;

    [SerializeField] private GameObject _heartPrefab;
    [SerializeField] private Sprite _filledHeart;
    [SerializeField] private Sprite _emptyHeart;
    [SerializeField] private int _maxLife = 10; 

    private Image[] _hearts; 
    private int _currentLife;

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        _currentLife = _maxLife;

        _hearts = new Image[_maxLife];

        for (int i = 0; i < _maxLife; i++)
        {
            GameObject heart = Instantiate(_heartPrefab, transform);
            _hearts[i] = heart.GetComponent<Image>();
            _hearts[i].sprite = _filledHeart;
        }
    }

    public void DecreaseLife()
    {
        if (_currentLife > 0)
        {
            int deletedir = _maxLife - _currentLife; // øﬁ¬ ∫Œ≈Õ ∫Û «œ∆Æ∏¶ º≥¡§
            _hearts[deletedir].sprite = _emptyHeart;
            _currentLife--;
        }
    }

    public void ResetLife()
    {
        _currentLife = _maxLife; 
        for (int i = 0; i < _maxLife; i++)
        {
            _hearts[i].sprite = _filledHeart; 
        }
    }
}

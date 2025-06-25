using UnityEngine;
using TMPro;

public class BlinkAnim : MonoBehaviour
{
    private float _time;
    private bool _isIncreasing = true;

    [SerializeField]
    private TextMeshProUGUI _text; // 깜빡임 효과를 적용할 텍스트

    [SerializeField]
    private float _blinkSpeed = 1f; // 깜빡이는 속도 (1초 기준)

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();

        if (_text == null)
        {
            Debug.LogError($"TextMeshProUGUI is missing on {gameObject.name}");
        }
    }

    private void Update()
    {
        if (_text == null) return;

        // 깜빡임 효과 계산
        if (_isIncreasing)
        {
            _time += Time.deltaTime * _blinkSpeed;
            if (_time >= 1f)
            {
                _time = 1f;
                _isIncreasing = false;
            }
        }
        else
        {
            _time -= Time.deltaTime * _blinkSpeed;
            if (_time <= 0f)
            {
                _time = 0f;
                _isIncreasing = true;
            }
        }

        // 텍스트 알파값 변경
        Color color = _text.color;
        color.a = _time;
        _text.color = color;
    }
}

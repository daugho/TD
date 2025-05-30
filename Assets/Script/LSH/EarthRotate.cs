using UnityEngine;

public class EarthRotate : MonoBehaviour
{
    public Transform earth;
    private RectTransform _content;
    [SerializeField] 
    private float _rotationMultiplier = 1f;

    private void Awake()
    {
        _content = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float contentX = _content.anchoredPosition.x;

        float newYRotation = -contentX * _rotationMultiplier;

        earth.localRotation = Quaternion.Euler(0f, newYRotation, 0f);
    }
}

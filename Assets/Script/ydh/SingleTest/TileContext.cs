using TMPro;
using UnityEngine;

public class TileContext : MonoBehaviour
{
    [Header("타일 공통 참조")]
    [SerializeField] private Transform _tileParent;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private TMP_InputField _xInputField;
    [SerializeField] private TMP_InputField _zInputField;

    public Transform TileParent => _tileParent;
    public GameObject TilePrefab=> _tilePrefab;
    public int Width=> int.TryParse(_xInputField.text,out int val) ? val : 0 ;
    public int Height => int.TryParse(_zInputField.text, out int val) ? val : 0;
    public void SetDimensions(int width, int height)//초기화 전용 함수.
    {
        _xInputField.text = width.ToString();
        _zInputField.text = height.ToString();
    }
}

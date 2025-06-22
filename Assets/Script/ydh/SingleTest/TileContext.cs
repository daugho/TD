using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileContext : MonoBehaviour
{
    private bool _isEditMode = true;

    [Header("Ÿ�� ���� ����")]
    [SerializeField] private Transform _tileParent;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _startPointPrefab;
    [SerializeField] private GameObject _endPointPrefab;
    [SerializeField] private TMP_InputField _xInputField;
    [SerializeField] private TMP_InputField _zInputField;

    private int _width = 0;
    private int _height = 0;
    public Transform TileParent => _tileParent;
    public GameObject TilePrefab=> _tilePrefab;
    public GameObject StartPointPrefab => _startPointPrefab;
    public GameObject EndPointPrefab => _endPointPrefab;
    //InputField ���̵� ���� ������
    public int Width => _isEditMode
        ? int.TryParse(_xInputField.text, out int val) ? val : 0
        : _width;

    public int Height => _isEditMode
        ? int.TryParse(_zInputField.text, out int val) ? val : 0
        : _height;
    private void Awake()
    {
        // �����ÿ� �� �̸��� �������� �ڵ� ����
        string currentScene = SceneManager.GetActiveScene().name;

        // ����: "MapEditor" �������� ����Ʈ ���, �� �ܴ� �÷��� ���
        _isEditMode = currentScene.Contains("Editor") || currentScene == "MapEditScene";

        Debug.Log($"[TileContext] Scene: {currentScene}, EditMode: {_isEditMode}");
    }
    public void SetDimensions(int width, int height)
    {
        _width = width;
        _height = height;

        if (_xInputField != null)
            _xInputField.text = width.ToString();
        if (_zInputField != null)
            _zInputField.text = height.ToString();
    }
}

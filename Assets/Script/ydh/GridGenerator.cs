using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    public Transform _tileParent;
    public TMP_InputField _xInputField;
    public TMP_InputField _zInputField;
    public Button _generateButton;
    public GameObject _tilePrefab;
    void Start()
    {
        _generateButton.onClick.AddListener(GenerateGrid);
    }
    public void GenerateGrid()
    {
        foreach (Transform child in _tileParent)
        {
            Destroy(child.gameObject);//기존 타일을 제거합니다.
        }
        if (!int.TryParse(_xInputField.text, out int xCount)) return;
        if (!int.TryParse(_zInputField.text, out int zCount)) return;
        for (int x = 0; x < xCount; x++)
        {
            for (int z = 0; z < zCount; z++)
            {
                Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                GameObject tile = Instantiate(_tilePrefab, pos, Quaternion.identity, _tileParent);
                tile.name = $"Tile_ {x}_{z}";
            }
        }
    }
}

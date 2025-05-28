using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private TileContext _tileContext;
    [SerializeField] private Button _generateButton;

    void Start()
    {
        _generateButton.onClick.AddListener(GenerateGrid);
    }

    public void GenerateGrid()
    {
        foreach (Transform child in _tileContext.TileParent)
        {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < _tileContext.Width; x++)
        {
            for (int z = 0; z < _tileContext.Height; z++)
            {
                Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                GameObject tile = Instantiate(_tileContext.TilePrefab, pos, Quaternion.identity, _tileContext.TileParent);
                tile.name = $"Tile_{x}_{z}";
            }
        }
    }
}
using System.IO;
using UnityEngine;
using TMPro;

public class LocalMapSerializer : MonoBehaviour
{
    [SerializeField] private TileContext tileContext;
    [SerializeField] private TMP_InputField fileNameInput;

    private string FileName => string.IsNullOrWhiteSpace(fileNameInput.text)
        ? "map_data.json" : AppendJson(fileNameInput.text);

    private string AppendJson(string name) =>
        name.EndsWith(".json") ? name : name + ".json";

    public void SaveGridToJson()
    {
        GridData gridData = new GridData
        {
            width = tileContext.Width,
            height = tileContext.Height
        };

        foreach (Transform tile in tileContext.TileParent)
        {
            var tb = tile.GetComponent<TileBehaviour>();
            if (tb == null) continue;

            gridData.tiles.Add(new TileData
            {
                x = tb.CoordX,
                z = tb.CoordZ,
                state = tb._tileState,
                access = tb._accessType
            });
        }

        string path = Path.Combine(Application.persistentDataPath, FileName);
        File.WriteAllText(path, JsonUtility.ToJson(gridData, true));
        Debug.Log($"[LocalMapSerializer] 저장 완료: {path}");
    }

    public void LoadGridFromJson()
    {
        string path = Path.Combine(Application.persistentDataPath, FileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"[LocalMapSerializer] 파일 없음: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        GridData gridData = JsonUtility.FromJson<GridData>(json);

        tileContext.SetDimensions(gridData.width, gridData.height);

        foreach (Transform child in tileContext.TileParent)
            Destroy(child.gameObject);

        foreach (var tileData in gridData.tiles)
        {
            Vector3 pos = new Vector3(tileData.x + 0.5f, 0, tileData.z + 0.5f);
            GameObject tile = Instantiate(tileContext.TilePrefab, pos, Quaternion.identity, tileContext.TileParent);
            tile.name = $"Tile_{tileData.x}_{tileData.z}";

            TileBehaviour tileBehaviour = tile.GetComponent<TileBehaviour>();
            if (tileBehaviour != null)
            {
                tileBehaviour.SetCoordinates(tileData.x, tileData.z);
                tileBehaviour.SetTileState(tileData.state, tileData.access);
            }
        }

        Debug.Log($"[LocalMapSerializer] 불러오기 완료: {path}");
    }
}

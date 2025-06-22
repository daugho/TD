using System.IO;
using UnityEngine;

public static class LocalMapSerializer
{
    public static void SaveGridToJson(TileContext context, string fileName)
    {
        GridData gridData = new GridData
        {
            width = context.Width,
            height = context.Height
        };

        foreach (Transform tile in context.TileParent)
        {
            TileBehaviour tb = tile.GetComponent<TileBehaviour>();
            if (tb == null) continue;

            gridData.tiles.Add(new TileData
            {
                x = tb.CoordX,
                z = tb.CoordZ,
                state = tb._tileState,
                access = tb._accessType
            });
        }

        string path = Path.Combine(Application.persistentDataPath, EnsureJson(fileName));
        File.WriteAllText(path, JsonUtility.ToJson(gridData, true));
        Debug.Log($"[LocalMapSerializer] 저장 완료: {path}");
    }

    public static GridData LoadGridFromJson(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, EnsureJson(fileName));
        if (!File.Exists(path))
        {
            Debug.LogError($"[LocalMapSerializer] 파일 없음: {path}");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<GridData>(json);
    }

    private static string EnsureJson(string name) =>
        name.EndsWith(".json") ? name : name + ".json";
}

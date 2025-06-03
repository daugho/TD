using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private TileContext _tileContext;
    [SerializeField] private TMP_InputField FileName;

    private string SaveFileName => string.IsNullOrWhiteSpace(FileName.text)
        ? "map_data.json" : AppendJson(FileName.text);
    private string LoadFileName => AppendJson(FileName.text);
    private string AppendJson(string fileName) =>
        fileName.EndsWith(".json") ? fileName : fileName + ".json";

    // 저장용 데이터 구조
    public void SaveGridToJson()
    {
        GridData gridData = new GridData
        {
            width = _tileContext.Width,
            height = _tileContext.Height
        };

        foreach (Transform tile in _tileContext.TileParent)
        {
            TileBehaviour tileBehaviour = tile.GetComponent<TileBehaviour>();
            if (tileBehaviour != null)
            {
                string[] parts = tile.name.Split('_');
                int x = int.Parse(parts[1]);
                int z = int.Parse(parts[2]);

                gridData.tiles.Add(new TileData
                {
                    x = x,
                    z = z,
                    state = tileBehaviour._tileState,
                    access = tileBehaviour._accessType  // ? 권한 정보 추가
                });
            }
        }

        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        File.WriteAllText(path, JsonUtility.ToJson(gridData, true));

        Debug.Log($"맵 저장 완료: {path}");
    }

    // 일반 파일 로딩
    public void LoadGridFromJson()
    {
        string path = Path.Combine(Application.persistentDataPath, LoadFileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"파일이 존재하지 않습니다: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        GridData gridData = JsonUtility.FromJson<GridData>(json);
        _tileContext.SetDimensions(gridData.width, gridData.height);

        foreach (Transform child in _tileContext.TileParent)
            Destroy(child.gameObject);

        Dictionary<string, (TileState state, TileAccessType access)> tileInfos = new();
        foreach (TileData tileData in gridData.tiles)
        {
            tileInfos[$"Tile_{tileData.x}_{tileData.z}"] = (tileData.state, tileData.access);
        }

        for (int x = 0; x < gridData.width; x++)
        {
            for (int z = 0; z < gridData.height; z++)
            {
                Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                string tileName = $"Tile_{x}_{z}";
                GameObject tile = Instantiate(_tileContext.TilePrefab, pos, Quaternion.identity, _tileContext.TileParent);
                tile.name = tileName;

                var tileBehaviour = tile.GetComponent<TileBehaviour>();
                if (tileBehaviour != null)
                {
                    if (tileInfos.TryGetValue(tileName, out var info))
                        tileBehaviour.SetTileState(info.state, info.access);
                    else
                        tileBehaviour.SetTileState(TileState.None);
                }
            }
        }

        Debug.Log($"맵 로드 완료: {path}");
    }

    // Firebase용 저장(JSON 반환)
    public string SaveMaptoFirebase()
    {
        GridData gridData = new GridData
        {
            width = _tileContext.Width,
            height = _tileContext.Height
        };

        foreach (Transform tile in _tileContext.TileParent)
        {
            var tileBehaviour = tile.GetComponent<TileBehaviour>();
            if (tileBehaviour == null) continue;

            string[] parts = tile.name.Split('_');
            int x = int.Parse(parts[1]);
            int z = int.Parse(parts[2]);

            gridData.tiles.Add(new TileData
            {
                x = x,
                z = z,
                state = tileBehaviour._tileState,
                access = tileBehaviour._accessType // ? 권한 정보 포함
            });
        }

        return JsonUtility.ToJson(gridData, true);
    }

    // Firebase에서 JSON 불러오기
    public void LoadMapFromFirebase(string json)
    {
        GridData gridData = JsonUtility.FromJson<GridData>(json);
        _tileContext.SetDimensions(gridData.width, gridData.height);

        foreach (Transform child in _tileContext.TileParent)
            Destroy(child.gameObject);

        Dictionary<string, (TileState state, TileAccessType access)> tileInfos = new();
        foreach (TileData tileData in gridData.tiles)
        {
            tileInfos[$"Tile_{tileData.x}_{tileData.z}"] = (tileData.state, tileData.access);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            for (int x = 0; x < gridData.width; x++)
            {
                for (int z = 0; z < gridData.height; z++)
                {
                    Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                    string tileName = $"Tile_{x}_{z}";

                    GameObject tile = PhotonNetwork.Instantiate("Tile", pos, Quaternion.identity);
                    tile.name = tileName;
                    tile.transform.SetParent(_tileContext.TileParent);

                    var tileBehaviour = tile.GetComponent<TileBehaviour>();
                    var view = tile.GetComponent<PhotonView>();

                    if (tileBehaviour != null && view != null)
                    {
                        if (tileInfos.TryGetValue(tileName, out var info))
                        {
                            view.RPC("RPC_SetTileState", RpcTarget.AllBuffered, (int)info.state, (int)info.access);
                        }
                        else
                        {
                            view.RPC("RPC_SetTileState", RpcTarget.AllBuffered, (int)TileState.None, (int)TileAccessType.Everyone);
                        }
                    }
                }
            }
        }
    }
}


//using Photon.Pun;
//using System.Collections.Generic;
//using System.IO;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class GridManager : MonoBehaviour
//{
//    [SerializeField] private TileContext _tileContext;
//    [SerializeField] private TMP_InputField FileName;

//    private string SaveFileName => string.IsNullOrWhiteSpace(FileName.text)
//        ? "map_data.json" : AppendJson(FileName.text);
//    private string LoadFileName => AppendJson(FileName.text);
//    private string AppendJson(string fileName) =>
//        fileName.EndsWith(".json") ? fileName : fileName + ".json";

//    public void SaveGridToJson()
//    {
//        GridData gridData = new GridData
//        {
//            width = _tileContext.Width,
//            height = _tileContext.Height
//        };

//        foreach (Transform tile in _tileContext.TileParent)
//        {
//            TileBehaviour tileBehaviour = tile.GetComponent<TileBehaviour>();
//            if (tileBehaviour != null)
//            {
//                string[] parts = tile.name.Split('_');
//                int x = int.Parse(parts[1]);
//                int z = int.Parse(parts[2]);

//                gridData.tiles.Add(new TileData
//                {
//                    x = x,
//                    z = z,
//                    state = tileBehaviour._tileState
//                });
//            }
//        }

//        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
//        File.WriteAllText(path, JsonUtility.ToJson(gridData, true));

//        Debug.Log($"맵 저장 완료: {path}");
//    }

//    public void LoadGridFromJson()
//    {
//        string path = Path.Combine(Application.persistentDataPath, LoadFileName);
//        if (!File.Exists(path))
//        {
//            Debug.LogError($"파일이 존재하지 않습니다: {path}");
//            return;
//        }

//        string json = File.ReadAllText(path);
//        GridData gridData = JsonUtility.FromJson<GridData>(json);

//        foreach (Transform child in _tileContext.TileParent)
//            Destroy(child.gameObject);

//        Dictionary<string, TileState> tileStates = new();
//        foreach (TileData tileData in gridData.tiles)
//        {
//            tileStates[$"Tile_{tileData.x}_{tileData.z}"] = tileData.state;
//        }

//        for (int x = 0; x < gridData.width; x++)
//        {
//            for (int z = 0; z < gridData.height; z++)
//            {
//                Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
//                string tileName = $"Tile_{x}_{z}";
//                GameObject tile = Instantiate(_tileContext.TilePrefab, pos, Quaternion.identity, _tileContext.TileParent);
//                tile.name = tileName;

//                var tileBehaviour = tile.GetComponent<TileBehaviour>();
//                if (tileBehaviour != null)
//                {
//                    tileBehaviour.SetTileState(tileStates.TryGetValue(tileName, out var s) ? s : TileState.None);
//                }
//            }
//        }

//        Debug.Log($"맵 로드 완료: {path}");
//    }
//    public string SaveMaptoFirebase()
//    {
//        GridData gridData = new GridData
//        {
//            width = _tileContext.Width,
//            height = _tileContext.Height
//        };

//        foreach (Transform tile in _tileContext.TileParent)
//        {
//            var tileBehaviour = tile.GetComponent<TileBehaviour>();
//            if (tileBehaviour == null) continue;

//            string[] parts = tile.name.Split('_');
//            int x = int.Parse(parts[1]);
//            int z = int.Parse(parts[2]);

//            gridData.tiles.Add(new TileData
//            {
//                x = x,
//                z = z,
//                state = tileBehaviour._tileState
//            });
//        }

//        return JsonUtility.ToJson(gridData, true);
//    }
//    public void LoadMapFromFirebase(string json)
//    {
//        GridData gridData = JsonUtility.FromJson<GridData>(json);

//        foreach (Transform child in _tileContext.TileParent)
//            Destroy(child.gameObject);

//        Dictionary<string, TileState> tileStates = new();
//        foreach (TileData tileData in gridData.tiles)
//            tileStates[$"Tile_{tileData.x}_{tileData.z}"] = tileData.state;

//        if (PhotonNetwork.IsMasterClient)
//        {
//            for (int x = 0; x < gridData.width; x++)
//            {
//                for (int z = 0; z < gridData.height; z++)
//                {
//                    Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
//                    string tileName = $"Tile_{x}_{z}";

//                    GameObject tile = PhotonNetwork.Instantiate("Tile", pos, Quaternion.identity);
//                    tile.name = tileName;
//                    tile.transform.SetParent(_tileContext.TileParent);

//                    var tileBehaviour = tile.GetComponent<TileBehaviour>();
//                    var view = tile.GetComponent<PhotonView>();

//                    if (tileBehaviour != null && view != null)
//                    {
//                        // 상태를 직접 적용하는 것이 아니라 RPC로 모든 클라이언트에 적용
//                        if (tileStates.TryGetValue(tileName, out var state))
//                        {
//                            view.RPC("RPC_SetTileState", RpcTarget.AllBuffered, (int)state);
//                        }
//                        else
//                        {
//                            view.RPC("RPC_SetTileState", RpcTarget.AllBuffered, (int)TileState.None);
//                        }
//                    }
//                }
//            }
//        }
//    }

//}
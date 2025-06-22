using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private TileContext _tileContext;
    [SerializeField] private TMP_InputField FileName;


    private Dictionary<(int, int), TileBehaviour> tileDictionary = new();

    private string SaveFileName => string.IsNullOrWhiteSpace(FileName.text)
        ? "map_data.json" : AppendJson(FileName.text);
    private string LoadFileName => AppendJson(FileName.text);
    private string AppendJson(string fileName) =>
        fileName.EndsWith(".json") ? fileName : fileName + ".json";

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
                gridData.tiles.Add(new TileData
                {
                    x = tileBehaviour.CoordX,
                    z = tileBehaviour.CoordZ,
                    state = tileBehaviour._tileState,
                    access = tileBehaviour._accessType
                });
            }
        }

        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        File.WriteAllText(path, JsonUtility.ToJson(gridData, true));

        Debug.Log($"맵 저장 완료: {path}");
    }

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

        tileDictionary.Clear();

        Dictionary<(int, int), (TileState state, TileAccessType access)> tileInfos = new();
        foreach (TileData tileData in gridData.tiles)
        {
            tileInfos[(tileData.x, tileData.z)] = (tileData.state, tileData.access);
        }

        for (int x = 0; x < gridData.width; x++)
        {
            for (int z = 0; z < gridData.height; z++)
            {
                Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                GameObject tile = Instantiate(_tileContext.TilePrefab, pos, Quaternion.identity, _tileContext.TileParent);
                tile.name = $"Tile_{x}_{z}";

                var tileBehaviour = tile.GetComponent<TileBehaviour>();
                if (tileBehaviour != null)
                {
                    tileBehaviour.SetCoordinates(x, z);
                    tileDictionary[(x, z)] = tileBehaviour;

                    if (tileInfos.TryGetValue((x, z), out var info))
                        tileBehaviour.SetTileState(info.state, info.access);
                    else
                        tileBehaviour.SetTileState(TileState.None);
                }
            }
        }

        Debug.Log($"맵 로드 완료: {path}");
    }

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

            gridData.tiles.Add(new TileData
            {
                x = tileBehaviour.CoordX,
                z = tileBehaviour.CoordZ,
                state = tileBehaviour._tileState,
                access = tileBehaviour._accessType
            });
        }

        return JsonUtility.ToJson(gridData, true);
    }
    public void LoadMapFromFirebase(string json)
    {
        GridData gridData = JsonUtility.FromJson<GridData>(json);
        _tileContext.SetDimensions(gridData.width, gridData.height);

        foreach (Transform child in _tileContext.TileParent)
            Destroy(child.gameObject);

        tileDictionary.Clear();

        Dictionary<(int, int), (TileState state, TileAccessType access)> tileInfos = new();
        foreach (TileData tileData in gridData.tiles)
        {
            tileInfos[(tileData.x, tileData.z)] = (tileData.state, tileData.access);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            for (int x = 0; x < gridData.width; x++)
            {
                for (int z = 0; z < gridData.height; z++)
                {
                    Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                    string prefabName = "Tile"; // 기본 프리팹 이름

                    if (tileInfos.TryGetValue((x, z), out var tileInfo))
                    {
                        if (tileInfo.state == TileState.StartPoint)
                            prefabName = "StartPointTile";
                        else if (tileInfo.state == TileState.EndPoint)
                            prefabName = "EndPointTile";
                    }

                    GameObject tile = PhotonNetwork.Instantiate(prefabName, pos, Quaternion.identity);
                    tile.name = $"Tile_{x}_{z}";
                    tile.transform.SetParent(_tileContext.TileParent);

                    var tileBehaviour = tile.GetComponent<TileBehaviour>();
                    var view = tile.GetComponent<PhotonView>();

                    if (tileBehaviour != null && view != null)
                    {
                        view.RPC("SetCoordinates", RpcTarget.AllBuffered, x, z);
                        tileDictionary[(x, z)] = tileBehaviour;

                        if (tileInfos.TryGetValue((x, z), out var info))
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

        StartCoroutine(DelayedSpawnPathTracer());

        TileChecker tileChecker = FindFirstObjectByType<TileChecker>();
        if (tileChecker != null)
        {
            tileChecker.HideNoneTiles();
            tileChecker.photonView.RPC(nameof(TileChecker.RPC_HideNoneTiles), RpcTarget.AllBuffered);
            Debug.Log("[GridManager] TileChecker가 None 상태 타일을 처리했습니다.");
        }
        else
        {
            Debug.LogWarning("[GridManager] TileChecker를 찾을 수 없습니다.");
        }
    }

    //public void LoadMapFromFirebase(string json)
    //{
    //    GridData gridData = JsonUtility.FromJson<GridData>(json);
    //    _tileContext.SetDimensions(gridData.width, gridData.height);

    //    foreach (Transform child in _tileContext.TileParent)
    //        Destroy(child.gameObject);

    //    tileDictionary.Clear();

    //    Dictionary<(int, int), (TileState state, TileAccessType access)> tileInfos = new();
    //    foreach (TileData tileData in gridData.tiles)
    //    {
    //        tileInfos[(tileData.x, tileData.z)] = (tileData.state, tileData.access);
    //    }

    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        for (int x = 0; x < gridData.width; x++)
    //        {
    //            for (int z = 0; z < gridData.height; z++)
    //            {
    //                Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
    //                GameObject tile = PhotonNetwork.Instantiate("Tile", pos, Quaternion.identity);
    //                tile.name = $"Tile_{x}_{z}";
    //                tile.transform.SetParent(_tileContext.TileParent);

    //                var tileBehaviour = tile.GetComponent<TileBehaviour>();
    //                var view = tile.GetComponent<PhotonView>();

    //                if (tileBehaviour != null && view != null)
    //                {
    //                    view.RPC("SetCoordinates", RpcTarget.AllBuffered, x, z);
    //                    tileDictionary[(x, z)] = tileBehaviour;

    //                    if (tileInfos.TryGetValue((x, z), out var info))
    //                    {
    //                        view.RPC("RPC_SetTileState", RpcTarget.AllBuffered, (int)info.state, (int)info.access);
    //                    }
    //                    else
    //                    {
    //                        view.RPC("RPC_SetTileState", RpcTarget.AllBuffered, (int)TileState.None, (int)TileAccessType.Everyone);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    StartCoroutine(DelayedSpawnPathTracer());
    //    TileChecker tileChecker = FindFirstObjectByType<TileChecker>();
    //    if (tileChecker != null)
    //    {
    //        tileChecker.HideNoneTiles();
    //        tileChecker.photonView.RPC(nameof(TileChecker.RPC_HideNoneTiles), RpcTarget.AllBuffered);
    //        Debug.Log("[GridManager] TileChecker가 None 상태 타일을 처리했습니다.");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[GridManager] TileChecker를 찾을 수 없습니다.");
    //    }
    //}
    private IEnumerator DelayedSpawnPathTracer()
    {
        // 최소한 1 프레임 기다림 (RPC 반영 대기)
        yield return new WaitForSeconds(3.0f);

        PathVisualizerSpawner.Instance?.SpawnPathTracer();
    }
    public TileBehaviour[,] GetTileArray()
    {
        int width = _tileContext.Width;
        int height = _tileContext.Height;
        TileBehaviour[,] tiles = new TileBehaviour[width, height];

        foreach (var kvp in tileDictionary)
        {
            tiles[kvp.Key.Item1, kvp.Key.Item2] = kvp.Value;
        }

        return tiles;
    }
    public int GetWidth() => _tileContext.Width;
    public int GetHeight() => _tileContext.Height;
    public void RegisterTile(TileBehaviour tile)
    {
        tileDictionary[(tile.CoordX, tile.CoordZ)] = tile;
    }
}
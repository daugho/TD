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
    [SerializeField] private CameraTargetSetter cameraTargetSetter;

    private Dictionary<(int, int), TileBehaviour> tileDictionary = new();
    public int GetWidth() => _tileContext.Width;
    public int GetHeight() => _tileContext.Height;
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
                    string prefabName = "Tile"; // �⺻ ������ �̸�

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
            Debug.Log("[GridManager] TileChecker�� None ���� Ÿ���� ó���߽��ϴ�.");
        }
        else
        {
            Debug.LogWarning("[GridManager] TileChecker�� ã�� �� �����ϴ�.");
        }
        cameraTargetSetter.StartSetCameraTargetDelayed(1.0f);
    }
    private IEnumerator DelayedSpawnPathTracer()
    {
        // �ּ��� 1 ������ ��ٸ� (RPC �ݿ� ���)
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

    public void RegisterTile(TileBehaviour tile)
    {
        tileDictionary[(tile.CoordX, tile.CoordZ)] = tile;
    }
    public TileBehaviour GetCenterTile()
    {
        int centerX = _tileContext.Width / 2;
        int centerZ = _tileContext.Height / 2;

        if (tileDictionary.TryGetValue((centerX, centerZ), out var tile))
            return tile;

        Debug.LogWarning($"[GridManager] �߽� Ÿ�� ({centerX}, {centerZ})�� ã�� ���߽��ϴ�.");
        return null;
    }
}
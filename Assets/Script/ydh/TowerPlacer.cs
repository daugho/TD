using UnityEngine;
using Photon.Pun;

public class TowerPlacer : MonoBehaviour
{
    [SerializeField] private GameObject _towerPrefab;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTower();
        }
    }
    private void TryPlaceTower()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
            if (tile == null || tile._tileState != TileState.Installable) return;

            bool isMasterClient = PhotonNetwork.IsMasterClient;
            bool hasAccess = tile._accessType switch
            {
                TileAccessType.Everyone => true,
                TileAccessType.MasterOnly => isMasterClient,
                TileAccessType.ClientOnly => !isMasterClient,
                _ => false
            };
            if (!hasAccess) return;

            // ? 1. 상태 임시 저장 후 설치불가로 변경
            TileState originalState = tile._tileState;
            TileAccessType originalAccess = tile._accessType;
            tile.SetTileState(TileState.Uninstallable); // 로컬 임시 차단

            // ? 2. 경로 확인
            bool pathExists = IsPathAvailable();

            // ? 3. 원상 복구
            tile.SetTileState(originalState, originalAccess);

            if (!pathExists)
            {
                Debug.Log("? 경로가 막혀 설치할 수 없습니다.");
                return;
            }

            // ? 4. 경로가 있으면 진짜 설치
            Vector3 spawnPos = tile.transform.position + Vector3.up * 0.5f;
            PhotonNetwork.Instantiate("TestTower", spawnPos, Quaternion.identity);
            tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
                (int)TileState.Installed, (int)tile._accessType);
        }
    }

    private bool IsPathAvailable()
    {
        var tileContext = GameObject.Find("GridSystem")?.GetComponent<TileContext>();
        if (tileContext == null) return false;

        int width = tileContext.Width;
        int height = tileContext.Height;
        TileBehaviour[,] tiles = new TileBehaviour[width, height];

        foreach (Transform t in tileContext.TileParent)
        {
            var tb = t.GetComponent<TileBehaviour>();
            if (tb == null) continue;

            string[] parts = tb.name.Split('_');
            int x = int.Parse(parts[1]);
            int z = int.Parse(parts[2]);
            if (tb._tileState != TileState.Installable &&
                tb._tileState != TileState.StartPoint &&
                tb._tileState != TileState.EndPoint)
            {
                Debug.Log($"? 타일 차단 상태: {tb.name}, 상태: {tb._tileState}");
            }
        }

        // Start/End 탐색
        Vector2Int? start = null, end = null;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var tile = tiles[x, z];
                if (tile == null) continue;
                if (tile._tileState == TileState.StartPoint) start = new(x, z);
                if (tile._tileState == TileState.EndPoint) end = new(x, z);
            }
        }

        if (start == null || end == null) return false;

        var pathfinder = new AStarPathfinder(tiles, width, height);
        var path = pathfinder.FindPath(start.Value, end.Value);
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("몬스터의 이동 경로가 막혀서 설치가 불가능합니다.");
        }
        return path != null;
    }
    //private void TryPlaceTower()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (!Physics.Raycast(ray, out RaycastHit hit)) return;

    //    TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
    //    if (tile == null) return;

    //    // 이미 설치된 타일은 무시
    //    if (tile._tileState == TileState.Uninstallable)
    //    {
    //        Debug.Log("? 이미 설치된 타일입니다.");
    //        return;
    //    }

    //    // 설치 가능한 상태인지 확인
    //    bool isInstallable = tile._tileState == TileState.Installable ||
    //                         tile._tileState == TileState.MasterInstallable ||
    //                         tile._tileState == TileState.ClientInstallable;

    //    if (!isInstallable)
    //    {
    //        Debug.Log("? 설치 불가능한 상태입니다.");
    //        return;
    //    }

    //    // 권한 확인
    //    bool isMaster = PhotonNetwork.IsMasterClient;
    //    bool hasAccess = tile._accessType switch
    //    {
    //        TileAccessType.Everyone => true,
    //        TileAccessType.MasterOnly => isMaster,
    //        TileAccessType.ClientOnly => !isMaster,
    //        _ => false
    //    };

    //    if (!hasAccess)
    //    {
    //        Debug.Log("? 타일에 대한 설치 권한이 없습니다.");
    //        return;
    //    }

    //    // 설치 진행
    //    Vector3 spawnPos = tile.transform.position + Vector3.up * 0.5f;
    //    PhotonNetwork.Instantiate(_towerPrefab.name, spawnPos, Quaternion.identity);

    //    // 타일 상태를 'Installed'로 변경 (모든 클라이언트 동기화)
    //    tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
    //        (int)TileState.Installed, (int)tile._accessType);
    //}
}
//타일이나 씬에 배치된 고정 오브젝트는 IsMine == false일 수 있음

//그 이유는 PhotonNetwork.Instantiate()로 생성하지 않고, 마스터 클라이언트가 만든 공유 오브젝트이기 때문

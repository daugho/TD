using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TowerPlacer : MonoBehaviour
{
    [SerializeField] private GameObject _towerPrefab;
    [SerializeField] private Button _activateModeButton;
    private TileContext tileContext;

    private void Start()
    {
        tileContext = GameObject.Find("GridSystem")?.GetComponent<TileContext>();
    }

    private void OnEnable()
    {
        InputManager.Instance.OnTowerBuildClick += HandleTowerPlacementClick;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnTowerBuildClick -= HandleTowerPlacementClick;
    }

    private void HandleTowerPlacementClick(Vector3 clickPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
            if (tile == null || tile._tileState != TileState.Installable) return;

            // 권한 확인
            bool isMasterClient = PhotonNetwork.IsMasterClient;
            bool hasAccess = tile._accessType switch
            {
                TileAccessType.Everyone => true,
                TileAccessType.MasterOnly => isMasterClient,
                TileAccessType.ClientOnly => !isMasterClient,
                _ => false
            };
            if (!hasAccess) return;

            // 설치 전 임시 차단 후 경로 확인
            TileState originalState = tile._tileState;
            TileAccessType originalAccess = tile._accessType;
            tile.SetTileState(TileState.Uninstallable);

            bool pathValid = PathChecker.IsPathAvailable(tileContext);

            tile.SetTileState(originalState, originalAccess); // 복원

            if (!pathValid)
            {
                Debug.Log("? 경로가 막혀 설치할 수 없습니다.");
                return;
            }

            // 설치 수행
            Vector3 spawnPos = tile.transform.position + Vector3.up * 0.5f;
            PhotonNetwork.Instantiate("TestTower", spawnPos, Quaternion.identity);

            tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
                (int)TileState.Installed, (int)tile._accessType);

            InputManager.Instance.ResetClickMode();
        }
    }
}


//using UnityEngine;
//using Photon.Pun;

//public class TowerPlacer : MonoBehaviour
//{
//    private void Update()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            TryPlaceTower();
//        }
//    }
//    private void TryPlaceTower()
//    {
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        if (Physics.Raycast(ray, out RaycastHit hit))
//        {
//            TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
//            if (tile == null || tile._tileState != TileState.Installable) return;

//            bool isMasterClient = PhotonNetwork.IsMasterClient;
//            bool hasAccess = tile._accessType switch
//            {
//                TileAccessType.Everyone => true,
//                TileAccessType.MasterOnly => isMasterClient,
//                TileAccessType.ClientOnly => !isMasterClient,
//                _ => false
//            };
//            if (!hasAccess) return;

//            // ? 1. 상태 임시 저장 후 설치불가로 변경
//            TileState originalState = tile._tileState;
//            TileAccessType originalAccess = tile._accessType;
//            tile.SetTileState(TileState.Uninstallable); // 로컬 임시 차단

//            // ? 2. 경로 확인
//            bool pathExists = IsPathAvailable();

//            // ? 3. 원상 복구
//            tile.SetTileState(originalState, originalAccess);

//            if (!pathExists)
//            {
//                Debug.Log("? 경로가 막혀 설치할 수 없습니다.");
//                return;
//            }

//            // ? 4. 경로가 있으면 진짜 설치
//            Vector3 spawnPos = tile.transform.position + Vector3.up * 0.5f;

//            GameObject towerPrefab = PhotonNetwork.Instantiate("TestTower", spawnPos, Quaternion.identity);
//            //TowerTypes towerTypes = TowerTypes.FlameTower;
//            //PhotonView view = towerPrefab.GetComponent<PhotonView>();
//            //view.RPC("OnBuildComplete", RpcTarget.AllBuffered, (int)towerTypes);


//            tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
//                (int)TileState.Installed, (int)tile._accessType);
//        }
//    }

//    private bool IsPathAvailable()
//    {
//        var tileContext = GameObject.Find("GridSystem")?.GetComponent<TileContext>();
//        if (tileContext == null) return false;

//        int width = tileContext.Width;
//        int height = tileContext.Height;
//        TileBehaviour[,] tiles = new TileBehaviour[width, height];

//        foreach (Transform t in tileContext.TileParent)
//        {
//            var tb = t.GetComponent<TileBehaviour>();
//            if (tb == null) continue;

//            string[] parts = tb.name.Split('_');
//            int x = int.Parse(parts[1]);
//            int z = int.Parse(parts[2]);

//            tiles[x, z] = tb;

//            if (tb._tileState != TileState.Installable &&
//                tb._tileState != TileState.StartPoint &&
//                tb._tileState != TileState.EndPoint)
//            {
//                //Debug.Log($"? 타일 차단 상태: {tb.name}, 상태: {tb._tileState}");
//            }
//        }

//        // Start/End 탐색
//        Vector2Int? start = null, end = null;

//        bool isMaster = PhotonNetwork.IsMasterClient;

//        for (int x = 0; x < width; x++)
//        {
//            for (int z = 0; z < height; z++)
//            {
//                var tile = tiles[x, z];
//                if (tile == null) continue;

//                // ?? START POINT: 권한 필터링
//                if (tile._tileState == TileState.StartPoint)
//                {
//                    switch (tile._accessType)
//                    {
//                        case TileAccessType.Everyone:
//                            start = new(x, z);
//                            break;

//                        case TileAccessType.MasterOnly:
//                            if (isMaster) start = new(x, z);
//                            break;

//                        case TileAccessType.ClientOnly:
//                            if (!isMaster) start = new(x, z);
//                            break;
//                    }
//                }

//                // ?? END POINT: 그대로 사용 (필요 시 여기도 권한 나눌 수 있음)
//                if (tile._tileState == TileState.EndPoint)
//                {
//                    end = new(x, z);
//                }
//            }
//        }
//        if (start != null)
//        {
//            Debug.Log($"?? StartTile = Tile_{start.Value.x}_{start.Value.y}");
//        }
//        if (end != null)
//        {
//            Debug.Log($"?? EndTile = Tile_{end.Value.x}_{end.Value.y}");
//        }
//        if (start == null || end == null) return false;

//        var pathfinder = new AStarPathfinder(tiles, width, height);
//        var path = pathfinder.FindPath(start.Value, end.Value);
//        if (path == null || path.Count == 0)
//        {
//            Debug.LogWarning("몬스터의 이동 경로가 막혀서 설치가 불가능합니다.");
//        }
//        return path != null;
//    }
//}
////타일이나 씬에 배치된 고정 오브젝트는 IsMine == false일 수 있음

////그 이유는 PhotonNetwork.Instantiate()로 생성하지 않고, 마스터 클라이언트가 만든 공유 오브젝트이기 때문

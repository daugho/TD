using UnityEngine;
using Photon.Pun;

public class TowerPlacer : MonoBehaviour
{
    [SerializeField] private Turret _towerPrefab;

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

            // ? 1. ���� �ӽ� ���� �� ��ġ�Ұ��� ����
            TileState originalState = tile._tileState;
            TileAccessType originalAccess = tile._accessType;
            tile.SetTileState(TileState.Uninstallable); // ���� �ӽ� ����

            // ? 2. ��� Ȯ��
            bool pathExists = IsPathAvailable();

            // ? 3. ���� ����
            tile.SetTileState(originalState, originalAccess);

            if (!pathExists)
            {
                Debug.Log("? ��ΰ� ���� ��ġ�� �� �����ϴ�.");
                return;
            }

            // ? 4. ��ΰ� ������ ��¥ ��ġ
            Vector3 spawnPos = tile.transform.position + Vector3.up * 0.5f;
            
            GameObject towerPrefab = PhotonNetwork.Instantiate("Prefabs/Turrets/FlameTower_Master", spawnPos, Quaternion.identity);
            TowerTypes towerTypes = TowerTypes.FlameTower;
            PhotonView view = towerPrefab.GetComponent<PhotonView>();
            view.RPC("OnBuildComplete", RpcTarget.AllBuffered, (int)towerTypes);
            
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

            tiles[x, z] = tb;

            if (tb._tileState != TileState.Installable &&
                tb._tileState != TileState.StartPoint &&
                tb._tileState != TileState.EndPoint)
            {
                Debug.Log($"? Ÿ�� ���� ����: {tb.name}, ����: {tb._tileState}");
            }
        }

        // Start/End Ž��
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
            Debug.LogWarning("������ �̵� ��ΰ� ������ ��ġ�� �Ұ����մϴ�.");
        }
        return path != null;
    }
    //private void TryPlaceTower()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (!Physics.Raycast(ray, out RaycastHit hit)) return;

    //    TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
    //    if (tile == null) return;

    //    // �̹� ��ġ�� Ÿ���� ����
    //    if (tile._tileState == TileState.Uninstallable)
    //    {
    //        Debug.Log("? �̹� ��ġ�� Ÿ���Դϴ�.");
    //        return;
    //    }

    //    // ��ġ ������ �������� Ȯ��
    //    bool isInstallable = tile._tileState == TileState.Installable ||
    //                         tile._tileState == TileState.MasterInstallable ||
    //                         tile._tileState == TileState.ClientInstallable;

    //    if (!isInstallable)
    //    {
    //        Debug.Log("? ��ġ �Ұ����� �����Դϴ�.");
    //        return;
    //    }

    //    // ���� Ȯ��
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
    //        Debug.Log("? Ÿ�Ͽ� ���� ��ġ ������ �����ϴ�.");
    //        return;
    //    }

    //    // ��ġ ����
    //    Vector3 spawnPos = tile.transform.position + Vector3.up * 0.5f;
    //    PhotonNetwork.Instantiate(_towerPrefab.name, spawnPos, Quaternion.identity);

    //    // Ÿ�� ���¸� 'Installed'�� ���� (��� Ŭ���̾�Ʈ ����ȭ)
    //    tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
    //        (int)TileState.Installed, (int)tile._accessType);
    //}
}
//Ÿ���̳� ���� ��ġ�� ���� ������Ʈ�� IsMine == false�� �� ����

//�� ������ PhotonNetwork.Instantiate()�� �������� �ʰ�, ������ Ŭ���̾�Ʈ�� ���� ���� ������Ʈ�̱� ����

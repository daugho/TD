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
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
        if (tile == null) return;

        // 이미 설치된 타일은 무시
        if (tile._tileState == TileState.Uninstallable)
        {
            Debug.Log("? 이미 설치된 타일입니다.");
            return;
        }

        // 설치 가능한 상태인지 확인
        bool isInstallable = tile._tileState == TileState.Installable ||
                             tile._tileState == TileState.MasterInstallable ||
                             tile._tileState == TileState.ClientInstallable;

        if (!isInstallable)
        {
            Debug.Log("? 설치 불가능한 상태입니다.");
            return;
        }

        // 권한 확인
        bool isMaster = PhotonNetwork.IsMasterClient;
        bool hasAccess = tile._accessType switch
        {
            TileAccessType.Everyone => true,
            TileAccessType.MasterOnly => isMaster,
            TileAccessType.ClientOnly => !isMaster,
            _ => false
        };

        if (!hasAccess)
        {
            Debug.Log("? 타일에 대한 설치 권한이 없습니다.");
            return;
        }

        // 설치 진행
        Vector3 spawnPos = tile.transform.position + Vector3.up * 0.5f;
        PhotonNetwork.Instantiate(_towerPrefab.name, spawnPos, Quaternion.identity);

        // 타일 상태를 'Installed'로 변경 (모든 클라이언트 동기화)
        tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
            (int)TileState.Installed, (int)tile._accessType);
    }
}
//타일이나 씬에 배치된 고정 오브젝트는 IsMine == false일 수 있음

//그 이유는 PhotonNetwork.Instantiate()로 생성하지 않고, 마스터 클라이언트가 만든 공유 오브젝트이기 때문

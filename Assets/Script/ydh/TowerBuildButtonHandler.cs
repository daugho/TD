using Photon.Pun;
using UnityEngine;

public class TowerBuildButtonHandler : MonoBehaviour
{
    private bool _isClickBtn = false;
    private GameObject _activeTurret;
    private GameObject currentPreviewTile;

    private void Update()
    {
        if (InputManager.Instance.CurrentMode != ClickMode.TileReveal) return;

        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
            if (tile == null) return;

            Vector3 targetPos = tile.transform.position;

            _activeTurret.transform.position = targetPos + Vector3.up * 0.01f;

            if (Input.GetMouseButtonDown(0))
            {
                bool isMaster = PhotonNetwork.IsMasterClient;
                TileAccessType access = isMaster ? TileAccessType.MasterOnly : TileAccessType.ClientOnly;

                tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
                    (int)TileState.Installable, (int)access);

                Collider col = tile.GetComponent<Collider>();
                if (col != null) col.enabled = true;

                InputManager.Instance.ResetClickMode(); // 설치 후 모드 종료
            }
            
        }
    }
    public void OnTowerBuildButtonClicked()
    {
        TowerTypes[] allTypes = (TowerTypes[])System.Enum.GetValues(typeof(TowerTypes));
        TowerTypes type = allTypes[Random.Range(0, allTypes.Length)];

        GameObject turret = TurretManager.Instance.GetAvailableTurret(type);
        turret.SetActive(true);

        _activeTurret = turret;

        _isClickBtn = true;

        InputManager.Instance.SetClickMode(ClickMode.TowerBuild);
        Debug.Log("[UI] 타워 설치 모드 활성화됨");
    }

}

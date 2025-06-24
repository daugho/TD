using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TowerBuildButtonHandler : MonoBehaviour
{
    private TileContext _tileContext;
    public bool IsClickBtn = false;
    private GameObject _activeTurret;
    private GameObject currentPreviewTile;
    private TowerTypes _curType;
    public UnityAction OnBuildEvent;
    private GameObject _btnObject;
   

    private void Awake()
    {
        _tileContext = GetComponent<TileContext>();
    }
    private void Update()
    {
        if (!IsClickBtn) return;

        Vector3 mousePos = Input.mousePosition;
       
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
        {
            TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();

            float tileHeight = hit.collider.bounds.size.y;
            Vector3 targetPos = hit.collider.transform.position + Vector3.up * (tileHeight * 0.5f);

            _activeTurret.transform.position = targetPos ;

            if (Input.GetMouseButtonDown(0))
            {
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
                
                TileState originalState = tile._tileState;
                TileAccessType originalAccess = tile._accessType;
                tile.SetTileState(TileState.Uninstallable);
                
                bool pathValid = PathChecker.IsPathAvailable(_tileContext);
                
                tile.SetTileState(originalState, originalAccess);
                
                if (!pathValid)
                {
                    Debug.Log("? 경로가 막혀 설치할 수 없습니다.");
                    return;
                }

                Turret turret = _activeTurret.GetComponent<Turret>();
             
                PhotonView turretView = turret.GetComponent<PhotonView>();
                turretView.RPC("ActivateTurret", RpcTarget.AllBuffered, true);
                turretView.RPC("SetTurretPosition", RpcTarget.AllBuffered, targetPos);

                turret.SetStateWhenBuildTurret();
                _activeTurret = null;

                IsClickBtn = false;

                tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
                (int)TileState.Installed, (int)tile._accessType);

                InputManager.Instance.ResetClickMode();

                _btnObject.SetActive(false);
                _btnObject = null;
            }
        }
        else
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
            _activeTurret.transform.position = worldPos;
        }
    }

    public void OnTowerBuildButtonClicked(TowerTypes type, GameObject btn)
    {
        GameObject turret = TurretManager.Instance.GetAvailableTurret(type);
        turret.SetActive(true);

        _activeTurret = turret;
        _curType = type;

        IsClickBtn = true;

        _btnObject = btn;

        InputManager.Instance.SetClickMode(ClickMode.TowerBuild);
        Debug.Log("[UI] 타워 설치 모드 활성화됨");

        OnBuildEvent?.Invoke();
    }
}

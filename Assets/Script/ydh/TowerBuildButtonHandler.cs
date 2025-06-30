using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif

public class TowerBuildButtonHandler : MonoBehaviour
{
    [SerializeField] private GachaSystem _gachaSystem;
    private TileContext _tileContext;
    public bool IsClickBtn = false;
    private GameObject _activeTurret;
    private TowerTypes _curType;
    public UnityAction OnBuildEvent;
    private GameObject _btnObject;

#if UNITY_ANDROID || UNITY_IOS
    private void OnEnable() => EnhancedTouchSupport.Enable();
    private void OnDisable() => EnhancedTouchSupport.Disable();
#endif

    private void Awake()
    {
        _tileContext = GetComponent<TileContext>();
    }

    private void Update()
    {
        if (!IsClickBtn) return;

        Vector2 screenPos;

#if UNITY_EDITOR || UNITY_STANDALONE
        screenPos = Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IOS
        var touches = Touch.activeTouches;
        if (touches.Count == 0) return;

        var touch = touches[0];
        screenPos = touch.screenPosition;

        if (touch.phase != TouchPhase.Began && touch.phase != TouchPhase.Moved) return;
#endif

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
        {
            TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
            float tileHeight = hit.collider.bounds.size.y;
            Vector3 targetPos = hit.collider.transform.position + Vector3.up * (tileHeight * 0.5f);
            _activeTurret.transform.position = targetPos;

#if UNITY_EDITOR || UNITY_STANDALONE
            bool isTouchConfirmed = Input.GetMouseButtonDown(0);
#elif UNITY_ANDROID || UNITY_IOS
            bool isTouchConfirmed = touch.phase == TouchPhase.Began;
#endif

            if (isTouchConfirmed)
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
                if (!hasAccess)
                {
                    ResetTowerClickHandler();
                    return;
                }

                TileState originalState = tile._tileState;
                TileAccessType originalAccess = tile._accessType;
                tile.SetTileState(TileState.Uninstallable);

                bool pathValid = PathChecker.IsPathAvailable(_tileContext);
                tile.SetTileState(originalState, originalAccess);

                if (!pathValid)
                {
                    ResetTowerClickHandler();
                    return;
                }

                Turret turret = _activeTurret.GetComponent<Turret>();
                PhotonView turretView = turret.GetComponent<PhotonView>();
                turretView.RPC("ActivateTurret", RpcTarget.AllBuffered, true);
                turretView.RPC("SetTurretPosition", RpcTarget.AllBuffered, targetPos);
                turret.SetStateWhenBuildTurret();
                turret.SetMyTile(tile);

                tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
                    (int)TileState.Installed, (int)tile._accessType);

                _activeTurret = null;
                IsClickBtn = false;
                InputManager.Instance.ResetClickMode();
                _btnObject.SetActive(false);
                _btnObject = null;
                _gachaSystem.RefreshBtnCount();
                GameResultData.Instance.AddTowerBuilt();
            }
        }
        else
        {
            // fallback 위치 (빈 공간 터치 시)
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
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

    private void ResetTowerClickHandler()
    {
        _activeTurret.SetActive(false);
        _activeTurret = null;
        IsClickBtn = false;
        InputManager.Instance.SetClickMode(ClickMode.None);
    }
}

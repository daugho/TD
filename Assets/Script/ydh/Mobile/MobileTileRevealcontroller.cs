using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class MobileTileRevealcontroller : MonoBehaviour
{
    [SerializeField] private TileContext _tileContext;
    [SerializeField] private Button _activateModeButton;
    [SerializeField] private MobileTilePrevController previewController;

    private Camera _mainCamera;
    private bool isStructMode = false;
    public bool IsStructMode => isStructMode;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        _activateModeButton.onClick.AddListener(() =>
        {
            isStructMode = true;
            InputManager.Instance.SetClickMode(ClickMode.TileReveal);
        });
    }

    private void Update()
    {
        if (!isStructMode) return;

        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        if (touches.Count == 0) return;

        var touch = touches[0];
        if (touch.phase != TouchPhase.Began) return;

        Vector2 screenPos = touch.screenPosition;
        Ray ray = _mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
            if (tile == null) return;

            var renderer = tile.GetComponent<Renderer>();
            var collider = tile.GetComponent<Collider>();

            if (renderer != null && !renderer.enabled)
            {
                bool isMaster = PhotonNetwork.IsMasterClient;
                TileAccessType access = isMaster ? TileAccessType.MasterOnly : TileAccessType.ClientOnly;

                tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
                    (int)TileState.Installable, (int)access);

                tile.photonView.RPC(nameof(TileBehaviour.RPC_RevealTile), RpcTarget.AllBuffered);

                renderer.enabled = true;
                if (collider != null) collider.enabled = true;

                Debug.Log($"[TileRevealController] 타일 복원 완료: {tile.name}, 권한: {access}");
                tile.PlayRevealEffect();
                previewController.HidePreview();

                // StructMode 종료 및 이벤트 초기화
                isStructMode = false;
                InputManager.Instance.ResetClickMode();
            }
        }
    }
}

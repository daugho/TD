using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TileRevealcontroller : MonoBehaviourPun
{
    [SerializeField] private TileContext _tileContext;
    [SerializeField] private Button _activateModeButton;
    [SerializeField] private TilePreviewController previewController;

    private Camera _mainCamera;
    private bool isStructMode = false;
    public bool IsStructMode => isStructMode;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _activateModeButton.onClick.AddListener(() =>
        {
            isStructMode = true;
            InputManager.Instance.SetClickMode(ClickMode.TileReveal);
            InputManager.Instance.OnTileRevealClick += HandleWorldClick;
        });
    }

    private void HandleWorldClick(Vector3 clickPosition)
    {
        if (!isStructMode) return;

        Ray ray = _mainCamera.ScreenPointToRay(clickPosition);
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
                //  StructMode 종료
                isStructMode = false;

                //  이벤트 해제 (한 번만 클릭되도록)
                InputManager.Instance.OnTileRevealClick -= HandleWorldClick;
                InputManager.Instance.ResetClickMode();
            }
        }
    }
}

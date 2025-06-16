using INab.Common;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TileRevealcontroller : MonoBehaviour
{
    [SerializeField] private TileContext _tileContext;
    [SerializeField] private Button _activateModeButton;

    private Camera _mainCamera;
    private bool isStructMode = false;

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

                renderer.enabled = true;
                if (collider != null) collider.enabled = true;

                Debug.Log($"[TileRevealController] Ÿ�� ���� �Ϸ�: {tile.name}, ����: {access}");
                tile.PlayRevealEffect();
                //  StructMode ����
                isStructMode = false;

                //  �̺�Ʈ ���� (�� ���� Ŭ���ǵ���)
                InputManager.Instance.OnTileRevealClick -= HandleWorldClick;
                InputManager.Instance.ResetClickMode();
            }
        }
    }
}

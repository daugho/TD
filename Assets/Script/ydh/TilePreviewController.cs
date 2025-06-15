using UnityEngine;
using Photon.Pun;

public class TilePreviewController : MonoBehaviour
{
    [SerializeField] private GameObject previewTilePrefab;
    [SerializeField] private TileContext tileContext;

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

            var renderer = tile.GetComponent<Renderer>();
            if (renderer != null && !renderer.enabled)
            {
                Vector3 targetPos = tile.transform.position;

                if (currentPreviewTile == null)
                {
                    currentPreviewTile = Instantiate(previewTilePrefab);
                }

                currentPreviewTile.transform.position = targetPos + Vector3.up * 0.01f;

                if (Input.GetMouseButtonDown(0))
                {
                    // 실제 설치 처리 (RPC 또는 RevealController에 위임)
                    bool isMaster = PhotonNetwork.IsMasterClient;
                    TileAccessType access = isMaster ? TileAccessType.MasterOnly : TileAccessType.ClientOnly;

                    tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
                        (int)TileState.Installable, (int)access);

                    renderer.enabled = true;
                    Collider col = tile.GetComponent<Collider>();
                    if (col != null) col.enabled = true;

                    Destroy(currentPreviewTile);
                    currentPreviewTile = null;

                    InputManager.Instance.ResetClickMode(); // 설치 후 모드 종료
                }
            }
            else
            {
                if (currentPreviewTile != null)
                    currentPreviewTile.SetActive(false);
            }
        }
        else
        {
            if (currentPreviewTile != null)
                currentPreviewTile.SetActive(false);
        }
    }
}

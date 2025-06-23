using UnityEngine;
using Photon.Pun;

public class TilePreviewController : MonoBehaviour
{
    [SerializeField] private GameObject previewTilePrefab;
    [SerializeField] private TileContext tileContext;
    [SerializeField] private TileRevealcontroller revealController;
    private GameObject currentPreviewTile;

    private void Start()
    {
        if (previewTilePrefab != null)
        {
            currentPreviewTile = Instantiate(previewTilePrefab);
            currentPreviewTile.SetActive(false);
        }
        else
        {
            Debug.LogError("[TilePreviewController] previewTilePrefab이 비어 있습니다!");
        }
    }

    private void Update()
    {
        if (currentPreviewTile == null || !revealController.IsStructMode) return;
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
                currentPreviewTile.SetActive(true);
                Vector3 targetPos = tile.transform.position;
                currentPreviewTile.transform.position = targetPos + Vector3.up * 0.01f;
            }
            else
            {
                SafeHidePreview(); // 타일은 있지만 이미 활성화된 경우
            }
        }
        else
        {
            SafeHidePreview(); // 아무것도 안 맞았을 때
        }
    }

    private void SafeHidePreview()
    {
        if (currentPreviewTile != null && currentPreviewTile.TryGetComponent(out Renderer _))
        {
            currentPreviewTile.SetActive(false);
        }
    }

    public void ActivePreview()
    {
        if (currentPreviewTile != null && !currentPreviewTile.activeSelf)
        {
            currentPreviewTile.SetActive(true);
        }
    }

    public void HidePreview()
    {
        SafeHidePreview();
    }
}

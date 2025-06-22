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
        if (currentPreviewTile == null)
        {
            currentPreviewTile = Instantiate(previewTilePrefab);
        }
        currentPreviewTile.SetActive(false);
    }
    private void Update()
    {
        if (!revealController.IsStructMode) return;
        if (InputManager.Instance.CurrentMode != ClickMode.TileReveal) return;
        currentPreviewTile.SetActive(true);
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
                currentPreviewTile.transform.position = targetPos + Vector3.up * 0.01f;
            }
        }
        else
        {
            if (currentPreviewTile != null)
                currentPreviewTile.SetActive(false);
        }
    }
    public void ActivePreview()
    {
        if (currentPreviewTile != null)
        {
            currentPreviewTile.SetActive(true);
        }
    }
    public void HidePreview()
    {
        if (currentPreviewTile != null)
        {
            currentPreviewTile.SetActive(false);
        }
    }
}

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
            Debug.LogError("[TilePreviewController] previewTilePrefab�� ��� �ֽ��ϴ�!");
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
                SafeHidePreview(); // Ÿ���� ������ �̹� Ȱ��ȭ�� ���
            }
        }
        else
        {
            SafeHidePreview(); // �ƹ��͵� �� �¾��� ��
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

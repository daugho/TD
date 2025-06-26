using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TilePreviewController : MonoBehaviour
{
    [SerializeField] private GameObject previewTilePrefab;
    [SerializeField] private TileContext tileContext;
    [SerializeField] private TileRevealcontroller revealController;
    private GameObject currentPreviewTile;

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

        var touches = Touch.activeTouches;
        if (touches.Count == 0)
        {
            SafeHidePreview();
            return;
        }

        var touch = touches[0];
        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
        {
            Vector2 touchPos = touch.screenPosition;
            Ray ray = Camera.main.ScreenPointToRay(touchPos);

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
                    SafeHidePreview(); // 타일은 있지만 이미 보이는 경우
                }
            }
            else
            {
                SafeHidePreview(); // 아무것도 안 맞은 경우
            }
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

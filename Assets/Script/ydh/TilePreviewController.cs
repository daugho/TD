using UnityEngine;
using Photon.Pun;

#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
#endif

public class TilePreviewController : MonoBehaviour
{
    [SerializeField] private GameObject previewTilePrefab;
    [SerializeField] private TileContext tileContext;
    [SerializeField] private TileRevealcontroller revealController;
    private GameObject currentPreviewTile;

#if UNITY_ANDROID || UNITY_IOS
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
#endif

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

#if UNITY_EDITOR || UNITY_STANDALONE
        Vector3 mousePos = Input.mousePosition;
        HandleRay(mousePos);
#elif UNITY_ANDROID || UNITY_IOS
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
            HandleRay(touchPos);
        }
#endif
    }

    private void HandleRay(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

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
                SafeHidePreview(); // Ÿ���� ������ �̹� ���̴� ���
            }
        }
        else
        {
            SafeHidePreview(); // �ƹ��͵� �� ���� ���
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

using UnityEngine;
using UnityEngine.EventSystems;

public class TowerClickHandler : MonoBehaviour
{
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    private void Update()
    {
        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        if (touches.Count == 0) return;

        var touch = touches[0];
        Vector2 screenPos = touch.screenPosition;

        Ray ray = _mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                if (TowerUIManager.Instance == null)
                    return;

                if (TowerUIManager.Instance.IsTowerUIActiveFor(transform))
                {
                    TowerUIManager.Instance.HideUI();
                }
                else
                {
                    TowerUIManager.Instance.ShowUI(transform);
                }
            }
        }
    }
}

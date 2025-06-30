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
        Vector2 screenPos;

#if UNITY_EDITOR || UNITY_STANDALONE
        screenPos = Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IOS
        var touches = Touch.activeTouches;
        if (touches.Count == 0) return;

        var touch = touches[0];
        screenPos = touch.screenPosition;

        if (touch.phase != TouchPhase.Began && touch.phase != TouchPhase.Moved) return;
#endif
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

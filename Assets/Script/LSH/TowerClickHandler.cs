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
        Vector2 inputPosition = Vector2.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Began)
                return;

            inputPosition = touch.position;

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Input.mousePosition;

            if (EventSystem.current.IsPointerOverGameObject())
                return;
        }
        else
        {
            return; 
        }

        Ray ray = _mainCamera.ScreenPointToRay(inputPosition);

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

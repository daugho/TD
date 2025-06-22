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
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
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
}

using UnityEngine;

public class ObjectClickHandler : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 클릭
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ObjectInteractionUI interactionUI = hit.collider.GetComponent<ObjectInteractionUI>();
                if (interactionUI != null)
                {
                    interactionUI.OnObjectClick(); // 클릭된 오브젝트의 UI 호출
                }
            }
        }
    }
}

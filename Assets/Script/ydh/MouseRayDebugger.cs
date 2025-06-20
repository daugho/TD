using UnityEngine;

public class MouseRayDebugger : MonoBehaviour
{
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log($"[Raycast] Hit: {hit.collider.name}, Position: {hit.point}");
        }
    }
}

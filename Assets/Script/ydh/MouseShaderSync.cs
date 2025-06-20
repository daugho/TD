using UnityEngine;

public class MouseShaderSync : MonoBehaviour
{
    [SerializeField] private float range = 3f;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Shader.SetGlobalVector("_MouseWorldPos", hit.point);
            Shader.SetGlobalFloat("_Range", range);
        }
    }
}
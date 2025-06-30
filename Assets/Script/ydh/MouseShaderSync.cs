using UnityEngine;

public class MouseShaderSync : MonoBehaviour
{
    [SerializeField] private float range = 3f;

    void Update()
    {
        Vector3 screenPos = Vector3.zero;
        bool hasInput = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        screenPos = Input.mousePosition;
        hasInput = true;
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            screenPos = Input.GetTouch(0).position;
            hasInput = true;
        }
#endif

        if (!hasInput) return;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Shader.SetGlobalVector("_MouseWorldPos", hit.point);
            Shader.SetGlobalFloat("_Range", range);
        }
    }
}

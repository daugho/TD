using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchShaderSync : MonoBehaviour
{
    [SerializeField] private float range = 3f;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;

        if (touches.Count > 0)
        {
            var touch = touches[0]; // ù ��° ��ġ�� ����

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Vector2 screenPos = touch.screenPosition;
                Ray ray = Camera.main.ScreenPointToRay(screenPos);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Shader.SetGlobalVector("_MouseWorldPos", hit.point);
                    Shader.SetGlobalFloat("_Range", range);
                }
            }
        }
        else
        {
            // ��ġ�� ������ ��ġ �ʱ�ȭ (���ϴ� ���)
            Shader.SetGlobalVector("_MouseWorldPos", Vector3.negativeInfinity);
        }
    }
}

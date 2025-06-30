using UnityEngine;

#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
#endif

public class CameraController : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    private float currentZoom;
    private bool isDragging = false;
    private Vector2 prevTouchPosition;
    private float prevPinchDistance = 0f;

    public float rotationSpeed = 50f;
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

#if UNITY_ANDROID || UNITY_IOS
    private void OnEnable() => EnhancedTouchSupport.Enable();
    private void OnDisable() => EnhancedTouchSupport.Disable();
#endif

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("[CameraController] 타겟이 설정되지 않았습니다.");
            return;
        }

        offset = transform.position - target.position;
        currentZoom = offset.magnitude;
    }

    private void Update()
    {
        if (target == null) return;

        HandleRotation();
        HandleZoom();

        offset = offset.normalized * currentZoom;
        transform.position = target.position + offset;
        transform.LookAt(target);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        offset = transform.position - target.position;
        currentZoom = offset.magnitude;
    }

    private void HandleRotation()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // PC에서 방향키 회전
        if (Input.GetKey(KeyCode.LeftArrow))
            RotateAroundTarget(-1);
        if (Input.GetKey(KeyCode.RightArrow))
            RotateAroundTarget(1);
#elif UNITY_ANDROID || UNITY_IOS
        var touches = Touch.activeTouches;
        if (touches.Count == 1)
        {
            var touch = touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                prevTouchPosition = touch.screenPosition;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                float deltaX = touch.screenPosition.x - prevTouchPosition.x;
                float angle = deltaX * rotationSpeed * Time.deltaTime * 0.1f;

                Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
                offset = rotation * offset;
                transform.position = target.position + offset;
                transform.LookAt(target);

                prevTouchPosition = touch.screenPosition;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
#endif
    }

    private void HandleZoom()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // 마우스 휠 줌
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed * 10f;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
#elif UNITY_ANDROID || UNITY_IOS
        var touches = Touch.activeTouches;
        if (touches.Count == 2)
        {
            Vector2 pos0 = touches[0].screenPosition;
            Vector2 pos1 = touches[1].screenPosition;

            float curDist = Vector2.Distance(pos0, pos1);

            if (prevPinchDistance > 0)
            {
                float delta = prevPinchDistance - curDist;
                currentZoom += delta * zoomSpeed * 0.01f;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            }

            prevPinchDistance = curDist;
        }
        else
        {
            prevPinchDistance = 0f;
        }
#endif
    }

    public void RotateLeft() => RotateAroundTarget(-1);
    public void RotateRight() => RotateAroundTarget(1);

    private void RotateAroundTarget(int direction)
    {
        float angle = rotationSpeed * direction * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
        offset = rotation * offset;
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}

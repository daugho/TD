using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    private float currentZoom;

    public float rotationSpeed = 50f;
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

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
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        offset = transform.position - target.position;
        currentZoom = offset.magnitude;
    }

    private void HandleRotation()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftArrow)) RotateAroundTarget(-1);
        if (Input.GetKey(KeyCode.RightArrow)) RotateAroundTarget(1);
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

    private void HandleZoom()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;

            float prevDist = Vector2.Distance(prev0, prev1);
            float curDist = Vector2.Distance(t0.position, t1.position);
            float delta = prevDist - curDist;

            currentZoom += delta * zoomSpeed * 0.01f;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
#endif
        offset = offset.normalized * currentZoom;
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}

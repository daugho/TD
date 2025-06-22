using UnityEngine;

public class TurretRange : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField] private int _segmentCount = 60;
    [SerializeField] private float _yOffset = 0.1f;

    private float _flameAngle = 45f;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.loop = false;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.enabled = false;
    }

    public void ShowRangeCircle(Vector3 position, float range)
    {
        gameObject.SetActive(true);
        DrawRangeCircle(position + Vector3.up * _yOffset, range);
    }

    public void ShowRangeCone(Transform origin, float range)
    {
        gameObject.SetActive(true);
        Vector3 position = origin.position + Vector3.up * _yOffset;
        Vector3 forward = origin.forward;
        DrawRangeCone(position, forward, _flameAngle, range);
    }

    public void Hide()
    {
        _lineRenderer.enabled = false;
        gameObject.SetActive(false);
    }
    private void DrawRangeCircle(Vector3 center, float rangeRadius)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = _segmentCount + 1;

        for (int i = 0; i <= _segmentCount; i++)
        {
            float angle = (float)i / _segmentCount * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * rangeRadius;
            float z = Mathf.Sin(angle) * rangeRadius;

            _lineRenderer.SetPosition(i, center + new Vector3(x, 0f, z));
        }
    }

    private void DrawRangeCone(Vector3 origin, Vector3 forward, float angle, float range)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = _segmentCount + 3;  

        _lineRenderer.SetPosition(0, origin); 

        for (int i = 0; i <= _segmentCount; i++)
        {
            float currentAngle = -angle + (i * (2f * angle / _segmentCount));
            Quaternion rot = Quaternion.AngleAxis(currentAngle, Vector3.up);
            Vector3 dir = rot * forward;
            Vector3 point = origin + dir.normalized * range;
            _lineRenderer.SetPosition(i + 1, point);
        }

        _lineRenderer.SetPosition(_segmentCount + 2, origin);
    }
}

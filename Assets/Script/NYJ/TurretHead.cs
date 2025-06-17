using Photon.Pun;
using UnityEngine;

public enum TurretType
{
    Direct, Indirect
}
public class TurretHead : MonoBehaviour
{
    private Monster _target;
    private PhotonView _photonView;
    private TurretType _type;
     
    [SerializeField] private float _rotationSpeed = 5.0f;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (_target == null) return;

        Vector3 direction = _target.transform.position - transform.position;
        float yAngle = 0f;

        if (_type == TurretType.Indirect)
        {
            float dist = Vector3.Distance(transform.position, _target.transform.position);
            float t = Mathf.Clamp01(dist / 10.0f); // 거리에 따라 0~1로 정규화
            yAngle = Mathf.Lerp(0f, 5.0f, t); 
        }

        direction.y = yAngle;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
    }
    public void SetTarget(Monster target)
    {
        _target = target;
    }

    public void SetTurretType(TurretType type)
    {
        _type = type;
    }
    public Monster GetTarget()
    {
        return _target;
    }
}

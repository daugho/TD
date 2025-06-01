using UnityEngine;

public class TurretHead : MonoBehaviour
{
    private Monster _target;
    [SerializeField] private float _rotationSpeed = 5.0f;


    private void Update()
    {
        if (_target == null)
            return;

        Vector3 direction = _target.transform.position - transform.position;
        direction.y = 0f;

        if (direction == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
    }

    public void SetTarget(Monster target)
    {
        _target = target;
    }

    public Monster GetTarget()
    {
        return _target;
    }
}

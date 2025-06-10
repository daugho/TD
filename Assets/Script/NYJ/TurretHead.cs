using Photon.Pun;
using UnityEngine;

public class TurretHead : MonoBehaviour
{
    private Monster _target;
    private PhotonView _photonView;
    [SerializeField] private float _rotationSpeed = 5.0f;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (_target == null) return;

        Vector3 direction = _target.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

        _photonView.RPC("RPC_SetRotation", RpcTarget.Others, transform.rotation);
    }

    [PunRPC]
    public void RPC_SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
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

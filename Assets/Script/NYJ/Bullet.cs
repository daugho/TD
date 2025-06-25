using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _moveSpeed = 5.0f;
    protected int _atk; 
    private float _excuteRange = 0.5f;
    protected GameObject _explosionEffect;
    protected Monster _target;
    private float _hitThreshold = 0.1f;
    private PhotonView _targetView;
    private TowerTypes _type;

    private void Awake()
    {
    }
    protected virtual void Update()
    {
        if (!_target)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 dir = _target.transform.position - transform.position;
        dir.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = targetRotation;
        transform.position += dir * _moveSpeed * Time.deltaTime;

        if (Vector3.Distance(_target.transform.position, transform.position) <= _excuteRange)
        {
            ExecuteAttack();

            switch (_type)
            {
                case TowerTypes.RifleTower:
                    break;
                case TowerTypes.MachinegunTower:
                    break;
                case TowerTypes.FlameTower:
                    break;
                case TowerTypes.MissileTower:
                    SoundManager.Instance.PlaySFX("BombHitSound", 0.1f, true);
                    break;
                case TowerTypes.RailgunTower:
                    SoundManager.Instance.PlaySFX("RailgunHitSound", 0.1f, true);
                    break;
                case TowerTypes.GravityTower:
                    break;
                case TowerTypes.GrenadeTower:
                    SoundManager.Instance.PlaySFX("BombHitSound", 0.1f, true);
                    break;
                case TowerTypes.ElectricTower:
                    break;
                case TowerTypes.LaserTower:
                    break;
                default:
                    break;
            }
        }
    }

    protected virtual void ExecuteAttack()
    {
        if (_targetView != null)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

            _targetView.RPC(nameof(Monster.TakeDamage), RpcTarget.MasterClient, _atk, actorNumber);
        }


        Destroy(gameObject);
        
        GameObject explosionPrefab = Instantiate<GameObject>(_explosionEffect, transform.position, transform.rotation);
    }

    public void SetBullet(float speed, int atk, string hitEffectPath, TowerTypes type)
    {
        _moveSpeed = speed;
        _atk = atk;
        _type = type;
        _explosionEffect = Resources.Load<GameObject>("Prefabs/HitEffects/" + hitEffectPath);
    }
    public void SetBulletTarget(Monster target)
    {
        _target = target;
        _targetView = target.GetComponent<PhotonView>();
    }
}

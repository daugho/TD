using DigitalRuby.LightningBolt;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ElectricTurret : Turret
{
    [SerializeField] private float _chainRangeSqr = 5.0f;
    private GameObject _electricEffect;
  
    [PunRPC]
    protected override void RPC_SpawnBullet(Vector3 firePosition, Vector3 targetPosition)
    {
        if (_electricEffect == null)
        {
            _electricEffect = Resources.Load<GameObject>("Prefabs/FireEffects/" + MyTurretData.FireEffectPath);
        }

        if (_target == null) return;

        List<Monster> chainTargets = new List<Monster>();
        chainTargets.Add(_target);

        List<Monster> all = MonsterManager.Monsters;

        foreach (Monster m in all)
        {
            if (m == null || m == _target) continue;

            float distSqr = (m.transform.position - _target.transform.position).sqrMagnitude;
            if (distSqr <= _chainRangeSqr) 
            {
                chainTargets.Add(m);
                if (chainTargets.Count >= 3) break; 
            }
        }

        Vector3 firstTargetPos = chainTargets[0].transform.position;
        GameObject entryEffect = Instantiate(_electricEffect, transform);
        LightningBoltScript entryBolt = entryEffect.GetComponent<LightningBoltScript>();

        if (entryBolt != null)
        {
            entryBolt.StartObject = null;
            entryBolt.EndObject = null;
            entryBolt.StartPosition = firePosition;
            entryBolt.EndPosition = firstTargetPos;
        }
        Destroy(entryEffect, 0.5f);

        for (int i = 0; i < chainTargets.Count - 1; i++)
        {
            PhotonView targetView = chainTargets[i].GetComponent<PhotonView>();
            
            Vector3 start = chainTargets[i].transform.position;
            Vector3 end = chainTargets[i + 1].transform.position;

            GameObject effectInstance = Instantiate(_electricEffect, transform);
            LightningBoltScript bolt = effectInstance.GetComponent<LightningBoltScript>();
            if (bolt != null)
            {
                bolt.StartObject = null;
                bolt.EndObject = null;
                bolt.StartPosition = start;
                bolt.EndPosition = end;
            }

            if (targetView != null)
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                targetView.RPC(nameof(Monster.TakeDamage), RpcTarget.MasterClient, MyTurretData.Atk, actorNumber);
            }

            Destroy(effectInstance, 0.5f);
        }

        SoundManager.Instance.PlaySFX("ElectricSound", 0.05f, false);
    }
}

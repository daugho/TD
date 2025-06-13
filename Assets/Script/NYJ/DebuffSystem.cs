using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class DebuffSystem : MonoBehaviour
{
    [SerializeField] float _range = 5.0f;
    
    private Transform _origin;

    private List<Monster> _slowedMonsters = new List<Monster>();

    
    public void DebuffSlow(Transform originPos, float amount)
    {
        Vector3 origin = originPos.transform.position;
       
        Collider[] hits = Physics.OverlapSphere(origin, _range, LayerMask.GetMask("Monster"));

        foreach (Collider hit in hits)
        {
            Monster monster = hit.GetComponent<Monster>();
            PhotonView view = hit.GetComponent<PhotonView>();

            if (monster != null && view != null && !_slowedMonsters.Contains(monster))
            {
                view.RPC("TakeSlowDebuff", RpcTarget.AllBuffered, amount / 2);
                _slowedMonsters.Add(monster);
            }
        }
    }
}

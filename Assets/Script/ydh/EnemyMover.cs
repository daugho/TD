using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    private Queue<Vector3> _waypoints;
    private float _moveSpeed = 10f;
    private Rigidbody _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void SetPath(List<TileBehaviour> path)
    {
        _waypoints = new Queue<Vector3>();
        foreach (var tile in path)
        {
            _waypoints.Enqueue(tile.transform.position);
        }

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (_waypoints.Count > 0)
        {
            Vector3 tileCenter = _waypoints.Peek();
            Vector3 target = new Vector3(tileCenter.x, tileCenter.y + 1.0f, tileCenter.z);
            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                Vector3 nextPos = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _moveSpeed);
                _rb.MovePosition(nextPos); // ? 이걸로 위치 이동!
                yield return null;
            }
            _waypoints.Dequeue();
        }
    }
}

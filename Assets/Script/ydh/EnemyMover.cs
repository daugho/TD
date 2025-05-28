using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    private Queue<Vector3> _waypoints;
    private float _moveSpeed = 2f;

    public void SetPath(List<TileBehaviour> path)
    {
        _waypoints = new Queue<Vector3>();
        foreach (var tile in path)
        {
            _waypoints.Enqueue(tile.transform.position);
        }

        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (_waypoints.Count > 0)
        {
            Vector3 target = _waypoints.Dequeue();
            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _moveSpeed);
                yield return null;
            }
        }

        Debug.Log("적이 목표에 도착했습니다.");
    }
}

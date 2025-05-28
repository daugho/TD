using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float _movespeed = 3.0f;
    [SerializeField] private EnemyPath _enemyPath;
    private Waypoint[] _waypoints;
    private int _currentIndex = 0;

    private void Update()
    {
        Move();
    }
    public void Move()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;
        if (_currentIndex >= _waypoints.Length) return;

        Vector3 target = _enemyPath.transform.position + _waypoints[_currentIndex].Position;

        transform.position = Vector3.MoveTowards(transform.position, target, _movespeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            _currentIndex++;
            if (_currentIndex >= _waypoints.Length)
            {
                OnPathComplete();
            }
        }
    }

    public void Init(EnemyPath path, float speed)
    {
        _enemyPath = path;
        _waypoints = _enemyPath.GetWaypoints;
        _movespeed = speed;

        transform.position = _enemyPath.transform.position + _waypoints[0].Position;
        _currentIndex = 1;
    }

    private void OnPathComplete()
    {
        Debug.Log("Enemy reached the final waypoint.");
        Destroy(gameObject);
    }
}

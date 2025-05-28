using System;
using UnityEngine;
[Serializable]
public struct Waypoint
{
    public Vector3 Position;
    public Color Color;
}
public class EnemyPath : MonoBehaviour
{
    [SerializeField] private Waypoint[] _waypoints;
    public Waypoint[] GetWaypoints { get { return _waypoints; } }

    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private int _segments = 32;
    private void OnDrawGizmos()
    {
        foreach (Waypoint waypoint in _waypoints)
        {
            Gizmos.color = waypoint.Color;

            Vector3 center = waypoint.Position;
            float angleDelta = 360f / _segments;

            Vector3 prevPoint = center + new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * _radius;

            for (int i = 1; i <= _segments; i++)
            {
                float angle = angleDelta * i * Mathf.Deg2Rad;
                Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * _radius;

                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }
}
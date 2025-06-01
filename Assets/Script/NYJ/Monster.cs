using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Monster : MonoBehaviour
{
    [SerializeField] private float _movespeed = 3.0f;
    [SerializeField] private EnemyPath _enemyPath;

    private Waypoint[] _waypoints;
    private int _currentIndex = 0;
    private HPBar _hpSlider;
    private int _maxHP = 100;
    public int CurHp { get; set; }

    private void Update()
    {
        Move();
    }
    private void LateUpdate()
    {
        if (_hpSlider != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
            _hpSlider.transform.position = screenPos;
        }
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

    public void Init(EnemyPath path, float speed, HPBar hpBar)
    {
        _enemyPath = path;
        _waypoints = _enemyPath.GetWaypoints;
        _movespeed = speed;
        _hpSlider = hpBar;

        CurHp = _maxHP;
        hpBar.SetMaxHp(_maxHP); // 나중에 수정 

        transform.position = _enemyPath.transform.position + _waypoints[0].Position;
        _currentIndex = 1;
    }

    public void GetDamaged(int damageAmount)
    {
        if (CurHp <= 0)
        {
            gameObject.SetActive(false);
            _hpSlider.gameObject.SetActive(false);  
            return;
        }

        CurHp -= damageAmount;

        _hpSlider.SetHp(CurHp);
    }

    private void OnPathComplete()
    {
        Destroy(gameObject);
    }
}

using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Monster : MonoBehaviour
{
    [SerializeField] private EnemyPath _enemyPath;

    private Waypoint[] _waypoints;
    private int _currentIndex = 0;
    private HPBar _hpSlider;
    private MonsterData _monsterData;
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

        Vector3 target = _waypoints[_currentIndex].Position;
        
        Vector3 direction = target - transform.position;
        direction.y = 0f; 

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        transform.position = Vector3.MoveTowards(transform.position, target, _monsterData.MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            _currentIndex++;
            if (_currentIndex >= _waypoints.Length)
            {
                OnPathComplete();
            }
        }
    }

    public void Init(MonsterData monsterData, EnemyPath path, float speedMultiplier, float hpMultiplier, HPBar hpBar)
    {
        _monsterData = monsterData; 
        _enemyPath = path;
        _waypoints = _enemyPath.GetWaypoints;

        _hpSlider = hpBar;

        float MaxHp = monsterData.HP * hpMultiplier;    
       
        CurHp = monsterData.HP;
        _hpSlider.SetMaxHp(MaxHp); 
        _monsterData.MoveSpeed *= speedMultiplier;

        transform.position = _enemyPath.transform.position + _waypoints[0].Position;
        _currentIndex = 1;
    }

    public void GetDamaged(int damageAmount)
    {
        if (CurHp <= 0)
        { // 풀링때 변경 
            Destroy(gameObject);
            Destroy(_hpSlider);  
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

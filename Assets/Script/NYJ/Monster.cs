using JetBrains.Annotations;
using UnityEngine;
using Photon.Pun;
using static UnityEngine.GraphicsBuffer;

public class Monster : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private EnemyPath _enemyPath;

    private Waypoint[] _waypoints;
    private int _currentIndex = 0;
    private HPBar _hpSlider;
    private MonsterData _monsterData;
    private float _originSpeed;
    private bool _isDebuffed = false;
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

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instData = photonView.InstantiationData;
        if (instData == null) return;

        int monsterDataId = (int)instData[0];
        float speedMul = (float)instData[1];
        float hpMul = (float)instData[2];

        MonsterData monsterData = DataManager.Instance.GetMonsterData(monsterDataId);
        EnemyPath path = GameManager.Instance.GetMonsterPath();
        Transform monsterGuiCanvas = GameManager.Instance.GetMonsterCanvas();

        HPBar hpBar = Instantiate(Resources.Load<HPBar>("Prefabs/Monsters/HPBar"), monsterGuiCanvas);

        Init(monsterData, path, speedMul, hpMul, hpBar);

        MonsterManager.Monsters.Add(this);
    }
    public void Init(MonsterData monsterData, EnemyPath path, float speedMultiplier, float hpMultiplier, HPBar hpBar)
    {
        _monsterData = monsterData;
        _originSpeed = monsterData.MoveSpeed;
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

    [PunRPC]
    public void TakeDamage(int damageAmount)
    {
        if (CurHp <= 0)
        { // Ǯ���� ���� 
            Destroy(gameObject);
            Destroy(_hpSlider);  
            return;
        }

        CurHp -= damageAmount;

        Debug.Log(CurHp);

        _hpSlider.SetHp(CurHp);
    }

    [PunRPC]
    public void TakeSlowDebuff(float slowAmount)
    {
        if (_isDebuffed)
        {
            return;
        }

        _monsterData.MoveSpeed *= slowAmount;

        _isDebuffed = true;

        Invoke(nameof(ClearDebuffs), 4.0f);
    }

    public void ClearDebuffs()
    {
        _monsterData.MoveSpeed = _originSpeed;
        _isDebuffed = false;
    }

    private void OnPathComplete()
    {
        Destroy(gameObject);
    }
}

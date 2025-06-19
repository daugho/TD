using JetBrains.Annotations;
using UnityEngine;
using Photon.Pun;
using static UnityEngine.GraphicsBuffer;
using Photon.Pun.Demo.PunBasics;

public class Monster : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private PlayerGUI _playerGUI;

    private int _currentIndex = 0;
    private HPBar _hpSlider;
    private MonsterData _monsterData;
    private float _originSpeed;
    private bool _isDebuffed = false;
    public int CurHp { get; set; }

    private void Start()
    {
        GameObject guiObject = GameObject.Find("MainCanvas/Gold");
        if (guiObject != null)
        {
            _playerGUI = guiObject.GetComponent<PlayerGUI>();
        }
    }
    private void LateUpdate()
    {
        if (_hpSlider != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
            _hpSlider.transform.position = screenPos;
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
        Transform monsterGuiCanvas = GameManager.Instance.GetMonsterCanvas();

        HPBar hpBar = Instantiate(Resources.Load<HPBar>("Prefabs/Monsters/HPBar"), monsterGuiCanvas);

        Init(monsterData, speedMul, hpMul, hpBar);

        MonsterManager.Monsters.Add(this);
    }
    public void Init(MonsterData monsterData, float speedMultiplier, float hpMultiplier, HPBar hpBar)
    {
        _monsterData = monsterData;
        _originSpeed = monsterData.MoveSpeed;

        _hpSlider = hpBar;

        float MaxHp = monsterData.HP * hpMultiplier;    
       
        CurHp = monsterData.HP;
        _hpSlider.SetMaxHp(MaxHp); 
        _monsterData.MoveSpeed *= speedMultiplier;

        MonsterMover mover = GetComponent<MonsterMover>();
        mover.SetMonsterSpeed(_monsterData.MoveSpeed);   
    }

    [PunRPC]
    public void TakeDamage(int damageAmount)
    {
        if (CurHp <= 0)
        { // 풀링때 변경 
            _playerGUI.AddPlayerGold(_monsterData.MonsterReward);

            Destroy(gameObject);
            Destroy(_hpSlider);  
            return;
        }

        CurHp -= damageAmount;

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
}

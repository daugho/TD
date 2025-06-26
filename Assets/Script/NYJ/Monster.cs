using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Monster : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private PlayerGUI _playerGUI;

    private int _currentIndex = 0;
    private HPBar _hpSlider;
    private MonsterData _monsterData;
    private float _originSpeed;
    private bool _isDebuffed = false;
    private MonsterMover _mover;
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

        _mover = GetComponent<MonsterMover>();
        _mover.SetMonsterSpeed(_monsterData.MoveSpeed);   
    }

    [PunRPC]
    public void TakeDamage(int damageAmount, int attackerActorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        CurHp -= damageAmount;

        if (CurHp <= 0)
        {
            Photon.Realtime.Player attacker = PhotonNetwork.CurrentRoom.GetPlayer(attackerActorNumber);
            if (attacker != null)
            {
                photonView.RPC(nameof(GivePlayerGoldRPC), attacker, _monsterData.MonsterReward);
            }

            photonView.RPC(nameof(RemoveFromMonsterListRPC), RpcTarget.AllBuffered);

            PhotonNetwork.Destroy(gameObject);
            return;
        }
        
        photonView.RPC(nameof(SetHpBarRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RemoveFromMonsterListRPC()
    {
        if (MonsterManager.Monsters.Contains(this))
        {
            MonsterManager.Monsters.Remove(this);
        }

        if (_hpSlider != null)
        {
            Destroy(_hpSlider.gameObject);
        }
    }

    [PunRPC]
    public void DestroyMonsterRPC()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void GivePlayerGoldRPC(int goldAmount)
    {
        _playerGUI.AddPlayerGold(goldAmount);
       
        if (_hpSlider != null)
        {
            Destroy(_hpSlider);
        }
    }

    [PunRPC]
    public void SetHpBarRPC()
    {
        _hpSlider.SetHp(CurHp);
    }

    [PunRPC]
    public void RemoveHpSliderRPC()
    {
        if (_hpSlider != null)
        {
            Destroy(_hpSlider);
        }
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

        _mover.SetMonsterSpeed(_monsterData.MoveSpeed);

        Invoke(nameof(ClearDebuffs), 4.0f);
    }

    public void ClearDebuffs()
    {
        _monsterData.MoveSpeed = _originSpeed;
        _mover.SetMonsterSpeed(_monsterData.MoveSpeed);
        _isDebuffed = false;
    }
}

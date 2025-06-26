using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TowerUIManager : MonoBehaviour
{
    public static TowerUIManager Instance { get; private set; }

    [SerializeField]
    private GameObject _towerUI;

    [SerializeField] private TowerBuildButtonHandler _towerBuildButtonHandler;

    [SerializeField]
    private GameObject _towerAtkRangePrefab;
    private TurretRange _currentRangeIndicator;
    private TurretInfoUI _turretInfoUI;

    private Vector3 _offset = new Vector3(340.0f, 200.0f, 0f);

    private Turret _targetTower; 
    private bool _isTowerUIActive = false;

    [SerializeField] private Button _upgradeBtn;
    [SerializeField] private Button _sellBtn;

    private Sprite _maxUpgradeSprite;
    private Sprite _upgradeSprite;
    private Image _upgradeBtnImage;

    private int _maxLevel = 10;
    private int _upgradePrice = 50;
    private int _nextUpgradePrice = 25;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _upgradeSprite = Resources.Load<Sprite>("Prefabs/UI/LevelUpBtn");
        _maxUpgradeSprite = Resources.Load<Sprite>("Prefabs/UI/MaxLevelBtn");
        _upgradeBtnImage = _upgradeBtn.GetComponent<Image>();
    }

    public bool IsTowerUIActiveFor(Transform tower)
    {
        return _towerUI.activeSelf && _targetTower == tower;
    }

    public void ShowUI(Transform tower)
    {
        Turret turret = tower.GetComponent<Turret>();

        if (_towerUI == null || turret.SetTurretBuildEarly)
        {
            return;
        }
        _isTowerUIActive = true;

        _targetTower = turret;

        _towerUI.SetActive(true);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(_targetTower.transform.position);

        _towerUI.transform.position = screenPos + _offset;

        if (_currentRangeIndicator == null)
        {
            GameObject turretRangePrefab = Instantiate(_towerAtkRangePrefab);
            _currentRangeIndicator = turretRangePrefab.GetComponent<TurretRange>();
            _turretInfoUI = _towerUI.GetComponent<TurretInfoUI>();
        }
       
        _currentRangeIndicator.transform.SetParent(_targetTower.transform, false);
        _currentRangeIndicator.transform.localPosition = Vector3.zero;
        _currentRangeIndicator.gameObject.SetActive(true);
      
        _turretInfoUI.SetTurretInfoUI(turret);
        
        if (turret.TurretType == TowerTypes.FlameTower)
        {
            _currentRangeIndicator.ShowRangeCone(turret.transform, turret.AtkRange);
        }
        else
        {
            _currentRangeIndicator.ShowRangeCircle(turret.transform.position, turret.AtkRange);
        }

        if (turret.MyTurretData.Level < _maxLevel)
        {
            _upgradeBtnImage.sprite = _upgradeSprite;
        }

            _upgradeBtn.onClick.AddListener(() =>
        {
            int playerLevel = turret.MyTurretData.Level - 1;
            int totalPrice = _upgradePrice + playerLevel * _nextUpgradePrice;
            turret.UpgradeTurret();
            if(PlayerGUI.Instance.PlayerGold < totalPrice)
            {
                return;
            }

            if(turret.MyTurretData.Level == _maxLevel)
            {
                _upgradeBtnImage.sprite = _maxUpgradeSprite;
                return;
            }
            else
            {
                _upgradeBtnImage.sprite = _upgradeSprite;
            }
                _turretInfoUI.SetTurretInfoUI(turret);
            PlayerGUI.Instance.RemovePlayerGold(totalPrice);
            GameResultData.Instance.AddUsedGold(totalPrice);
        });

        _sellBtn.onClick.AddListener(() => { ResellTower(); });

    }

    public void ResellTower()
    {
        Turret turret = _targetTower.GetComponent<Turret>();
        turret.SetMyTileStateInit();

        int totalResellPrice = turret.MyTurretData.ResellPrice 
            + turret.MyTurretData.Level * _upgradePrice;    

        PlayerGUI.Instance.AddPlayerGold(totalResellPrice);

        PhotonView turretView = turret.GetComponent<PhotonView>();
        turretView.RPC("ActivateTurret", RpcTarget.AllBuffered, false);

        HideUI();
    }
    
    public void HideUI()
    {
        _isTowerUIActive = false;
        _targetTower = null;
        _towerUI.SetActive(false);

        if (_currentRangeIndicator != null)
        {
            _currentRangeIndicator.Hide();  
        }
    }
}

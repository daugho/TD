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

    private Transform _targetTower; 
    private bool _isTowerUIActive = false;

    private Button _upgradeBtn;
    private Button _sellBtn;


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
    }

    public bool IsTowerUIActiveFor(Transform tower)
    {
        return _towerUI.activeSelf && _targetTower == tower;
    }

    public void ShowUI(Transform tower)
    {
        if (_towerUI == null || _towerBuildButtonHandler._isClickBtn)
        {
            return;
        }
        _isTowerUIActive = true;

        _targetTower = tower;

        _towerUI.SetActive(true);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(_targetTower.position);

        _towerUI.transform.position = screenPos + _offset;

        if (_currentRangeIndicator == null)
        {
            GameObject turretRangePrefab = Instantiate(_towerAtkRangePrefab);
            _currentRangeIndicator = turretRangePrefab.GetComponent<TurretRange>();
            _turretInfoUI = _towerUI.GetComponent<TurretInfoUI>();
        }
       
        _currentRangeIndicator.transform.SetParent(_targetTower, false);
        _currentRangeIndicator.transform.localPosition = Vector3.zero;
        _currentRangeIndicator.gameObject.SetActive(true);
        
        Turret turret = _targetTower.GetComponent<Turret>();

        _turretInfoUI.SetTurretInfoUI(turret);
        
        if (turret.TurretType == TowerTypes.FlameTower)
        {
            _currentRangeIndicator.ShowRangeCone(turret.transform, turret.AtkRange);
        }
        else
        {
            _currentRangeIndicator.ShowRangeCircle(turret.transform.position, turret.AtkRange);
        }

        _upgradeBtn.onClick.AddListener(() => turret.UpgradeTurret());
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

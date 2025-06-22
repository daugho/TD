using UnityEngine;
using UnityEngine.UI;

public class TurretInfoUI : MonoBehaviour
{
    private Image _turretImage;
    private Text _turretName;

    private GameObject _turretInfoPanel; 
    private Text _levelText;
    private Text _atkText;
    private Text _speedText;

    private float _atkSpeedAmount = 0.1f;

    private bool _isInitialized = false;

    private void OnEnable()
    {
        if (_isInitialized) return;

        Transform panel = transform.Find("TowerInfo");

        _turretImage = transform.Find("TowerImage").GetComponent<Image>();
        _turretName = transform.Find("TowerName").GetComponent<Text>();

        _levelText = panel.Find("Level").GetComponent<Text>();
        _atkText = panel.Find("Atk").GetComponent<Text>();
        _speedText = panel.Find("AtkSpeed").GetComponent<Text>();

        _isInitialized = true;
    }
    public void SetTurretInfoUI(Turret turret)
    {
        TurretData data = DataManager.Instance.GetTurretData(turret.TurretType);
        Sprite turretSpite = Resources.Load<Sprite>("Prefabs/UI/Tower" + data.GachaPath);
        _turretImage.sprite = turretSpite;

        _turretName.text = data.KoreanName + " 타워";

        SetTurretStatInfo(turret);
    }

    public void SetTurretStatInfo(Turret turret)
    {
        TurretData turretData = turret.MyTurretData;

        int baseAtk = turretData.Atk;
        int upgradeAtk = turretData.Upgrade * turretData.Level;
        float totalAtk = baseAtk + upgradeAtk;
        
        float baseSpeed = turretData.AtkSpeed;
        float upgradeSpeed = turretData.Level * 0.1f;
        float totalSpeed = baseSpeed + upgradeSpeed;
        float fireDelay = 1.0f / totalSpeed;

        _levelText.text = "레벨 : " + turretData.Level.ToString();
        _atkText.text = $"공격력 : {totalAtk} (기본: {baseAtk} + 강화: {upgradeAtk})";
        _speedText.text = $"공격속도 : {totalSpeed:0.0} (기본: {baseSpeed:0.0} + 강화: {upgradeSpeed:0.0})\n" +
                      $"발사간격 : {fireDelay:0.00}초";
    }
}

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

        _turretName.text = data.KoreanName + " Ÿ��";

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

        _levelText.text = "���� : " + turretData.Level.ToString();
        _atkText.text = $"���ݷ� : {totalAtk} (�⺻: {baseAtk} + ��ȭ: {upgradeAtk})";
        _speedText.text = $"���ݼӵ� : {totalSpeed:0.0} (�⺻: {baseSpeed:0.0} + ��ȭ: {upgradeSpeed:0.0})\n" +
                      $"�߻簣�� : {fireDelay:0.00}��";
    }
}

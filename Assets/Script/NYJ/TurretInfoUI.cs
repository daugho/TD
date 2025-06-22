using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretInfoUI : MonoBehaviour
{
    private Image _turretImage;
    private TextMeshProUGUI _turretName;

    private GameObject _turretInfoPanel; 
    private TextMeshProUGUI _levelText;
    private TextMeshProUGUI _atkText;
    private TextMeshProUGUI _speedText;

    private float _atkSpeedAmount = 0.1f;

    private bool _isInitialized = false;

    private void OnEnable()
    {
        if (_isInitialized) return;

        Transform panel = transform.Find("TowerInfo");

        _turretImage = transform.Find("TowerImage").GetComponent<Image>();
        _turretName = transform.Find("TowerName").GetComponent<TextMeshProUGUI>();

        _levelText = panel.Find("Level").GetComponent<TextMeshProUGUI>();
        _atkText = panel.Find("Atk").GetComponent<TextMeshProUGUI>();
        _speedText = panel.Find("AtkSpeed").GetComponent<TextMeshProUGUI>();

        _isInitialized = true;
    }
    public void SetTurretInfoUI(Turret turret)
    {
        TurretData data = turret.MyTurretData;
        Sprite turretSpite = Resources.Load<Sprite>("Prefabs/UI/Tower/" + data.GachaPath);
        _turretImage.sprite = turretSpite;
        
        Color rarityColor = turret.TurretRarity.GetRarityColor(data.Rarity);
        string hexColor = ColorUtility.ToHtmlStringRGB(rarityColor);

        _turretName.text = $"{data.KoreanName} Ÿ��\n<color=#{hexColor}>{data.Rarity} ���</color></size>";

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

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

        _turretName.text = $"{data.KoreanName} 타워\n<color=#{hexColor}>{data.Rarity} 등급</color></size>";

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

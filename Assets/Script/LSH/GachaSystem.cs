using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GachaSystem : MonoBehaviour
{
    [SerializeField]
    private Transform _content; // ������(Content)

    [SerializeField]
    private TowerBuildButtonHandler _towerBuildButtonHandler;

    [SerializeField]
    private int _invenMaxCount = 10;

    private int _curButtonCount = 0;

    private int _gachaPrice = 200;

    private void OnEnable()
    {
        StartCoroutine(WaitForDataManager());
    }

    private IEnumerator WaitForDataManager()
    {
        while (DataManager.Instance.TurretDatas == null || DataManager.Instance.TurretDatas.Count == 0)
            yield return null;

        allTowers = DataManager.Instance.TurretDatas;
        SetTotalTower();
    }
    public void OnGachaButtonClick()
    {
        if (_curButtonCount >= _invenMaxCount)
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�.");
            return;
        }

        if (PlayerGUI.Instance.PlayerGold < _gachaPrice)
        {
            return;
        }

        PlayerGUI.Instance.RemovePlayerGold(_gachaPrice);

        var (type, chance) = GetRandomTowerWithChance();
        CreateButton(type);

        TurretData turretData = DataManager.Instance.GetTurretData(type);
        string rarity = turretData.Rarity.ToString();
        string sender = Photon.Pun.PhotonNetwork.NickName;
        string message = $"<color=yellow>{sender}</color>���� <color=orange>{chance:F1}%</color> Ȯ���� <b>{rarity}</b> ����� <color=cyan>{type}</color>�� ȹ���߽��ϴ�!";

        AlerManager.instance.SendMeesageToChat(message);
    }

    private void CreateButton(TowerTypes type)
    {
        TurretData turretData = DataManager.Instance.GetTurretData(type);   
        GameObject buttonprefab = Resources.Load<GameObject>("Prefabs/UI/TowerButtonPrefab");

        GameObject newButton = Instantiate(buttonprefab, _content);
        Button button = newButton.GetComponent<Button>();
        TowerPlaceBtn newButtonComponent = newButton.GetComponent<TowerPlaceBtn>();
        button.onClick.AddListener(() =>_towerBuildButtonHandler.OnTowerBuildButtonClicked(type, newButton));
        newButtonComponent.SetImage(turretData.GachaPath);

        _curButtonCount++;
        Debug.Log($"��ư ���� �Ϸ�! ���� ��ư ��: {_curButtonCount}/{_invenMaxCount}");
    }

    
    private void SetTotalTower()
    {
        weightedTowers.Clear(); // ���� ��� �ʱ�ȭ
        var grouped = allTowers.GroupBy(kvp => kvp.Value.Rarity);

        foreach (var group in grouped)
        {
            float rarityWeight = rarityWeights[group.Key];
            float weightPerTower = rarityWeight / group.Count();

            foreach (var kvp in group)
            {
                weightedTowers.Add((kvp.Key, weightPerTower));
            }
        }
    }
    private (TowerTypes type, float chance) GetRandomTowerWithChance()
    {
        float totalWeight = weightedTowers.Sum(x => x.weight);
        float rand = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in weightedTowers)
        {
            cumulative += entry.weight;
            if (rand <= cumulative)
            {
                float chance = (entry.weight / totalWeight) * 100f;
                return (entry.type, chance);
            }
        }

        var fallback = weightedTowers[0];
        float fallbackChance = (fallback.weight / totalWeight) * 100f;
        return (fallback.type, fallbackChance);
    }

    List<(TowerTypes type, float weight)> weightedTowers = new();

    private Dictionary<TowerTypes, TurretData> allTowers = new Dictionary<TowerTypes, TurretData>();

    Dictionary<TowerRarity, float> rarityWeights = new()
    {
    { TowerRarity.Normal, 0.5f },//2��
    { TowerRarity.Rare, 0.35f },//4��
    { TowerRarity.Epic, 0.15f },//2��
    { TowerRarity.Legendary, 0.05f }//1��
    };
}

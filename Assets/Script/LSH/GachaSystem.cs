using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GachaSystem : MonoBehaviour
{
    [SerializeField]
    private Transform _content; // 보관함(Content)

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
            Debug.Log("인벤토리가 가득 찼습니다.");
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
        string message = $"<color=yellow>{sender}</color>님이 <color=orange>{chance:F1}%</color> 확률로 <b>{rarity}</b> 등급의 <color=cyan>{type}</color>을 획득했습니다!";

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
        Debug.Log($"버튼 생성 완료! 현재 버튼 수: {_curButtonCount}/{_invenMaxCount}");
    }

    
    private void SetTotalTower()
    {
        weightedTowers.Clear(); // 기존 목록 초기화
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
    { TowerRarity.Normal, 0.5f },//2개
    { TowerRarity.Rare, 0.35f },//4개
    { TowerRarity.Epic, 0.15f },//2개
    { TowerRarity.Legendary, 0.05f }//1개
    };
}

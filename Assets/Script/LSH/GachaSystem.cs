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

    private void Start()
    {
        allTowers = DataManager.Instance.TurretDatas;

        SetTotalTower();
    }
    public void OnGachaButtonClick()
    {
        if(_curButtonCount >= _invenMaxCount)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return;
        }
       
        CreateButton(GetRandomTower());
    }

    private void CreateButton(TowerTypes type)
    {
        TurretData turretData = DataManager.Instance.GetTurretData(type);   
        GameObject buttonprefab = Resources.Load<GameObject>("Prefabs/UI/TowerButtonPrefab");

        GameObject newButton = Instantiate(buttonprefab, _content);
        Button button = newButton.GetComponent<Button>();
        TowerPlaceBtn newButtonComponent = newButton.GetComponent<TowerPlaceBtn>();
        button.onClick.AddListener(() =>_towerBuildButtonHandler.OnTowerBuildButtonClicked(type));
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

    public TowerTypes GetRandomTower()
    {
        float totalWeight = weightedTowers.Sum(x => x.weight);
        float rand = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach ((TowerTypes type, float weight) entry in weightedTowers)
        {
            cumulative += entry.weight;
            if (rand <= cumulative)
                return entry.type;
        }

        return weightedTowers[0].type; // fallback
    }

    List<(TowerTypes type, float weight)> weightedTowers = new();

    private Dictionary<TowerTypes, TurretData> allTowers = new Dictionary<TowerTypes, TurretData>();

    Dictionary<TowerRarity, float> rarityWeights = new()
    {
    { TowerRarity.Normal, 0.5f },
    { TowerRarity.Rare, 0.35f },
    { TowerRarity.Epic, 0.15f },
    { TowerRarity.Legendary, 0.05f }
    };
}

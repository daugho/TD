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

    public void OnGachaButtonClick()
    {
        if(_curButtonCount >= _invenMaxCount)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return;
        }
        TowerTypes towerTypes = TowerTypes.RifleTower;

        float randomValue = Random.Range(1.0f, 101.0f);
   
        if (randomValue <= 40)
            towerTypes = TowerTypes.RifleTower;
        else if (randomValue <= 70)
            towerTypes = TowerTypes.MachinegunTower;
        else if (randomValue <= 90)
            towerTypes = TowerTypes.GrenadeTower;
        else
            towerTypes = TowerTypes.ElectricTower;


        CreateButton(towerTypes);
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

        foreach (var group in allTowers.Values.GroupBy(t => t.Rarity))
        {
            float rarityWeight = rarityWeights[group.Key];
            float weightPerTower = rarityWeight / group.Count();

            foreach (var tower in group)
            {
                // TowerTypes 키를 가져오기 위해 Dictionary 검색
                TowerTypes type = allTowers.First(kvp => kvp.Value == tower).Key;
                weightedTowers.Add((type, weightPerTower));
            }
        }
    }

    public TowerTypes GetRandomTower()
    {
        float totalWeight = weightedTowers.Sum(x => x.weight);
        float rand = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in weightedTowers)
        {
            cumulative += entry.weight;
            if (rand <= cumulative)
                return entry.type;
        }

        return weightedTowers[0].GetType; // fallback
    }
    List<(TowerData tower, float weight)> weightedTowers = new();

    private Dictionary<TowerTypes, TurretData> allTowers = DataManager.Instance.TurretDatas;

    Dictionary<TowerRarity, float> rarityWeights = new()
    {
    { TowerRarity.Normal, 0.5f },
    { TowerRarity.Rare, 0.35f },
    { TowerRarity.Epic, 0.15f },
    { TowerRarity.Legendary, 0.05f }
    };
    public class TowerData
{
    public TowerRarity Rarity;
    public string Name;
    // 기타 정보
}
}

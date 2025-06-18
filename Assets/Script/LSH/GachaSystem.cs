using System.Collections.Generic;
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

        int randomValue = Random.Range(1, 101);
   
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
}

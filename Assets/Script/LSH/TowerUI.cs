using UnityEngine;

public class TowerUI : MonoBehaviour
{
    [SerializeField] private LayerMask _towerLayer; // 타워만 감지하는 레이어
    [SerializeField] private GameObject _uiPanel;   // 타워 정보 UI 패널
    private Turret _currentTower;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 클릭 시
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _towerLayer))
            {
                Turret tower = hit.collider.GetComponent<Turret>();

                if (tower != null)
                {
                    ShowTowerUI(tower);
                }
            }
            else
            {
                HideTowerUI();
            }
        }
    }

    private void ShowTowerUI(Turret tower)
    {
        _currentTower = tower;
        _uiPanel.SetActive(true);
        _uiPanel.transform.position = Camera.main.WorldToScreenPoint(tower.transform.position);

        // 필요한 경우 UI 내용 갱신
        // e.g. _uiPanel.GetComponent<TowerInfoPanel>().SetInfo(tower);
    }

    private void HideTowerUI()
    {
        _currentTower = null;
        _uiPanel.SetActive(false);
    }
}

using UnityEngine;

public class TowerUI : MonoBehaviour
{
    [SerializeField] private LayerMask _towerLayer; // Ÿ���� �����ϴ� ���̾�
    [SerializeField] private GameObject _uiPanel;   // Ÿ�� ���� UI �г�
    private Turret _currentTower;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Ŭ�� ��
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

        // �ʿ��� ��� UI ���� ����
        // e.g. _uiPanel.GetComponent<TowerInfoPanel>().SetInfo(tower);
    }

    private void HideTowerUI()
    {
        _currentTower = null;
        _uiPanel.SetActive(false);
    }
}

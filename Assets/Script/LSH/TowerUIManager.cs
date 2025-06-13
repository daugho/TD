using UnityEngine;

public class TowerUIManager : MonoBehaviour
{
    public static TowerUIManager Instance { get; private set; }

    [SerializeField]
    private GameObject towerUI; // UI GameObject
    private Transform targetTower; // 현재 UI가 활성화된 타워

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowUI(Transform tower)
    {
        if (towerUI == null)
        {
            Debug.LogError("Tower UI Prefab is not assigned!");
            return;
        }

        targetTower = tower;

        // UI 활성화
        towerUI.SetActive(true);

        // UI 위치를 타워 위로 이동
        Vector3 offset = new Vector3(0, 2.0f, -1.0f); // Z 값을 -1로 조정
        towerUI.transform.position = tower.position + offset;


        // 디버그 로그 추가
        Debug.Log($"UI Activated. World Position: {towerUI.transform.position}");
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(towerUI.transform.position);
        Debug.Log($"Viewport Position: {viewportPosition}");
    }


    public void HideUI()
    {
        targetTower = null;
        towerUI.SetActive(false);
    }
}

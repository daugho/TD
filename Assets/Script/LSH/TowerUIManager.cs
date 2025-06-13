using UnityEngine;

public class TowerUIManager : MonoBehaviour
{
    public static TowerUIManager Instance { get; private set; }

    [SerializeField]
    private GameObject towerUI; // UI GameObject
    private Transform targetTower; // ���� UI�� Ȱ��ȭ�� Ÿ��

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

        // UI Ȱ��ȭ
        towerUI.SetActive(true);

        // UI ��ġ�� Ÿ�� ���� �̵�
        Vector3 offset = new Vector3(0, 2.0f, -1.0f); // Z ���� -1�� ����
        towerUI.transform.position = tower.position + offset;


        // ����� �α� �߰�
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

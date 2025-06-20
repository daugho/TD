using UnityEngine;

public class ObjectInteractionUI : MonoBehaviour
{
    [SerializeField]
    private GameObject interactionUIPrefab; // UI Prefab
    private GameObject activeUI;

    [SerializeField] private Transform _towerUICanvas;

    public void OnObjectClick()
    {
        if (activeUI != null)
        {
            //Destroy(activeUI); // ���� UI ����
        }

        if (!interactionUIPrefab.gameObject.activeInHierarchy)
        {
            // UI�� Ȱ��ȭ
            interactionUIPrefab.gameObject.SetActive(true);
        }
        // UI ��ġ ���� (������ ���� ������)
        //Vector3 offset = new Vector3(1.0f, 1.0f, 0); // ������ ���� �̵�
        //activeUI.transform.position = transform.position + offset;
        //
        //// �θ� ���� (���� ���� ����)
        //activeUI.transform.SetParent(null, true);
    }

    public void CloseUI()
    {
        if (activeUI != null)
        {
            Destroy(activeUI);
        }
    }
}

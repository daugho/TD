using UnityEngine;

public class ObjectInteractionUI : MonoBehaviour
{
    [SerializeField]
    private GameObject interactionUIPrefab; // UI Prefab
    private GameObject activeUI;

    public void OnObjectClick()
    {
        if (activeUI != null)
        {
            Destroy(activeUI); // ���� UI ����
        }

        // UI ����
        activeUI = Instantiate(interactionUIPrefab, transform.position, Quaternion.identity);

        // UI ��ġ ���� (������ ���� ������)
        Vector3 offset = new Vector3(1.0f, 1.0f, 0); // ������ ���� �̵�
        activeUI.transform.position = transform.position + offset;

        // �θ� ���� (���� ���� ����)
        activeUI.transform.SetParent(null, true);
    }

    public void CloseUI()
    {
        if (activeUI != null)
        {
            Destroy(activeUI);
        }
    }
}

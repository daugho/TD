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
            //Destroy(activeUI); // 기존 UI 제거
        }

        if (!interactionUIPrefab.gameObject.activeInHierarchy)
        {
            // UI를 활성화
            interactionUIPrefab.gameObject.SetActive(true);
        }
        // UI 위치 조정 (오른쪽 위로 오프셋)
        //Vector3 offset = new Vector3(1.0f, 1.0f, 0); // 오른쪽 위로 이동
        //activeUI.transform.position = transform.position + offset;
        //
        //// 부모 설정 (월드 공간 유지)
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

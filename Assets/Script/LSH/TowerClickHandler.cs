using UnityEngine;

public class TowerClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (TowerUIManager.Instance != null)
        {
            Debug.Log("clcked");
            TowerUIManager.Instance.ShowUI(transform);
        }
    }
}

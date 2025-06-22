using UnityEngine;

public class TowerClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (TowerUIManager.Instance == null)
            return;
        if (TowerUIManager.Instance.IsTowerUIActiveFor(transform))
        {
            TowerUIManager.Instance.HideUI();
        }
        else
        {
            TowerUIManager.Instance.ShowUI(transform);
        }
    }
}

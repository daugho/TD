using UnityEngine;

public class TowerBuildButtonHandler : MonoBehaviour
{    public void OnTowerBuildButtonClicked()
    {
        InputManager.Instance.SetClickMode(ClickMode.TowerBuild);
        Debug.Log("[UI] Ÿ�� ��ġ ��� Ȱ��ȭ��");
    }
}

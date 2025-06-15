using UnityEngine;

public class TowerBuildButtonHandler : MonoBehaviour
{    public void OnTowerBuildButtonClicked()
    {
        InputManager.Instance.SetClickMode(ClickMode.TowerBuild);
        Debug.Log("[UI] 타워 설치 모드 활성화됨");
    }
}

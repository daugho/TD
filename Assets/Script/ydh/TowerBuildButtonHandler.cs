using Photon.Pun;
using UnityEngine;

public class TowerBuildButtonHandler : MonoBehaviour
{
    private bool _isClickBtn = false;
    private GameObject _activeTurret;
    private GameObject currentPreviewTile;

    private void Update()
    {
        if (!_isClickBtn) return;

        Vector3 mousePos = Input.mousePosition;
       
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
        {
            TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
            if (tile == null) return;

            Vector3 targetPos = tile.transform.position;
            _activeTurret.transform.position = targetPos + Vector3.up * 1.0f;

            if (Input.GetMouseButtonDown(0))
            {
                _activeTurret.transform.position = targetPos + Vector3.up * 1.0f;
                _activeTurret = null;
                _isClickBtn = false;
            }
        }
        else
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
            _activeTurret.transform.position = worldPos;
        }
    }
    public void OnTowerBuildButtonClicked()
    {
        TowerTypes[] allTypes = (TowerTypes[])System.Enum.GetValues(typeof(TowerTypes));
        TowerTypes type = allTypes[Random.Range(0, allTypes.Length)];

        GameObject turret = TurretManager.Instance.GetAvailableTurret(type);
        turret.SetActive(true);

        _activeTurret = turret;

        _isClickBtn = true;

        InputManager.Instance.SetClickMode(ClickMode.TowerBuild);
        Debug.Log("[UI] 타워 설치 모드 활성화됨");
    }

}

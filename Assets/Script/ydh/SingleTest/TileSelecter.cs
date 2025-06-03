using TMPro;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private Camera otherCamera;
    [SerializeField] private TextMeshProUGUI tileCoordText;
    private TileEditManager tileEditManager;

    private void Start()
    {
        if (otherCamera == null)
        {
            otherCamera = Camera.main;
        }
        tileEditManager = GetComponent<TileEditManager>();
        if (tileEditManager == null)
        {
            Debug.LogError("TileEditManager not found in the scene.");
        }
    }
    void Update()
    {
        Ray ray = otherCamera.ScreenPointToRay((Input.mousePosition));
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 pos = hit.point;
            int x = Mathf.FloorToInt(pos.x);//FloorToInt는 소수점 이하를 버립니다. 즉 가장 가까운 정수로 내림합니다.
            int z = Mathf.FloorToInt(pos.z);
            tileCoordText.text = $"Tile Coord: {x}, {z}";
            if (Input.GetMouseButtonDown(0))
            {
                TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
                if (tile == null) return;

                switch (tileEditManager.editMode)
                {
                    case EditMode.TileStateEdit:
                        tile.SetTileState(tileEditManager.currentTileState);
                        break;

                    case EditMode.AccessTypeEdit:
                        if (tile._tileState == TileState.Installable)
                        {
                            tile.SetAccessType(tileEditManager.currentAccessType);
                        }
                        else
                        {
                            Debug.LogWarning("?? Installable 타일 위에만 접근 타입을 설정할 수 있습니다.");
                        }
                        break;
                }
            }
            //if (Input.GetMouseButtonDown(0))
            //{
            //    TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
            //    if (tile == null) return;

            //    switch (tileEditManager.currentMode)
            //    {
            //        case TileState.Installable:
            //            tile.SetTileState(TileState.Installable);
            //            break;
            //        case TileState.Uninstallable:
            //            tile.SetTileState(TileState.Uninstallable);
            //            break;
            //        case TileState.StartPoint:
            //            tile.SetTileState(TileState.StartPoint);
            //            break;
            //        case TileState.EndPoint:
            //            tile.SetTileState(TileState.EndPoint);
            //            break;
            //        case TileState.MasterInstallable:
            //            tile.SetTileState(TileState.MasterInstallable);
            //            break;
            //        case TileState.ClientInstallable:
            //            tile.SetTileState(TileState.ClientInstallable);
            //            break;
            //    }
            //    if (tile._tileState == TileState.MasterInstallable || tile._tileState == TileState.ClientInstallable)
            //    {
            //        string role = Photon.Pun.PhotonNetwork.IsMasterClient ? "Master" : "Client";
            //        tile.photonView.RPC("SetTileClickedColor", Photon.Pun.RpcTarget.AllBuffered, role);
            //    }
            //}
        }
    }
}

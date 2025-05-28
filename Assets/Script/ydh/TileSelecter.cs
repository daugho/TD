using TMPro;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private Camera otherCamera;
    [SerializeField] private TextMeshProUGUI tileCoordText;
    private TileEditManager tileEditManager;

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

                switch (tileEditManager.currentMode)
                {
                    case TileState.Installable:
                        tile.SetTileState(TileState.Installable);
                        break;
                    case TileState.Uninstallable:
                        tile.SetTileState(TileState.Uninstallable);
                        break;
                    case TileState.StartPoint:
                        tile.SetTileState(TileState.StartPoint);
                        break;
                    case TileState.EndPoint:
                        tile.SetTileState(TileState.EndPoint);
                        break;
                }
            }
        }
    }
}

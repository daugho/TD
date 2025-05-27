using Photon.Pun;
using UnityEngine;

public class TileClickManager : MonoBehaviour
{
    [SerializeField] private Camera otherCamera;

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)  // 마스터만 작동
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = otherCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
                    if (tile != null)
                    {
                        tile.ToggleColor(); // 로컬 색상 변경만 실행
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = otherCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
                    if (tile != null)
                    {
                        tile.CToggleColor(); // 클라이언트 로컬 색상 변경만 실행
                    }
                }
            }
        }
    }
}

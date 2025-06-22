using System.Collections;
using UnityEngine;

public class CameraTargetSetter : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CameraController cameraController;

    public void StartSetCameraTargetDelayed(float delay = 1.0f)
    {
        StartCoroutine(DelayedSetTarget(delay));
    }

    private IEnumerator DelayedSetTarget(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetCameraTargetToCenterTile();
    }

    public void SetCameraTargetToCenterTile()
    {
        var centerTile = gridManager.GetCenterTile();
        if (centerTile != null)
        {
            cameraController.SetTarget(centerTile.transform);
            Debug.Log($"[CameraTargetSetter] �߽� Ÿ�� Ÿ�� ���� �Ϸ�: ({centerTile.CoordX}, {centerTile.CoordZ})");
        }
    }
}

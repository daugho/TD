using UnityEngine;

public class TileEditManager : MonoBehaviour
{
    public EditMode editMode = EditMode.TileStateEdit;
    public TileState currentTileState = TileState.None;
    public TileAccessType currentAccessType = TileAccessType.Everyone;

    // ---------- 설치 상태 모드 설정 ----------
    public void SelectInstallableMode()
    {
        editMode = EditMode.TileStateEdit;
        currentTileState = TileState.Installable;
    }

    public void SelectUninstallableMode()
    {
        editMode = EditMode.TileStateEdit;
        currentTileState = TileState.Uninstallable;
    }

    public void SelectStartPointMode()
    {
        editMode = EditMode.TileStateEdit;
        currentTileState = TileState.StartPoint;
    }

    public void SelectEndPointMode()
    {
        editMode = EditMode.TileStateEdit;
        currentTileState = TileState.EndPoint;
    }

    // ---------- 접근 권한 모드 설정 ----------
    public void SetAccessEveryone()
    {
        editMode = EditMode.AccessTypeEdit;
        currentAccessType = TileAccessType.Everyone;
    }

    public void SetAccessMasterOnly()
    {
        editMode = EditMode.AccessTypeEdit;
        currentAccessType = TileAccessType.MasterOnly;
    }

    public void SetAccessClientOnly()
    {
        editMode = EditMode.AccessTypeEdit;
        currentAccessType = TileAccessType.ClientOnly;
    }
}

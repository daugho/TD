using UnityEngine;

public class TileEditManager : MonoBehaviour
{
    public TileState currentMode = TileState.None;

    public void SelectInstallableMode()
    {
        currentMode = TileState.Installable;
    }

    public void SelectUninstallableMode()
    {
        currentMode = TileState.Uninstallable;
    }
    public void SelectStartPointMode()
    {
        currentMode = TileState.StartPoint;
    }
    public void SelectEndPointMode()
    {
        currentMode = TileState.EndPoint;
    }
}

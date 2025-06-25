using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public enum MobileTouchMode
{
    None,
    TileReveal,
    TowerBuild
}

public class MobileTouchHandler : MonoBehaviour
{
    [SerializeField] private TilePreviewController previewController;

    private MobileTouchMode currentMode = MobileTouchMode.None;

    private InputAction tapAction;
    private InputAction positionAction;

    private void OnEnable()
    {
        var inputMap = new InputActionMap("Touch");
        tapAction = inputMap.AddAction("Tap", binding: "<Touchscreen>/press");
        positionAction = inputMap.AddAction("Position", binding: "<Touchscreen>/position");

        tapAction.performed += OnTap;
        inputMap.Enable();
    }

    private void OnDisable()
    {
        tapAction.performed -= OnTap;
        tapAction.Disable();
        positionAction.Disable();
    }

    private void OnTap(InputAction.CallbackContext context)
    {
        if (currentMode == MobileTouchMode.None) return;

        Vector2 screenPos = positionAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
        if (tile == null) return;

        switch (currentMode)
        {
            case MobileTouchMode.TileReveal:
                TryRevealTile(tile);
                break;
            case MobileTouchMode.TowerBuild:
                TryPlaceTower(tile);
                break;
        }
    }

    private void TryRevealTile(TileBehaviour tile)
    {
        var renderer = tile.GetComponent<Renderer>();
        var collider = tile.GetComponent<Collider>();

        if (renderer != null && !renderer.enabled)
        {
            var access = PhotonNetwork.IsMasterClient ? TileAccessType.MasterOnly : TileAccessType.ClientOnly;

            tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
                (int)TileState.Installable, (int)access);

            tile.photonView.RPC(nameof(TileBehaviour.RPC_RevealTile), RpcTarget.AllBuffered);

            renderer.enabled = true;
            if (collider != null) collider.enabled = true;

            tile.PlayRevealEffect();

            previewController?.HidePreview();

            Debug.Log($"[Touch] 타일 리빌 완료: {tile.name}");
            ResetMode(); // 터치 1회로 종료
        }
    }

    private void TryPlaceTower(TileBehaviour tile)
    {
        if (tile._tileState != TileState.Installable) return;

        bool isMaster = PhotonNetwork.IsMasterClient;
        bool hasAccess = tile._accessType switch
        {
            TileAccessType.Everyone => true,
            TileAccessType.MasterOnly => isMaster,
            TileAccessType.ClientOnly => !isMaster,
            _ => false
        };
        if (!hasAccess) return;

        Vector3 spawnPos = tile.transform.position + Vector3.up * 0.5f;
        PhotonNetwork.Instantiate("TestTower", spawnPos, Quaternion.identity);

        tile.photonView.RPC(nameof(TileBehaviour.RPC_SetTileState), RpcTarget.AllBuffered,
            (int)TileState.Installed, (int)tile._accessType);

        ResetMode(); // 설치 후 자동 종료
    }

    public void SetTouchMode(int modeIndex)
    {
        currentMode = (MobileTouchMode)modeIndex;
        Debug.Log($"[Touch] 모드 변경: {currentMode}");
    }

    public void ResetMode()
    {
        currentMode = MobileTouchMode.None;
    }
}

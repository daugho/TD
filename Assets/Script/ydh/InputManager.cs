using System;
using UnityEngine;

public enum ClickMode
{
    None,
    TileReveal,
    TowerBuild
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    [SerializeField] private TilePreviewController previewController;
    public event Action<Vector3> OnTileRevealClick;
    public event Action<Vector3> OnTowerBuildClick;

    public ClickMode CurrentMode { get; private set; } = ClickMode.None;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()//타입을나눠 동시에 설치되는 것을 방지.
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Input.mousePosition;
            switch (CurrentMode)
            {
                case ClickMode.TileReveal:
                    OnTileRevealClick?.Invoke(clickPos);
                    break;

                case ClickMode.TowerBuild:
                    OnTowerBuildClick?.Invoke(clickPos);
                    break;
            }
        }
    }

    public void SetClickMode(ClickMode mode)
    {
        CurrentMode = mode;
        Debug.Log($"[InputManager] 모드 변경: {mode}");
    }

    public void ResetClickMode()
    {
        CurrentMode = ClickMode.None;
    }
}

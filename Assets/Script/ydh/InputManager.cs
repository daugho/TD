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

    private void Update()
    {
        Vector3 clickPos = Vector3.zero;
        bool isClick = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            clickPos = Input.mousePosition;
            isClick = true;
            Debug.Log("���콺 Ŭ�� ������: " + clickPos);
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            clickPos = Input.GetTouch(0).position;
            isClick = true;
            Debug.Log("��ġ ������: " + clickPos);
        }
#endif

        if (!isClick) return;

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

    public void SetClickMode(ClickMode mode)
    {
        CurrentMode = mode;
        Debug.Log($"[InputManager] ��� ����: {mode}");
    }

    public void ResetClickMode()
    {
        CurrentMode = ClickMode.None;
    }
}

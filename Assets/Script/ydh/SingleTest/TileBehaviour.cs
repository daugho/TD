using Photon.Pun;
using System;
using System.Diagnostics;
using UnityEngine;
public class TileBehaviour : MonoBehaviourPun
{
    public TileState _tileState { get; private set; } = TileState.None;
    public TileAccessType _accessType { get; private set; } = TileAccessType.Everyone;
    private Renderer _renderer;
    private Color _originalColor;
    private bool _isSelected = false;

    public static event Action OnAnyTileChanged;
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
        UnityEngine.Debug.Log($"[TileBehaviour] ViewID: {photonView.ViewID}");
    }
    public void SetTileState(TileState state)
    {
        SetTileState(state, TileAccessType.Everyone); // 기본 접근 권한
    }

    public void SetTileState(TileState state, TileAccessType access)
    {
        _tileState = state;

        if (state == TileState.Installable)
        {
            _accessType = access;
        }
        else
        {
            // ? 경고 조건 추가
            if (state != TileState.Installable)
            {
                UnityEngine.Debug.LogWarning($"?? {gameObject.name}: Installable 타일이 아닌데 권한 타입이 부여되었습니다. → 'Installable 타일 위에 타입을 부여하세요'");
            }

            _accessType = TileAccessType.Everyone; // 강제로 초기화
        }

        UnityEngine.Debug.Log($"[SetTileState] {gameObject.name} → 상태: {_tileState}, 권한: {_accessType}");
        UpdateColor();
        OnAnyTileChanged?.Invoke();
    }

    private void UpdateColor()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();

        Color color = _tileState switch
        {
            TileState.Installable => _accessType switch
            {
                TileAccessType.Everyone => Color.blue,
                TileAccessType.MasterOnly => Color.cyan,
                TileAccessType.ClientOnly => Color.yellow,
                _ => Color.white
            },
            TileState.Uninstallable => Color.gray,
            TileState.Installed => Color.red,
            TileState.StartPoint => Color.green,
            TileState.EndPoint => Color.black,
            _ => Color.white
        };

        _renderer.material.color = color;
    }
    public void SetAccessType(TileAccessType access)
    {
        if (_tileState != TileState.Installable)
        {
            UnityEngine.Debug.LogWarning("?? Installable 상태가 아닙니다. 접근 권한을 설정할 수 없습니다.");
            return;
        }

        _accessType = access;
        UnityEngine.Debug.Log($"[SetAccessType] {gameObject.name} → 권한: {_accessType}");
        UpdateColor();
    }
    [PunRPC]
    public void SetTileClickedColor(string role)
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();

        if (role == "Master")
            _renderer.material.color = Color.magenta;
        else if (role == "Client")
            _renderer.material.color = new Color(1.0f, 0.0f, 1.0f); // 마젠타
    }
    [PunRPC]
    public void RPC_SetTileState(int state, int access)
    {
        SetTileState((TileState)state, (TileAccessType)access);
    }

    [PunRPC]
    public void SetColorRPC(bool isSelected)
    {
        _isSelected = isSelected;
        _renderer.material.color = isSelected ? Color.green : _originalColor;
        Color color = _renderer.material.color;
    }
    public void ToggleColor()
    {
        bool nextState = !_isSelected;
        photonView.RPC("SetColorRPC", RpcTarget.AllBuffered, nextState);
    }
    [PunRPC]
    public void CSetColorRPC(bool isSelected)
    {
        _isSelected = isSelected;
        _renderer.material.color = isSelected ? Color.red : _originalColor;
        Color color = _renderer.material.color;
    }
    
    public void CToggleColor()
    {
        bool nextState = !_isSelected;
        photonView.RPC("CSetColorRPC", RpcTarget.AllBuffered, nextState);
    }
}

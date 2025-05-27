using Photon.Pun;
using System.Diagnostics;
using UnityEngine;
public class TileBehaviour : MonoBehaviourPun
{
    public TileState _tileState { get; private set; } = TileState.None;
    private Renderer _renderer;
    private Color _originalColor;
    private bool _isSelected = false;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
        UnityEngine.Debug.Log($"[TileBehaviour] ViewID: {photonView.ViewID}");
    }
    public void SetTileState(TileState newState)
    {
        _tileState = newState;
        UnityEngine.Debug.Log($"[SetTileState] {gameObject.name} → 상태: {_tileState}");
        UpdateColor();
    }  

    private void UpdateColor()
    {
        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        Color color = _tileState switch
        {
            TileState.Installable => Color.blue,
            TileState.Uninstallable => Color.gray,
            TileState.Installed => Color.red,
            _ => Color.white
        };

        UnityEngine.Debug.Log($"[UpdateColor] {gameObject.name} → 적용 색: {color}");

        _renderer.material.color = color;

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
    //public void ToggleColorLocalOnly()
    //{
    //    _isSelected = !_isSelected;
    //    _renderer.material.color = _isSelected ? Color.green : _originalColor;
    //    Color currentColor = _renderer.material.color;
    //    Debug.Log($"현재 머티리얼 색상: {currentColor}");
    //}
    //public void ToggleColorRPC()
    //{
    //    _isSelected = !_isSelected;
    //    Color newColor = _isSelected ? Color.green : _originalColor;
    //    _renderer.material.color = newColor;
    //    Debug.Log($"[TileBehaviour] 색상 변경됨: {newColor}");
    //}
}

using INab.Common;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
public class TileBehaviour : MonoBehaviourPun
{
    public TileState _tileState { get; private set; } = TileState.None;
    public TileAccessType _accessType { get; private set; } = TileAccessType.Everyone;
    private Renderer _renderer;
    private InteractiveEffect _effect;
    private Color _originalColor;
    private bool _isSelected = false;
    public int CoordX { get; private set; }
    public int CoordZ { get; private set; }

    public static event Action OnAnyTileChanged;
    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            var context = GameObject.Find("GridSystem")?.GetComponent<TileContext>();
            if (context != null)
            {
                transform.SetParent(context.TileParent);
            }
            else
            {
                UnityEngine.Debug.Log("TileContext를 찾을 수 없습니다.");
            }
        }
    }
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _effect = GetComponentInChildren<InteractiveEffect>();
        _effect.GetRendererMaterials();
        _effect.initialPosition = new Vector3(0f, 0f, 0f);
        _effect.finalPosition = new Vector3(0f, -2.5f, -1.0f);
        _effect.mask.transform.localPosition = _effect.initialPosition;
        _effect.PlayEffect();
        //_originalColor = _renderer.material.color;
        //UnityEngine.Debug.Log($"[TileBehaviour] ViewID: {photonView.ViewID}");
    }
    public void SetTileState(TileState state)
    {
        SetTileState(state, TileAccessType.Everyone); // 기본 접근 권한
    }

    public void SetTileState(TileState state, TileAccessType access)
    {
        UnityEngine.Debug.Log($"[SetTileState] 상태 변경 발생 → {gameObject.name}, state: {state}, access: {access}");
        _tileState = state;

        // ? StartPoint와 EndPoint도 권한을 허용
        if (state == TileState.Installable || state == TileState.StartPoint)
        {
            _accessType = access;
        }
        else
        {
            _accessType = TileAccessType.Everyone; // 기본값으로 리셋
        }

        //UnityEngine.Debug.Log($"[SetTileState] {gameObject.name} → 상태: {_tileState}, 권한: {_accessType}");
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
            TileState.StartPoint => _accessType switch
            {
                TileAccessType.Everyone => Color.green,               // 기본 StartPoint
                TileAccessType.MasterOnly => new Color(0.0f, 0.6f, 0.0f), // 진한 초록 (Master용)
                TileAccessType.ClientOnly => new Color(0.6f, 1.0f, 0.6f), // 밝은 초록 (Client용)
                _ => Color.green
            },
            TileState.Uninstallable => Color.gray,
            TileState.Installed => Color.red,
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
        UnityEngine.Debug.Log($"[RPC_SetTileState] {gameObject.name} 에서 RPC 호출 수신됨 → state: {(TileState)state}, access: {(TileAccessType)access}");
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
    
    public void CToggleColor()//클라이언트 전용 색상 변경함수
    {
        bool nextState = !_isSelected;
        photonView.RPC("CSetColorRPC", RpcTarget.AllBuffered, nextState);
    }
    [PunRPC]
    public void SetCoordinates(int x, int z)
    {
        CoordX = x;
        CoordZ = z;
        if (!PhotonNetwork.IsMasterClient)
        {
            var gridManager = FindFirstObjectByType<GridManager>();
            gridManager?.RegisterTile(this);
        }
    }
    [PunRPC]
    public void RPC_RevealTile()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = true;

        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = true;

        UnityEngine.Debug.Log($"[TileBehaviour] Reveal RPC 동기화 완료: {gameObject.name}");
    }
    public void PlayRevealEffect()
    {
        var effect = GetComponentInChildren<InteractiveEffect>(true);//비활성화된 자식 오브젝트까지 포함해서 InteractiveEffect를 찾는다
        if (effect == null || effect.mask == null)
        {
            UnityEngine.Debug.LogWarning("[TileBehaviour] 이펙트 없음");
            return;
        }

        effect.initialPosition = new Vector3(0f, 0f, 0f);
        effect.finalPosition = new Vector3(0f, -2.7f, -2.0f);
        effect.mask.transform.localPosition = effect.initialPosition;

        effect.gameObject.SetActive(true);
        effect.PlayEffect();
    }
}

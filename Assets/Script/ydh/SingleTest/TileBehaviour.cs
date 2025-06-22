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
                UnityEngine.Debug.Log("TileContext�� ã�� �� �����ϴ�.");
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
        SetTileState(state, TileAccessType.Everyone); // �⺻ ���� ����
    }

    public void SetTileState(TileState state, TileAccessType access)
    {
        UnityEngine.Debug.Log($"[SetTileState] ���� ���� �߻� �� {gameObject.name}, state: {state}, access: {access}");
        _tileState = state;

        // ? StartPoint�� EndPoint�� ������ ���
        if (state == TileState.Installable || state == TileState.StartPoint)
        {
            _accessType = access;
        }
        else
        {
            _accessType = TileAccessType.Everyone; // �⺻������ ����
        }

        //UnityEngine.Debug.Log($"[SetTileState] {gameObject.name} �� ����: {_tileState}, ����: {_accessType}");
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
                TileAccessType.Everyone => Color.green,               // �⺻ StartPoint
                TileAccessType.MasterOnly => new Color(0.0f, 0.6f, 0.0f), // ���� �ʷ� (Master��)
                TileAccessType.ClientOnly => new Color(0.6f, 1.0f, 0.6f), // ���� �ʷ� (Client��)
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
            UnityEngine.Debug.LogWarning("?? Installable ���°� �ƴմϴ�. ���� ������ ������ �� �����ϴ�.");
            return;
        }

        _accessType = access;
        UnityEngine.Debug.Log($"[SetAccessType] {gameObject.name} �� ����: {_accessType}");
        UpdateColor();
    }
    [PunRPC]
    public void SetTileClickedColor(string role)
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();

        if (role == "Master")
            _renderer.material.color = Color.magenta;
        else if (role == "Client")
            _renderer.material.color = new Color(1.0f, 0.0f, 1.0f); // ����Ÿ
    }
    [PunRPC]
    public void RPC_SetTileState(int state, int access)
    {
        UnityEngine.Debug.Log($"[RPC_SetTileState] {gameObject.name} ���� RPC ȣ�� ���ŵ� �� state: {(TileState)state}, access: {(TileAccessType)access}");
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
    
    public void CToggleColor()//Ŭ���̾�Ʈ ���� ���� �����Լ�
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

        UnityEngine.Debug.Log($"[TileBehaviour] Reveal RPC ����ȭ �Ϸ�: {gameObject.name}");
    }
    public void PlayRevealEffect()
    {
        var effect = GetComponentInChildren<InteractiveEffect>(true);//��Ȱ��ȭ�� �ڽ� ������Ʈ���� �����ؼ� InteractiveEffect�� ã�´�
        if (effect == null || effect.mask == null)
        {
            UnityEngine.Debug.LogWarning("[TileBehaviour] ����Ʈ ����");
            return;
        }

        effect.initialPosition = new Vector3(0f, 0f, 0f);
        effect.finalPosition = new Vector3(0f, -2.7f, -2.0f);
        effect.mask.transform.localPosition = effect.initialPosition;

        effect.gameObject.SetActive(true);
        effect.PlayEffect();
    }
}

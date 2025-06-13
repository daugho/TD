using Photon.Pun;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class PathVisualizerSpawner : MonoBehaviour
{

    [SerializeField] private TileContext tileContext;
    private PhotonView photonView;
    public static PathVisualizerSpawner Instance { get; private set; }
    
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("PathVisualizerSpawner �ߺ� �ν��Ͻ� ����");
            Destroy(gameObject);
        }
    }
    public void SpawnPathTracer()
    {
        photonView.RPC(nameof(RPC_SpawnPathTracer), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_SpawnPathTracer()
    {
        if (tileContext == null)
        {
            Debug.LogError("[PathVisualizer] TileContext�� ����Ǿ� ���� �ʽ��ϴ�.");
            return;
        }

        TileBehaviour startTile = FindMyStartPoint(tileContext);
        if (startTile == null)
        {
            Debug.LogWarning("[PathVisualizer] StartPoint�� ã�� �� �����ϴ�.");
            return;
        }

        Vector3 spawnPos = startTile.transform.position + Vector3.up;
        GameObject tracer = PhotonNetwork.Instantiate("PathVisualer", spawnPos, Quaternion.identity);

        PathTracer mover = tracer.GetComponent<PathTracer>();
        if (mover != null)
        {
            OwnerRole role = startTile._accessType switch
            {
                TileAccessType.MasterOnly => OwnerRole.Master,
                TileAccessType.ClientOnly => OwnerRole.Client,
                _ => PhotonNetwork.IsMasterClient ? OwnerRole.Master : OwnerRole.Client
            };

            mover.Initialize(role);
            mover.MoveByPathfindingLoop();
        }
    }

    private TileBehaviour FindMyStartPoint(TileContext tileContext)
    {
        bool isMaster = Photon.Pun.PhotonNetwork.IsMasterClient;

        foreach (Transform child in tileContext.TileParent)
        {
            var tile = child.GetComponent<TileBehaviour>();
            if (tile == null || tile._tileState != TileState.StartPoint)
                continue;

            switch (tile._accessType)
            {
                case TileAccessType.Everyone: return tile;
                case TileAccessType.MasterOnly: if (isMaster) return tile; break;
                case TileAccessType.ClientOnly: if (!isMaster) return tile; break;
            }
        }

        return null;
    }

}

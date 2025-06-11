using Photon.Pun;
using UnityEngine;

public class EnemySpawner : MonoBehaviour//멀티전용 몬스터 소환.
{
    [SerializeField] private TileContext tileContext;
    [SerializeField] private GameObject monsterPrefab;

    public void SpawnEnemy()
    {
        TileBehaviour startTile = FindMyStartPoint();
        if (startTile == null)
        {
            Debug.LogWarning("자신의 역할에 맞는 StartPoint가 없습니다.");
            Debug.Log($"[클라이언트] 타일 수: {tileContext.TileParent.childCount}");
            return;
        }

        Vector3 spawnPos = startTile.transform.position + Vector3.up * 0.5f;
        GameObject monster = PhotonNetwork.Instantiate("Prefabs/Monsters/Sentinel", spawnPos, Quaternion.identity);

        // MonsterMover를 찾아 경로 이동 시작
        MonsterMover mover = monster.GetComponent<MonsterMover>();
        if (mover != null)
        {
            OwnerRole role = startTile._accessType switch
            {
                TileAccessType.MasterOnly => OwnerRole.Master,
                TileAccessType.ClientOnly => OwnerRole.Client,
                _ => PhotonNetwork.IsMasterClient ? OwnerRole.Master : OwnerRole.Client
            };

            mover.Initialize(role);
            mover.MoveByPathfinding();
        }
    }

private TileBehaviour FindMyStartPoint()
{
    bool isMaster = PhotonNetwork.IsMasterClient;
        Debug.Log($"[DEBUG] 역할: {(isMaster ? "마스터" : "클라이언트")}");
    foreach (Transform child in tileContext.TileParent)
    {
        var tile = child.GetComponent<TileBehaviour>();
            if (tile == null || tile._tileState != TileState.StartPoint)
            {
                Debug.LogWarning("타일에 TileBehaviour 없음");
                continue;
            }

        Debug.Log($"▶ 타일 발견: {tile.CoordX},{tile.CoordZ} / Access: {tile._accessType}");

        switch (tile._accessType)
        {
        case TileAccessType.Everyone:
            return tile;

        case TileAccessType.MasterOnly:
            if (isMaster) return tile;
            break;

        case TileAccessType.ClientOnly:
            if (!isMaster) return tile;
            break;
        }
    }

    return null;
}
}

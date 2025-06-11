using Photon.Pun;
using UnityEngine;

public class EnemySpawner : MonoBehaviour//��Ƽ���� ���� ��ȯ.
{
    [SerializeField] private TileContext tileContext;
    [SerializeField] private GameObject monsterPrefab;

    public void SpawnEnemy()
    {
        TileBehaviour startTile = FindMyStartPoint();
        if (startTile == null)
        {
            Debug.LogWarning("�ڽ��� ���ҿ� �´� StartPoint�� �����ϴ�.");
            Debug.Log($"[Ŭ���̾�Ʈ] Ÿ�� ��: {tileContext.TileParent.childCount}");
            return;
        }

        Vector3 spawnPos = startTile.transform.position + Vector3.up * 0.5f;
        GameObject monster = PhotonNetwork.Instantiate("Prefabs/Monsters/Sentinel", spawnPos, Quaternion.identity);

        // MonsterMover�� ã�� ��� �̵� ����
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
        Debug.Log($"[DEBUG] ����: {(isMaster ? "������" : "Ŭ���̾�Ʈ")}");
    foreach (Transform child in tileContext.TileParent)
    {
        var tile = child.GetComponent<TileBehaviour>();
            if (tile == null || tile._tileState != TileState.StartPoint)
            {
                Debug.LogWarning("Ÿ�Ͽ� TileBehaviour ����");
                continue;
            }

        Debug.Log($"�� Ÿ�� �߰�: {tile.CoordX},{tile.CoordZ} / Access: {tile._accessType}");

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

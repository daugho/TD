using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private Turret[] _turrets;
    [SerializeField] private TileContext tileContext;
    private PhotonView _photonView;

    public static List<Monster> Monsters = new List<Monster>();
    
    public static Dictionary<(int,int), List<Monster>> ActiveMonsters = new Dictionary<(int, int), List<Monster>>();

    private HPBar _hpBar;
    private int _stage = 1;
    private int _wave = 2;


    private void Awake()
    {
        _hpBar = Resources.Load<HPBar>("Prefabs/Monsters/HPBar");
        _photonView = GetComponent<PhotonView>();
    }

    public void OnClickSpawn()
    {
        _photonView.RPC("SpawnMonsterRPC", RpcTarget.AllBuffered);
    }

    public void CreateMonsterPool(RoundData roundData)
    {
        foreach (MonsterSpawnInfo monster in roundData.Monsters)
        {
            for (int i = 0; i < monster.Count; i++)
            {
                MonsterSpawn(monster.Type);
            }
        }
    }

    [PunRPC]
    public void SpawnMonsterRPC()
    {
        RoundData roundData = DataManager.Instance.GetRoundData((_stage, _wave));

        StartCoroutine(SpawnWaveCoroutine(roundData));
    }

    private IEnumerator SpawnWaveCoroutine(RoundData roundData)
    {
        foreach (MonsterSpawnInfo monster in roundData.Monsters)
        {
            for (int i = 0; i < monster.Count; i++)
            {
                MonsterSpawn(monster.Type);
                yield return new WaitForSeconds(0.4f); 
            }
        }
    }
    private void MonsterSpawn(DroneTypes type)
    {
        TileBehaviour startTile = FindMyStartPoint();
        if (startTile == null)
        {
            Debug.LogWarning("자신의 역할에 맞는 StartPoint가 없습니다.");
            Debug.Log($"[클라이언트] 타일 수: {tileContext.TileParent.childCount}");
            return;
        }

        Vector3 spawnPos = startTile.transform.position + Vector3.up;

        RoundData roundData = DataManager.Instance.GetRoundData((_stage, _wave));

        MonsterData monsterData = DataManager.Instance.GetMonsterData((int)type);

        object[] instData = new object[]
       {
            (int)type,
            roundData.SpeedMultiplier,
            roundData.HpMultiplier
       };

        GameObject enemy = PhotonNetwork.Instantiate("Prefabs/Monsters/" + monsterData.PrefabPath,
            spawnPos, Quaternion.identity, 0, instData);
        
        FindMonsterPath(enemy, startTile);
    }

    public void FindMonsterPath(GameObject monster, TileBehaviour tile)
    {
        MonsterMover mover = monster.GetComponent<MonsterMover>();
        if (mover != null)
        {
            OwnerRole role = tile._accessType switch
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

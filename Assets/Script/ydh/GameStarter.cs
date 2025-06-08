using Photon.Pun;
using System.Collections;
using UnityEngine;

public class GameStarter : MonoBehaviourPun
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TileContext tileContext;
    public void OnClickStartButton()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(RPC_Spawn), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Spawn()
    {
        enemySpawner.SpawnEnemy();
    }

    private IEnumerator WaitUntilGridReady()
    {
        // 조건: 타일이 하나라도 생성될 때까지 대기
        yield return new WaitUntil(() => tileContext.TileParent.childCount > 0);

        yield return new WaitForSeconds(0.1f); // 약간의 안정 대기

        enemySpawner.SpawnEnemy();
    }
}

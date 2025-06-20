using UnityEngine;
using Photon.Pun;
using System.Collections;

public class TileChecker : MonoBehaviourPun
{
    [SerializeField] private TileContext tileContext;
    public void HideNoneTiles()
    {
        foreach (Transform child in tileContext.TileParent)
        {
            var tile = child.GetComponent<TileBehaviour>();
            if (tile != null && tile._tileState == TileState.None)
            {
                // ? 타일 비활성화 방식: 렌더러 & 콜라이더 끄기
                var renderer = tile.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.enabled = false;
            }
        }

        Debug.Log("None 상태인 모든 타일을 렌더러/콜라이더 비활성화 처리했습니다.");
    }
    [PunRPC]
    public void RPC_HideNoneTiles()
    {
        StartCoroutine(DelayHideNoneTiles());
    }

    private IEnumerator DelayHideNoneTiles()
    {
        // 동기화 시간이 충분하도록 약간 대기
        yield return new WaitForSeconds(0.5f);

        HideNoneTiles();
    }
}

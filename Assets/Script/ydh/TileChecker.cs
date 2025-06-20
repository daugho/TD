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
                // ? Ÿ�� ��Ȱ��ȭ ���: ������ & �ݶ��̴� ����
                var renderer = tile.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.enabled = false;
            }
        }

        Debug.Log("None ������ ��� Ÿ���� ������/�ݶ��̴� ��Ȱ��ȭ ó���߽��ϴ�.");
    }
    [PunRPC]
    public void RPC_HideNoneTiles()
    {
        StartCoroutine(DelayHideNoneTiles());
    }

    private IEnumerator DelayHideNoneTiles()
    {
        // ����ȭ �ð��� ����ϵ��� �ణ ���
        yield return new WaitForSeconds(0.5f);

        HideNoneTiles();
    }
}

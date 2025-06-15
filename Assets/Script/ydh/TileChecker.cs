using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TileChecker : MonoBehaviour
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
}

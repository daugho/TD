using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TileChecker : MonoBehaviour
{
    [SerializeField] private TileContext tileContext;
    [SerializeField] private Button checkButton;

    private void Start()
    {
        checkButton.onClick.AddListener(HideNoneTiles);
    }

    private void HideNoneTiles()
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
}

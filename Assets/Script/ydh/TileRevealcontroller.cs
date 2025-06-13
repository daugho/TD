using UnityEngine;
using UnityEngine.UI;

public class TileRevealcontroller : MonoBehaviour
{
    [SerializeField] private TileContext _tileContext;
    [SerializeField] private Button _activateModeButton;

    private bool isRevealMode = false;
    private void Start()
    {
        _activateModeButton.onClick.AddListener(()=>
        {
            isRevealMode = !isRevealMode;
            Debug.Log($"[TileRevealController] 활성화 모드: {isRevealMode}");
        }
        );
    }
    private void Update()
    {
        if (!isRevealMode) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
                if (tile == null) return;
                var renderer = tile.GetComponent<Renderer>();
                var collider = tile.GetComponent<Collider>();
                if(renderer != null && !renderer.enabled)
                {
                    renderer.enabled = true;
                    if(collider != null) collider.enabled = true;
                    Debug.Log($"[TileRevealController] 타일 복원됨 : {tile.name}");
                    //아이템 관련 로직을 여기에 추가.
                }
            }
        }
    }
}

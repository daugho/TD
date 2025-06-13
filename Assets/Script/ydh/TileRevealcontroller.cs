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
            Debug.Log($"[TileRevealController] Ȱ��ȭ ���: {isRevealMode}");
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
                    Debug.Log($"[TileRevealController] Ÿ�� ������ : {tile.name}");
                    //������ ���� ������ ���⿡ �߰�.
                }
            }
        }
    }
}

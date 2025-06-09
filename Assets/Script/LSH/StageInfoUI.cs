using UnityEngine;
using UnityEngine.UI;

public class StageInfoUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _stageInfoUI;
    [SerializeField]
    private Button _startButton;


    private void Start()
    {
        if(_stageInfoUI != null)
        {
            _stageInfoUI.SetActive(false);
        }
    }
    public void ShowStageInfo()
    {
        if (_stageInfoUI == null) return;

        _stageInfoUI.SetActive(true);
    }

    public void CloseStageInfo()
    {
        if(_stageInfoUI != null)
        {
            _stageInfoUI.SetActive(false);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class StageInfoUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _stageInfoUI;
    [SerializeField]
    private Button _startButton;

    [SerializeField]
    private GameObject _buttonPrefab;
    [SerializeField]
    private Transform _stageBtnContent;

    [SerializeField]
    private AddChildImages _addChildImages;

    private void Start()
    {
        if (_stageInfoUI != null)
        {
            _stageInfoUI.SetActive(false);
        }

        CreateStageBtn();
    }

    public void CreateStageBtn()
    {
        for (int i = 1; i <= 10; i++)
        {
            GameObject newButton = Instantiate(_buttonPrefab, _stageBtnContent);
            Button button = newButton.GetComponent<Button>();
            TowerPlaceBtn newButtonComponent = newButton.GetComponent<TowerPlaceBtn>();
            button.onClick.AddListener(() =>
            {
                _addChildImages.LoadAndDisplayMonsters(i, 10);
                ShowStageInfo();
            }); 
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

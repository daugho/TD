using TMPro;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
    public static PlayerGUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _playerGoldUI;

    private int _playerGold = 400;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetGoldUI()
    {
        _playerGoldUI.text = _playerGold.ToString();
    }

    public void AddPlayerGold(int goldAmount)
    {
        _playerGold += goldAmount;
        SetGoldUI();
    }

    public void RemovePlayerGold(int goldAmount)
    {
        _playerGold -= goldAmount;
        SetGoldUI();
    }
    
}

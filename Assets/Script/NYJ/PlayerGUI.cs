using TMPro;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
    public static PlayerGUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _playerGoldUI;

    public int PlayerGold;

    private int _playerGold = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        AddPlayerGold(400);
    }

    public void SetGoldUI()
    {
        _playerGoldUI.text = _playerGold.ToString();
        PlayerGold = _playerGold;
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

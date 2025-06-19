using TMPro;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerGoldUI;

    private int _playerGold;

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

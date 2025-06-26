using UnityEngine;

public class GameResultData : MonoBehaviour
{
    public static GameResultData Instance { get; private set; }

    private int _life;
    private int _usedGold;
    private int _monsterKill;
    private int _towerBuilt;

    private void Awake()
    {
        Instance = this;
    }

    public void SetLife(int value) => _life = value;
    public void AddUsedGold(int value) => _usedGold += value;
    public void AddMonsterKill(int count = 1) => _monsterKill += count;
    public void AddTowerBuilt(int count = 1) => _towerBuilt += count;

    public int GetLife() => _life;
    public int GetUsedGold() => _usedGold;
    public int GetMonsterKill() => _monsterKill;
    public int GetTowerBuilt() => _towerBuilt;
}

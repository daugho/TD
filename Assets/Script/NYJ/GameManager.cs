using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isGameStart = false;
    private static GameManager _instance;

    [SerializeField] private EnemyPath _enemyPath;
    [SerializeField] private Transform _monsterGuiCanvas;
    public Transform GetMonsterCanvas () { return _monsterGuiCanvas; }
    public EnemyPath GetMonsterPath () { return _enemyPath; }

    public static GameManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;

        DataManager.Instance.LoadTurretData();
        DataManager.Instance.LoadMonsterData();
        DataManager.Instance.LoadRoundData();
    }
}

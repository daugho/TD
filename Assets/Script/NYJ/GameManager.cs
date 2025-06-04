using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isGameStart = false;
    private static GameManager _instance;

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

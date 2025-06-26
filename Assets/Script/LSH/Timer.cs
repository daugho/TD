using UnityEngine;
using TMPro;
using Photon.Pun;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;
    [SerializeField]
    private TextMeshProUGUI _stageText;
    [SerializeField]
    private TextMeshProUGUI _waveText;

    private MonsterManager _monsterManager;

    [SerializeField] private GameResult _gameResult;

    private float _waveStartTimer = 20.0f;
    private float _waveTimer = 40f;
    private float _waveWaitingTimer = 20.0f;

    private float _timer = 0.0f;
    private bool _isRunning = false;
    private bool _isWaveStart = false;

    private int _stage = 1;
    private int _wave = 1;
    private int _maxWave = 10;

    private RoundState _currentState;

    public enum RoundState
    {
        WaitingToStart, InWave, BreakTime 
    }
    private void Awake()
    {
        _monsterManager = GetComponent<MonsterManager>();
    }

    private void Start()
    {
        UpdateStageAndWaveText();
        ChangeTimerState(RoundState.WaitingToStart);    
    }

    private void Update()
    {
        if (_isRunning)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                _timer = 0;
                _isRunning = false;

                switch (_currentState)
                {
                    case RoundState.WaitingToStart:
                        ChangeTimerState(RoundState.InWave);
                        break;
                    case RoundState.InWave:
                        ChangeTimerState(RoundState.BreakTime);
                        break;
                    case RoundState.BreakTime:
                        ChangeTimerState(RoundState.InWave);
                        break;
                }
            }

            UpdateTimerText(_timer);
        }
    }

    private void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        string formattedTime = $"{minutes:00}:{seconds:00}";

        _timerText.text = formattedTime;
    }

    public void StartTimer()
    {
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }

    public void ChangeTimerState(RoundState state)
    {
        _currentState = state;

        switch (state)
        {
            case RoundState.WaitingToStart:
                _timer = _waveStartTimer;
                break;
            case RoundState.InWave:
                _isWaveStart = true;
                _monsterManager.SetStageAndWave(_stage, _wave);
                _wave++;

                if (PhotonNetwork.IsMasterClient)
                {
                    _monsterManager.SpawnMonsters();
                }
                _timer = _waveTimer;
                break;
            case RoundState.BreakTime:
                if(_wave > _maxWave)
                {
                    _gameResult.gameObject.SetActive(true);
                    //PhotonNetwork.LoadLevel("StageScene");
                }
                _timer = _waveWaitingTimer;
                break;
        }

        UpdateTimerText(_timer);
        UpdateStageAndWaveText();
        StartTimer();
    }

    private void UpdateStageAndWaveText()
    {
        _stageText.text = $"{_stage}";
        _waveText.text = $"{_wave - 1}";
    }

}

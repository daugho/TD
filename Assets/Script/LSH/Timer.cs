using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;

    private MonsterManager _monsterManager;

    private float _waveStartTimer = 20.0f;
    private float _waveTimer = 60f;
    private float _waveWaitingTimer = 30.0f;

    private float _timer = 0.0f;
    private bool _isRunning = false;
    private bool _isGameStart = false;
    
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
                _monsterManager.SpawnMonsters();
                _timer = _waveTimer;
                break;
            case RoundState.BreakTime:
                _timer = _waveWaitingTimer;
                break;
        }

        UpdateTimerText(_timer);
        StartTimer();
    }
}

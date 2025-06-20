using UnityEngine;
using TMPro; 

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;

    private float timer = 60f;
    private bool isRunning = true;

    void Update()
    {
        if (isRunning)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                isRunning = false;
            }
            UpdateTimerText(timer);
        }

        // 탭 키를 누르면 타이머 초기화
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ResetTimer();
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
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        timer = 60f;
        isRunning = true;
        UpdateTimerText(timer);
    }
}

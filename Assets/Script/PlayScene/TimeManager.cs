using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("시간 설정")]
    [SerializeField] private float timeScale = 30f; // 실시간 1분 = 게임 30분
    [SerializeField] private int startHour = 7; // 시작 시간 (오전 7시)
    [SerializeField] private int startMinute = 0;

    [Header("현재 시간")]
    public int currentDay = 1;
    public int currentHour = 7;
    public int currentMinute = 0;

    private float minuteTimer = 0f;
    private TimeOfDay currentTimeOfDay;

    public enum TimeOfDay
    {
        Dawn,      // 새벽 (5-7시)
        Day,       // 낮 (7-18시)
        Evening,   // 저녁 (18-20시)
        Night      // 밤 (20-5시)
    }

    // 이벤트
    public event Action<int, int> OnTimeChanged; // 시간 변경
    public event Action<TimeOfDay> OnTimeOfDayChanged; // 시간대 변경
    public event Action OnNewDay; // 새날

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHour = startHour;
        currentMinute = startMinute;
        UpdateTimeOfDay();
    }

    void Update()
    {
        // 1분마다 게임 시간 증가
        minuteTimer += Time.deltaTime * timeScale;

        if (minuteTimer >= 60f) // 1분 경과
        {
            minuteTimer = 0f;
            AddMinute();
        }
    }

    void AddMinute()
    {
        currentMinute++;

        if (currentMinute >= 60)
        {
            currentMinute = 0;
            currentHour++;

            if (currentHour >= 24)
            {
                currentHour = 0;
                currentDay++;
                OnNewDay?.Invoke();
            }

            UpdateTimeOfDay();
        }

        OnTimeChanged?.Invoke(currentHour, currentMinute);
    }

    void UpdateTimeOfDay()
    {
        TimeOfDay newTimeOfDay = GetCurrentTimeOfDay();

        if (newTimeOfDay != currentTimeOfDay)
        {
            currentTimeOfDay = newTimeOfDay;
            OnTimeOfDayChanged?.Invoke(currentTimeOfDay);
            Debug.Log($"시간대 변경: {currentTimeOfDay}");
        }
    }

    TimeOfDay GetCurrentTimeOfDay()
    {
        if (currentHour >= 5 && currentHour < 7)
            return TimeOfDay.Dawn;
        else if (currentHour >= 7 && currentHour < 18)
            return TimeOfDay.Day;
        else if (currentHour >= 18 && currentHour < 20)
            return TimeOfDay.Evening;
        else
            return TimeOfDay.Night;
    }

    public string GetTimeString()
    {
        return $"Day {currentDay} - {currentHour:00}:{currentMinute:00}";
    }

    public TimeOfDay GetTimeOfDay()
    {
        return currentTimeOfDay;
    }
}
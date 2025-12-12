using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField] private TimeCapsule startTime = new(6, 0), endTime = new(18, 0);
    [SerializeField] private int secondsInDay = 180;
    [SerializeField] private TextMeshProUGUI txt_Minute, txt_Hour;

    private int day = -1;
    private float currentSeconds = 180;
    private bool isPlaying = false;

    void Start()
    {

    }

    public void StartGame()
    {
        day = 0;
    }

    public void StartDay()
    {
        day++;
        isPlaying = true;
        currentSeconds = secondsInDay;
        GetDayClock();
        StartCoroutine(StartTime());
    }

    public void EndDay()
    {
        currentSeconds = secondsInDay;
        isPlaying = false;
    }

    private bool timeStarted = false;

    private IEnumerator StartTime()
    {
        if (!timeStarted)
        {
            timeStarted = true;
            while (isPlaying && currentSeconds > 0)
            {
                Debug.Log("Tick" + currentSeconds);
                currentSeconds -= 0.5f;
                if (currentSeconds % 2.5f == 0)
                {
                    GetDayClock();
                }
                yield return new WaitForSeconds(0.5f);
            }
            Debug.Log("Tick End" + currentSeconds);
            txt_Hour.text = $"00";
            txt_Minute.text = "00";
            timeStarted = false;

        }
    }

    public bool IsPlaying() => isPlaying;

    public float GetSecondLeft() => currentSeconds;

    private void GetDayClock()
    {
        float secondsLeft = currentSeconds;
        int totalDuration = secondsInDay;

        float progress = Mathf.Clamp01(secondsLeft / (float)totalDuration);

        int totalDayMinutes = (endTime.hour - startTime.hour) * 60;
        int minutesPassed = Mathf.RoundToInt((1f - progress) * totalDayMinutes);

        int currentHour = startTime.hour + (minutesPassed / 60);
        int currentMinute = minutesPassed % 60;

        txt_Hour.text = $"{currentHour:00}";
        txt_Minute.text = $"{currentMinute:00}";
    }

    //

    public int GetDay() => day;

    public TimeCapsule GetTime()
    {
        TimeCapsule rtnTime = new();
        float secondsLeft = currentSeconds;
        int totalDuration = secondsInDay;

        float progress = Mathf.Clamp01(secondsLeft / (float)totalDuration);

        int totalDayMinutes = (endTime.hour - startTime.hour) * 60;
        int minutesPassed = Mathf.RoundToInt((1f - progress) * totalDayMinutes);

        int currentHour = startTime.hour + (minutesPassed / 60);
        int currentMinute = minutesPassed % 60;

        rtnTime.hour = currentHour;
        rtnTime.minute = currentMinute;

        return rtnTime;
    }
}

[Serializable]
public class TimeCapsule
{
    public int hour, minute;
    public TimeCapsule() { }
    public TimeCapsule(int hours, int minutes)
    {
        hour = hours; minute = minutes;
    }
}

[Serializable]
public class TimeCapsules
{
    [SerializeField] public List<int> hours, minutes;
    public List<TimeCapsule> GetTimeCapsules()
    {
        List<TimeCapsule> rtn = new();
        for (int i = 0; i < hours.Count; i++) rtn.Add(new(hours[i], minutes[i]));
        return rtn;
    }
}
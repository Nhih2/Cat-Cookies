using System;
using System.Collections;
using UnityEngine;

public interface Taskable
{
    public event Action onTaskDone;
    public event Action<float> onTaskProgress;
    public void Interact(float time);
    public float GetCurrentProgress();
    public TaskType GetTaskType();
    public void Reset();
}

public class Task : MonoBehaviour
{
    [SerializeField] private TaskScoreType type;
    [SerializeField] private bool isUnavaibleAfterDone = false;
    [Space]
    [Header("Task Mode")]
    [SerializeField] private bool isNaughtyTask = false;
    [SerializeField] private TaskMode mode = TaskMode.Single;
    [SerializeField][ShowIfEnum("mode", (int)TaskMode.Chain)] private int id = 0;
    [SerializeField] private int score = 100, sastisfaction = 100;


    [Space]
    [Header("Link")]
    [SerializeField] private bool activateAfterTask = false;
    [ShowIfBool("activateAfterTask", true)][SerializeField] private float lastTaskDelay;
    [ShowIfBool("activateAfterTask", true)][SerializeField] private Task previousTask;
    private bool isActivated = false;

    private void PrevTaskSetUp()
    {
        if (!activateAfterTask || !previousTask) return;
        previousTask.OnTaskDone += OnLastTaskActivation;
    }

    private void OnLastTaskActivation()
    {
        StartCoroutine(WaitToActivate(lastTaskDelay));
    }

    private IEnumerator WaitToActivate(float time)
    {
        yield return new WaitForSeconds(time);
        isActivated = true;
    }


    [Space]
    [Header("Limitation (start inclusive, end inclusive)")]
    [SerializeField] private bool onlyAvaibleInTimeFrame = false;
    [ShowIfBool("onlyAvaibleInTimeFrame", true)][SerializeField] private TimeCapsule startTime, endTime;
    [SerializeField] private bool onlyAvaibleInDays = false;
    [ShowIfBool("onlyAvaibleInDays", true)][SerializeField] private int startDay, endDay;
    [SerializeField] private bool isRepeatable = true;
    [SerializeField] private bool isHarcore = false;



    public event Action OnTaskDone, OnTaskCaught;

    private Task_TapAndWait task_TapAndWait;
    private Task_Hold task_Duration;
    private Task_RepeatedTap task_RepeatedTap;
    private Taskable currentTask;

    private bool isDone = false, isCaught = false;

    void Awake()
    {
        task_TapAndWait = GetComponent<Task_TapAndWait>();
        if (task_TapAndWait) currentTask = task_TapAndWait;
        task_Duration = GetComponent<Task_Hold>();
        if (task_Duration) currentTask = task_Duration;
        task_RepeatedTap = GetComponent<Task_RepeatedTap>();
        if (task_RepeatedTap) currentTask = task_RepeatedTap;
        CheckError();
        PrevTaskSetUp();
    }

    private void CheckError()
    {
        int cnt = 0;
        if (task_Duration) cnt++;
        if (task_TapAndWait) cnt++;
        if (task_RepeatedTap) cnt++;
        if (cnt > 1) DebugSys.LogError("It is not allowed to have more than one type of Task (Task_TapAndWait, etc)");
        if (cnt == 0) DebugSys.LogError("A TaskType Component must be assigned (e.g. TapAndWait)");
    }

    void Start()
    {
        currentTask.onTaskDone += FinishedTask;
        if (isRepeatable)
        {
            GameManager.Instance.OnDayStart += DayStart;
            GameManager.Instance.OnScoreEnd += DayStart;
        }
    }

    private void DayStart()
    {
        isDone = false; isCaught = false;
        currentTask.Reset();
    }

    public void Interact(float time)
    {
        if (!CheckAvailibility()) return;
        if (isDone)
        {
            DebugSys.LogWarning("Task is already finished.");
            return;
        }
        if (mode == TaskMode.Chain)
        {
            bool hasChain = GameSetting.TaskChainDictionary.ContainsKey(tag);
            int currentId = hasChain ? GameSetting.TaskChainDictionary[tag] : -1;

            if (id > 0 && !hasChain)
            {
                DebugSys.LogWarning("Please start the task chain first.");
                return;
            }

            if (hasChain && currentId < id)
            {
                DebugSys.LogWarning("Please finish previous task in this task chain.");
                return;
            }
        }

        //        DebugSys.Log("Interact");
        currentTask.Interact(time);
    }

    private void FinishedTask()
    {
        if (isDone) return;
        OnTaskDone?.Invoke();

        ScoreManager.Instance.AddScore(score, type);
        if (sastisfaction > 0) SastisfactoryManager.Instance.AddSastisfactory(sastisfaction);

        isDone = true;
        if (mode == TaskMode.Chain)
        {
            //DebugSys.Log("[Task Chain of " + tag + "] Link " + id + " is finished");
            if (!GameSetting.TaskChainDictionary.ContainsKey(tag))
                GameSetting.TaskChainDictionary.Add(tag, 1);
            else
                GameSetting.TaskChainDictionary[tag] = id + 1;
        }
        else if (mode == TaskMode.Single)
            mode = mode;
        //DebugSys.Log("Finished Task " + gameObject.name + "!");
    }

    public void CaughtNaughtyTask()
    {
        if (!isNaughtyTask) return;
        if (isDone) return;

        isDone = true;
        isCaught = true;
        OnTaskCaught?.Invoke();
        SastisfactoryManager.Instance.AddSastisfactory(sastisfaction);
    }



    public bool IsNaughtyTask() => isNaughtyTask;
    public bool IsWorking() => currentTask.GetCurrentProgress() > 0 && !isDone;
    public bool IsCaught() => isCaught;
    public float GetScore() => score;
    public float GetSastisfaction() => sastisfaction;
    public Taskable GetCurrentTask() => currentTask;
    public float GetCurrentProgress => currentTask.GetCurrentProgress();
    public bool CheckAvailibility()
    {
        if (isUnavaibleAfterDone && isDone) return false;
        if (isHarcore && !GameManager.Instance.isHardcore) return false;
        if (TimerManager.Instance.GetSecondLeft() <= 0) return false;
        if (activateAfterTask && !isActivated) return false;
        if (!CheckChain()) return false;
        if (!onlyAvaibleInTimeFrame && !onlyAvaibleInDays) return true;
        bool flag = true;
        if (onlyAvaibleInTimeFrame && !CheckTime()) flag = false;
        if (onlyAvaibleInDays && !CheckDay()) flag = false;
        return flag;
    }

    private bool CheckChain()
    {
        if (mode == TaskMode.Chain)
        {
            bool hasChain = GameSetting.TaskChainDictionary.ContainsKey(tag);
            int currentId = hasChain ? GameSetting.TaskChainDictionary[tag] : -1;

            if (id > 0 && !hasChain)
            {
                return false;
            }

            if (hasChain && currentId < id)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckTime()
    {
        TimeCapsule currentTime = TimerManager.Instance.GetTime();
        int currentMinutes = currentTime.hour * 60 + currentTime.minute;
        int startMinutes = startTime.hour * 60 + startTime.minute;
        int endMinutes = endTime.hour * 60 + endTime.minute;

        if (startMinutes <= endMinutes)
        {
            return currentMinutes >= startMinutes && currentMinutes <= endMinutes;
        }
        else
        {
            return currentMinutes >= startMinutes || currentMinutes <= endMinutes;
        }
    }

    private bool CheckDay()
    {
        int currentDay = TimerManager.Instance.GetDay();
        if (currentDay >= startDay && currentDay <= endDay) return true;
        return false;
    }
}

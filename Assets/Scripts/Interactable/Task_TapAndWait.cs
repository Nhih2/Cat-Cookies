using System;
using System.Collections;
using UnityEngine;

public class Task_TapAndWait : MonoBehaviour, Taskable
{
    public event Action onTaskDone;
    public event Action<float> onTaskProgress;

    [SerializeField] private float waitTime = 2.5f;

    private float timeWaited = 0;
    private bool isWaiting = false;

    public void Interact(float time)
    {
        if (isWaiting)
        {
            DebugSys.LogWarning("Task is running, please wait.");
            return;
        }

        StartCoroutine(WaitToFinish());
    }

    private IEnumerator WaitToFinish()
    {
        isWaiting = true;
        float decayUpdateTime = 0, maxUpdate = 0.1f;
        while (timeWaited < waitTime)
        {
            decayUpdateTime += Time.deltaTime;
            timeWaited += Time.deltaTime;
            if (decayUpdateTime > maxUpdate)
            {
                decayUpdateTime = 0;
                onTaskProgress?.Invoke(timeWaited / waitTime);
            }
            yield return null;
        }
        isWaiting = false;
        onTaskProgress?.Invoke(1);
        onTaskDone?.Invoke();
    }

    public float GetCurrentProgress() => timeWaited / waitTime;

    public TaskType GetTaskType() => TaskType.TapAndWait;

    public void Reset()
    {
        timeWaited = 0;
        isWaiting = false;
    }
}

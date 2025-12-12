using System;
using UnityEngine;

public class Task_Hold : MonoBehaviour, Taskable
{
    private const float CUT_OFF_DURATION = 0.05f;

    public event Action onTaskDone;
    public event Action<float> onTaskProgress;

    [SerializeField] private float holdDuration = 2.5f;
    [SerializeField] private bool isDecaying = false;
    [SerializeField] private float decayRate = 1f;

    private float timeSinceLastSignal = 0, culmulativeTime = 0, currentTime = 0, decayLevel = 0;
    private bool isInteracting = false, isDone = false;

    private float decayUpdateTime = 0, maxUpdateTime = 0.1f;

    public void Interact(float time)
    {
        isInteracting = true;
        timeSinceLastSignal = 0;
        currentTime = time;
    }

    public void Update()
    {
        HandleTimeUpdate();
        HandleTaskProgress();
        HandleDecay();
        HandleProgressUpdate();
    }

    private void HandleProgressUpdate()
    {
        if (!isDecaying && !isInteracting) return;
        decayUpdateTime += Time.deltaTime;
        if (decayUpdateTime > maxUpdateTime)
        {
            decayUpdateTime = 0;
            onTaskProgress?.Invoke((currentTime + culmulativeTime - decayLevel) / holdDuration);
        }
    }

    private void HandleDecay()
    {
        if (!isDecaying) return;
        if (isDone) return;
        if (culmulativeTime + currentTime - decayLevel <= 0) return;
        decayLevel += Time.deltaTime * decayRate;
        if (culmulativeTime + currentTime - decayLevel < 0) decayLevel = culmulativeTime + currentTime;
    }

    private void HandleTaskProgress()
    {
        if (culmulativeTime + currentTime - decayLevel > holdDuration && !isDone)
        {
            onTaskProgress?.Invoke(1);
            onTaskDone?.Invoke();
            isDone = true;
        }
    }

    private void HandleTimeUpdate()
    {
        if (isDone) return;
        if (!isInteracting) return;
        timeSinceLastSignal += Time.deltaTime;
        if (timeSinceLastSignal > CUT_OFF_DURATION)
        {
            Release();
            isInteracting = false;
        }
    }

    private void Release()
    {
        culmulativeTime += currentTime;
        currentTime = 0;
    }

    public float GetCurrentProgress()
    {
        if (culmulativeTime + currentTime - decayLevel >= holdDuration) return 1;
        return (currentTime + culmulativeTime - decayLevel) / holdDuration;
    }

    public TaskType GetTaskType() => TaskType.Hold;

    public void Reset()
    {
        timeSinceLastSignal = 0; culmulativeTime = 0; currentTime = 0; decayLevel = 0;
        isInteracting = false; isDone = false;
        decayUpdateTime = 0; maxUpdateTime = 0.1f;
    }
}

using System;
using UnityEngine;

public class Task_RepeatedTap : MonoBehaviour, Taskable
{
    private const float CUT_OFF_DURATION = 0.05f;

    public event Action onTaskDone;
    public event Action<float> onTaskProgress;

    [SerializeField] private int numberOfTaps = 5;
    [SerializeField] private bool isDecaying = false;
    [SerializeField] private float decayRate = 1f;

    private int taps = 0;
    private float timeSinceLastSignal = 0, decayLevel = 0;
    private bool isInteracting = false, isDone = false;

    private float decayUpdateTime = 0, maxUpdateTime = 0.1f;

    public void Interact(float time)
    {
        isInteracting = true;
        timeSinceLastSignal = 0;
    }

    public void Update()
    {
        HandleTimeUpdate();
        HandleDecay();
    }

    private void HandleDecay()
    {
        if (!isDecaying) return;
        if (isDone) return;
        if (taps - decayLevel <= 0) return;
        decayLevel += Time.deltaTime * decayRate;
        if (taps - decayLevel < 0) decayLevel = taps;

        decayUpdateTime += Time.deltaTime;
        if (decayUpdateTime > maxUpdateTime)
        {
            decayUpdateTime = 0;
            onTaskProgress?.Invoke(((taps - decayLevel) / numberOfTaps));
        }
    }

    private void HandleTimeUpdate()
    {
        if (!isInteracting) return;
        timeSinceLastSignal += Time.deltaTime;
        if (timeSinceLastSignal > CUT_OFF_DURATION)
        {
            Tap();
            isInteracting = false;
        }
    }

    private void Tap()
    {
        taps++;
        onTaskProgress?.Invoke(((taps - decayLevel) / numberOfTaps));
        DebugSys.Log("Tapped: " + taps);
        if (taps - decayLevel >= numberOfTaps)
        {
            onTaskProgress?.Invoke(1);
            onTaskDone?.Invoke();
            isDone = true;
        }
    }

    public float GetCurrentProgress() => ((taps - decayLevel) / numberOfTaps);

    public TaskType GetTaskType() => TaskType.RepeatedTap;

    public void Reset()
    {
        taps = 0;
        timeSinceLastSignal = 0; decayLevel = 0;
        isInteracting = false; isDone = false;
        decayUpdateTime = 0; maxUpdateTime = 0.1f;
    }
}

using System;
using TMPro;
using UnityEngine;

public class PlayerInteractionDisplay : MonoBehaviour
{
    [SerializeField] private InfoBoardUI infoBoard;
    [SerializeField] private GameObject slider;
    [SerializeField] private CanvasGroup FinishedDisplay, CaughtDisplay;

    private Task currentTask;
    private TaskSliderUI taskSliderUI;
    private SliderUI sliderUI;

    void Start()
    {
        FinishedDisplay.alpha = 0;
        CaughtDisplay.alpha = 0;
        sliderUI = slider.GetComponent<SliderUI>();
        taskSliderUI = slider.GetComponent<TaskSliderUI>();
    }

    public void ShowDisplay(Task task)
    {
        if (currentTask)
        {
            currentTask.GetCurrentTask().onTaskProgress -= UpdateInfoDisplay;
            currentTask.GetCurrentTask().onTaskProgress -= sliderUI.SetValue;
            currentTask.GetCurrentTask().onTaskDone -= UpdateFinishDisplay;
            currentTask.OnTaskCaught -= UpdateCaughtDisplay;
        }
        currentTask = task;

        UpdateCaughtDisplay();
        UpdateFinishDisplay();
        infoBoard.ShowInfoBoard(task);
        SetSliderValue(task);
    }

    private void SetSliderValue(Task task)
    {
        taskSliderUI.SetType(task.GetCurrentTask().GetTaskType(), task.IsWorking());
        sliderUI.SetValueNoAnim(task.GetCurrentProgress);

        currentTask.GetCurrentTask().onTaskProgress += UpdateInfoDisplay;
        currentTask.GetCurrentTask().onTaskProgress += sliderUI.SetValue;
        currentTask.GetCurrentTask().onTaskDone += UpdateFinishDisplay;
        currentTask.OnTaskCaught += UpdateCaughtDisplay;
    }

    private void UpdateInfoDisplay(float obj)
    {
        taskSliderUI.SetType(currentTask.GetCurrentTask().GetTaskType(), currentTask.IsWorking());
    }

    public void HideDisplay()
    {
        if (currentTask)
        {
            currentTask.GetCurrentTask().onTaskProgress -= UpdateInfoDisplay;
            currentTask.GetCurrentTask().onTaskProgress -= sliderUI.SetValue;
            currentTask.GetCurrentTask().onTaskDone -= UpdateFinishDisplay;
            currentTask.OnTaskCaught -= UpdateCaughtDisplay;
        }
        infoBoard.HideInfoBoard();
    }

    private void UpdateFinishDisplay()
    {
        if (currentTask.IsCaught()) FinishedDisplay.alpha = 0;
        else if (currentTask.GetCurrentProgress >= 1) FinishedDisplay.alpha = 1;
        else FinishedDisplay.alpha = 0;
    }

    private void UpdateCaughtDisplay()
    {
        if (currentTask.IsCaught()) CaughtDisplay.alpha = 1;
        else CaughtDisplay.alpha = 0;
    }
}

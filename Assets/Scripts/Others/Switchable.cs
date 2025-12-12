using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Switchable : MonoBehaviour
{
    [SerializeField] private bool lockedOnHardcore = false;
    [Header("Node")]
    [SerializeField] private bool activateAfterPrevNode = false;
    [ShowIfBool("activateAfterPrevNode", true)][SerializeField] private float lastNodeDelay = 0;
    [ShowIfBool("activateAfterPrevNode", true)][SerializeField] private List<MovementNode> previousNodes;

    private void PrevNodeActSetUp()
    {
        if (!activateAfterPrevNode || previousNodes == null || previousNodes.Count == 0) return;
        foreach (MovementNode previousNode in previousNodes)
            previousNode.OnActivation += OnLastNodeActivation;
    }

    private void OnLastNodeActivation(HumanController human)
    {
        DebugSys.Log("Activate By Last Node");
        StartCoroutine(WaitToActivate(lastNodeDelay));
    }

    private IEnumerator WaitToActivate(float time)
    {
        yield return new WaitForSeconds(time);
        Activate();
    }

    [Space]
    [Header("Task")]
    [SerializeField] private bool activateAfterTask = false;
    [ShowIfBool("activateAfterTask", true)][SerializeField] private float lastTaskDelay;
    [ShowIfBool("activateAfterTask", true)][SerializeField] private Task previousTask;
    [ShowIfBool("activateAfterTask", true)][SerializeField] private List<Task> previousTasks = new();

    private void PrevTaskSetUp()
    {
        if (!activateAfterTask || !previousTask) return;
        previousTask.OnTaskDone += OnLastTaskActivation;
        if (previousTask)
            foreach (Task task in previousTasks)
                task.OnTaskDone += OnLastTaskActivation;
    }

    private void OnLastTaskActivation()
    {
        StartCoroutine(WaitToActivate(lastTaskDelay));
    }

    [Space]
    [Header("Time")]
    [SerializeField] private bool isActivateOnTime = false;
    [ShowIfBool("isActivateOnTime", true)][SerializeField] private TimeCapsules activationTime;
    private List<TimeCapsule> timeCapsules;
    private List<bool> isTimeActivated;
    [SerializeField] private bool isActivatedOnDays = false;
    [ShowIfBool("isActivatedOnDays", true)][SerializeField] private int startDay, endDay;
    [Space]
    [SerializeField] private bool resetOnDayStart = true;

    private SpriteRenderer _renderer;

    private void TimeConditionSetUp()
    {
        if (!isActivateOnTime || activationTime == null) return;
        isTimeActivated = new List<bool>();
        timeCapsules = activationTime.GetTimeCapsules();
        timeCapsules = timeCapsules.OrderBy(tc => tc.GetTotalMinutes()).ToList();
        foreach (TimeCapsule time in timeCapsules) isTimeActivated.Add(false);
    }

    private void Update_CheckTimeCondition()
    {
        if (!isActivateOnTime || activationTime == null) return;
        if (!CheckDay()) return;

        int cnt = 0;
        foreach (TimeCapsule time in timeCapsules)
        {
            if (isTimeActivated[cnt])
            {
                cnt++;
                continue;
            }
            int actMin = time.GetTotalMinutes();
            int curMin = TimerManager.Instance.GetTime().GetTotalMinutes();
            if (actMin <= curMin)
            {
                isTimeActivated[cnt] = true;
                Activate();
            }
            break;
        }
    }

    private bool CheckDay()
    {
        if (!isActivatedOnDays) return true;
        int curDay = TimerManager.Instance.GetDay();
        return curDay >= startDay && curDay <= endDay;
    }


    [Space]
    [Header("Switchable Info")]
    [SerializeField] private SwitchInfo switchInfo;

    //

    public event Action OnActivate;
    private bool isActivated = false;

    void Awake()
    {
        CheckError();
        SetUp();
    }

    void Start()
    {
        if (resetOnDayStart) GameManager.Instance.OnDayStart += OnDayStart;
    }

    private void OnDayStart()
    {
        DebugSys.Log("Day Started");
        isActivated = false;
        bool state = switchInfo.isTransitioning;
        switchInfo.isTransitioning = false;
        if (lockedOnHardcore && GameManager.Instance.isHardcore) SetToActivated();
        else SetToDeactivated();
        switchInfo.isTransitioning = state;
    }

    private void SetUp()
    {
        PrevNodeActSetUp();
        PrevTaskSetUp();
        TimeConditionSetUp();
        if (switchInfo.changeSprite) _renderer = GetComponent<SpriteRenderer>();
    }

    private void CheckError()
    {
        if (!activateAfterPrevNode && !isActivateOnTime && !activateAfterTask) DebugSys.LogError("Switch can not be activated. Please set up an activation method " + gameObject.name);
        if (!activateAfterPrevNode && previousNodes == null && previousNodes.Count == 0) DebugSys.LogError("Switch can not be activated. Please set up a previous Node " + gameObject.name);
        if (activateAfterTask && previousTask == null) DebugSys.LogError("Switch can not be activated. Please set up a previous Task " + gameObject.name);
        if (isActivateOnTime && activationTime == null) DebugSys.LogError("Switch can not be activated. Please set up an activation time " + gameObject.name);
        if (switchInfo == null) DebugSys.LogError("Switch can not be activated. Please set up the switch info. " + gameObject.name);
    }

    private void Activate()
    {
        if (lockedOnHardcore && GameManager.Instance.isHardcore) return;
        isActivated = !isActivated;
        OnActivate?.Invoke();
        if (isActivated) SetToActivated();
        else SetToDeactivated();
    }

    private void SetToActivated()
    {
        var info = switchInfo;
        if (info.isTransitioning)
        {
            transform.DOKill();
            if (info.changePosition)
                transform.DOLocalMove(info.activatedPosition, info.tweenDuration).SetEase(info.Ease);

            if (info.changeRotation)
                transform.DORotate(new Vector3(0, 0, info.activatedRotation), info.tweenDuration).SetEase(info.Ease);
        }
        else
        {
            if (info.changePosition)
                transform.localPosition = info.activatedPosition;

            if (info.changeRotation)
                transform.rotation = Quaternion.Euler(0, 0, info.activatedRotation);
        }

        if (info.changeSprite)
            _renderer.sprite = info.activatedSprite;
    }

    private void SetToDeactivated()
    {
        var info = switchInfo;
        if (info.isTransitioning)
        {
            transform.DOKill();
            if (info.changePosition)
                transform.DOLocalMove(info.basePosition, info.tweenDuration).SetEase(info.Ease);

            if (info.changeRotation)
                transform.DORotate(new Vector3(0, 0, info.baseRotation), info.tweenDuration).SetEase(info.Ease);
        }
        else
        {
            if (info.changePosition)
                transform.localPosition = info.basePosition;

            if (info.changeRotation)
                transform.rotation = Quaternion.Euler(0, 0, info.baseRotation);
        }

        if (info.changeSprite)
            _renderer.sprite = info.baseSprite;
    }

    void Update()
    {
        Update_CheckTimeCondition();
    }
}

[Serializable]
public class SwitchInfo
{
    [Space]
    [Space]
    public bool changePosition;
    public Vector3 basePosition, activatedPosition;
    public bool changeRotation;
    public float baseRotation, activatedRotation;
    public bool changeSprite;
    public Sprite baseSprite, activatedSprite;
    [Space]
    [Header("Animation")]
    public bool isTransitioning = false;
    public float tweenDuration = 0.5f;
    public Ease Ease = Ease.Linear;
}
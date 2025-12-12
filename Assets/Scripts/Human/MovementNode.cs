using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class MovementNode : MonoBehaviour
{
    [ShowIfAnyBool(true, "activateAfterTask", "isActivateOnTime")][SerializeField] private HumanController human;
    [Header("Node")]
    [SerializeField] private bool activateAfterPrevNode = false;
    [ShowIfBool("activateAfterPrevNode", true)][SerializeField] private float lastNodeDelay = 0;
    [ShowIfBool("activateAfterPrevNode", true)][SerializeField] private MovementNode previousNode;

    private void PrevNodeActSetUp()
    {
        if (!activateAfterPrevNode || !previousNode) return;
        previousNode.OnActivation += OnLastNodeActivation;
    }

    private void OnLastNodeActivation(HumanController human)
    {
        Initialize(human);

        if (hasActivated)
        {
            DebugSys.LogWarning("Node has already been activated");
            return;
        }
        if (!TimerManager.Instance.IsPlaying()) return;

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

    private void PrevTaskSetUp()
    {
        if (!activateAfterTask || !previousTask) return;
        Initialize(human);
        previousTask.OnTaskDone += OnLastTaskActivation;
    }

    private void OnLastTaskActivation()
    {
        if (hasActivated)
        {
            DebugSys.LogWarning("Node has already been activated");
            return;
        }
        if (!TimerManager.Instance.IsPlaying()) return;

        StartCoroutine(WaitToActivate(lastTaskDelay));
    }

    [Space]
    [Header("Time")]
    [SerializeField] private bool isActivateOnTime = false;
    [ShowIfBool("isActivateOnTime", true)][SerializeField] private TimeCapsule activationTime;
    [SerializeField] private bool isRepeatable = true;
    [SerializeField] private int day;

    private void TimeConditionSetUp()
    {
        if (!isActivateOnTime || activationTime == null) return;
        Initialize(human);
    }

    private void Update_CheckTimeCondition()
    {
        if (!TimerManager.Instance.IsPlaying()) return;
        if (hasActivated) return;
        if (!isActivateOnTime || activationTime == null) return;
        if (TimerManager.Instance.GetDay() != day && !isRepeatable) return;

        int actMin = activationTime.GetTotalMinutes();
        int curMin = TimerManager.Instance.GetTime().GetTotalMinutes();
        if (actMin < curMin) Activate();
    }


    [Space]
    [Header("Movement Info")]
    [SerializeField] private MovementInfo movementInfo;


    [Header("For Editor")]
    [SerializeField] private bool isVisualized = true;
    [ShowIfBool("isVisualized", true)][SerializeField] private Color visualizedColor = Color.red;

    //

    public event Action<HumanController> OnActivation;
    private bool hasActivated = false;

    void Awake()
    {
        hasActivated = false;
        CheckError();
        SetUp();
    }

    void Start()
    {
        GameManager.Instance.OnDayStart += DayStart;
    }

    private void DayStart()
    {
        hasActivated = false;
    }

    private void SetUp()
    {
        PrevNodeActSetUp();
        PrevTaskSetUp();
        TimeConditionSetUp();
    }

    private void CheckError()
    {
        if (!activateAfterPrevNode && !isActivateOnTime) DebugSys.LogError("Node can not be activated. Please set up an activation method");
        if (activateAfterPrevNode && previousNode == null) DebugSys.LogError("Node can not be activated. Please set up a previous Node");
        if (activateAfterTask && previousTask == null) DebugSys.LogError("Node can not be activated. Please set up a previous Task");
        if (isActivateOnTime && activationTime == null) DebugSys.LogError("Node can not be activated. Please set up an activation time");
        if (movementInfo == null) DebugSys.LogError("Node can not be activated. Please set up the movement info");
    }

    public void Initialize(HumanController humanController)
    {
        human = humanController;
    }

    private void Activate()
    {
        hasActivated = true;
        movementInfo.node = this;
        StartCoroutine(WaitForActivation());
    }

    private IEnumerator WaitForActivation()
    {
        while (human.Move(movementInfo) == MoveStatus.Wait)
        {
            yield return new WaitForSeconds(0.1f);
        }
        OnActivation?.Invoke(human);
    }

    void Update()
    {
        Update_CheckTimeCondition();
    }

    void OnDrawGizmos()
    {
        if (hasActivated) return;
        Gizmos.color = visualizedColor;
        if ((isActivateOnTime && activationTime != null && human) || (activateAfterTask && previousTask && human))
        {
            Gizmos.DrawLine(human.transform.position, transform.position);
            return;
        }
        if (activateAfterPrevNode && previousNode)
        {
            Gizmos.DrawLine(previousNode.transform.position, transform.position);
            return;
        }
        if (activateAfterTask && previousTask)
        {
            Gizmos.DrawLine(previousTask.transform.position, transform.position);
            return;
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

public class WorldIndicator : MonoBehaviour
{
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Task task;
    [Space]
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
        StartCoroutine(WaitToActivate(lastNodeDelay));
    }

    private IEnumerator WaitToActivate(float time)
    {
        yield return new WaitForSeconds(time);
        StartIndicator();
    }

    public Transform anchor;
    public RectTransform indicator;
    public float edgeBuffer = 50f;

    private bool isDone = false;

    void Start()
    {
        PrevNodeActSetUp();
        GameManager.Instance.OnDayStart += DayStart;
        GameManager.Instance.OnScoreEnd += DayEnd;
        task.OnTaskDone += Activate;
        indicator.gameObject.SetActive(false);
    }

    private void DayEnd()
    {
        indicator.gameObject.SetActive(false);
    }

    private void Activate()
    {
        isDone = true;
        indicator.gameObject.SetActive(false);
    }

    private void StartIndicator()
    {
        DebugSys.Log("Start By Indicator");
        isDone = false;
        indicator.gameObject.SetActive(true);
    }

    private void DayStart()
    {
        if (activateAfterPrevNode) return;
        isDone = false;
        indicator.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!task.CheckAvailibility())
        {
            indicator.gameObject.SetActive(false);
            return;
        }
        if (isDone) return;
        if (anchor == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(anchor.position);

        if (screenPos.z < 0)
        {
            screenPos *= -1;
        }

        screenPos.x = Mathf.Clamp(screenPos.x, edgeBuffer, Screen.width - edgeBuffer);
        screenPos.y = Mathf.Clamp(screenPos.y, edgeBuffer, Screen.height - edgeBuffer);

        indicator.position = screenPos + offset;
    }
}

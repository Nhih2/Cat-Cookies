using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HumanSwitchable : MonoBehaviour
{
    [Header("Task")]
    [SerializeField] private List<Transform> Nodes;
    [SerializeField] private float lastTaskDelay;
    [SerializeField] private Task previousTask;
    [SerializeField] private float speed = 10, delayInterval = 2.5f;

    Sequence seq;

    private int index = 0;
    private bool isMoving = false;
    private Transform target;

    void Start()
    {
        PrevTaskSetUp();
        GameManager.Instance.OnDayStart += OnDayStart;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            if (!isActivated && !isMoving)
            {
                index++;
                if (index >= Nodes.Count) index = 0;
                target = Nodes[index];
                isMoving = true;

                float distance = (target.position - transform.position).magnitude;
                float moveDuration = distance / speed;

                if (seq != null && seq.IsActive()) seq.Kill();
                transform.DOKill();

                FlipSprite(Nodes[index]);
                seq = DOTween.Sequence();
                seq.Append(transform.DOMove(target.position, moveDuration));
                seq.AppendInterval(delayInterval);
                seq.OnComplete(() =>
                {
                    //                    DebugSys.LogWarning("Complete");
                    isMoving = false;
                });
            }
            yield return null;
        }
    }

    private void FlipSprite(Transform Node)
    {
        Vector3 dir = (Node.position - transform.position).normalized;

        if (dir.x > 0 && transform.localScale.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (dir.x < 0 && transform.localScale.x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void OnDayStart()
    {
        isActivated = false;
        index = 0;

        if (seq != null && seq.IsActive()) seq.Kill();
        transform.DOKill();

        transform.position = Nodes[index].position;

        StartCoroutine(ResetMoveNextFrame());
    }

    private IEnumerator ResetMoveNextFrame()
    {
        yield return null;
        isMoving = false;
    }

    private void PrevTaskSetUp()
    {
        previousTask.OnTaskDone += OnLastTaskActivation;
    }

    private void OnLastTaskActivation()
    {
        StartCoroutine(WaitToActivate(lastTaskDelay));
    }

    private IEnumerator WaitToActivate(float time)
    {
        yield return new WaitForSeconds(time);
        Activate();
    }

    private bool isActivated = false;

    private void Activate()
    {
        if (isActivated) return;
        if (seq != null && seq.IsActive()) seq.Kill();
        DebugSys.Log("Activated");
        transform.DOKill();

        transform.position = Nodes[index].position;

        transform.position = new(-100, -100);
    }
}

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    private Transform body;
    public void SetBody(Transform body)
    {
        this.body = body;
    }

    [SerializeField] private bool rotationLocked = false;
    [SerializeField] private float defaultRotateSpeed = 260;
    [SerializeField] private Ease defaultRotateEase = Ease.Linear;

    private Vector3 basePosition;

    private Tween moveTween;
    public bool needUpdate = false;
    public bool isMoving = false;
    public bool isBusy = false;

    void Awake()
    {
        basePosition = transform.position;
    }

    void Start()
    {
        GameManager.Instance.OnDayStart += DayStart;
    }

    private void DayStart()
    {
        transform.position = basePosition;
    }

    public void Interrupt(float duration)
    {
        if (moveTween == null) return;
        StartCoroutine(WaitForInterrupt(duration));
    }

    private IEnumerator WaitForInterrupt(float waitTime)
    {
        moveTween.Pause();
        yield return new WaitForSeconds(waitTime);
        moveTween.Play();
    }

    private float currentAngle;

    public void TurnTowardCat(Action OnComplete)
    {
        if (rotationLocked)
        {
            currentAngle = transform.localScale.x;
            OnComplete();
            return;
        }
        Vector3 dir = RoomManager.Instance.cat.transform.position - transform.position;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (!body) body = gameObject.transform;
        currentAngle = body.transform.eulerAngles.z;
        float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
        float rotateDuration = Mathf.Abs(angleDiff) / defaultRotateSpeed;

        body.transform.DOKill();
        transform.DOKill();
        body.transform.DORotate(new Vector3(0, 0, targetAngle), rotateDuration, RotateMode.Fast).SetEase(defaultRotateEase)
        .OnComplete(() =>
        {
            OnComplete();
        });
    }

    public void ReturnToNormal(Action OnComplete)
    {
        if (rotationLocked)
        {
            FlipSprite(currentAngle);
            OnComplete();
            return;
        }
        float targetAngle = currentAngle;
        float oldAngle = body.transform.eulerAngles.z;
        float angleDiff = Mathf.DeltaAngle(oldAngle, targetAngle);
        float rotateDuration = Mathf.Abs(angleDiff) / defaultRotateSpeed;

        body.transform.DOKill();
        transform.DOKill();
        body.transform.DORotate(new Vector3(0, 0, targetAngle), rotateDuration, RotateMode.Fast).SetEase(defaultRotateEase)
        .OnComplete(() =>
        {
            OnComplete();
        });
    }

    public MoveStatus Move(MovementInfo info)
    {
        if (isMoving) return MoveStatus.Wait;
        if (isBusy) return MoveStatus.Wait;
        if (info == null) return MoveStatus.Failure;

        Vector3 dir = info.node.transform.position - transform.position;
        float distance = dir.magnitude;
        float moveDuration = distance / info.moveSpeed;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = body.transform.eulerAngles.z;
        float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
        float rotateDuration = Mathf.Abs(angleDiff) / info.rotationSpeed;

        if (!isMoving) needUpdate = true;
        isMoving = true;

        FlipSprite(info.node.transform);

        moveTween?.Kill();
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(info.node.transform.position, moveDuration).SetEase(info.moveEase));
        if (!rotationLocked) seq.Join(body.transform.DORotate(new Vector3(0, 0, targetAngle), rotateDuration, RotateMode.Fast).SetEase(info.rotateEase));
        moveTween = seq;
        seq.OnComplete(() =>
        {
            if (isMoving) needUpdate = true;
            isMoving = false;
        });
        return MoveStatus.Success;
    }

    private void FlipSprite(Transform Node)
    {
        if (!rotationLocked) return;
        Vector3 dir = (Node.position - transform.position).normalized;

        if (dir.x < 0 && transform.localScale.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (dir.x > 0 && transform.localScale.x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void FlipSprite(float flipX)
    {
        Vector3 scale = transform.localScale;
        scale.x = flipX;
        transform.localScale = scale;
    }
}

[Serializable]
public class MovementInfo
{
    [HideInInspector] public MovementNode node;
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    public Ease moveEase = Ease.Linear, rotateEase = Ease.Linear;
}

public enum MoveStatus
{
    Success, Failure, Wait
}

using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private Transform arrow;
    [SerializeField] private float speed = 5;
    [SerializeField] private float rotateDuration = 0.15f;

    public bool needUpdate = false, isMoving = false;

    private Vector3 basePosition, baseRotation;

    private Rigidbody2D _rb;
    private Tween rotateTween;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        basePosition = transform.position;
    }

    void Start()
    {
        GameManager.Instance.OnDayStart += DayStart;
        GameManager.Instance.OnDayEnd += DayStart;
        GameManager.Instance.OnScoreEnd += DayStart;
    }

    private void DayStart()
    {
        transform.position = basePosition;
        arrow.eulerAngles = baseRotation;
        _rb.linearVelocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        MovementControl();
    }

    private bool isDisable = false;
    public void DisableMovement()
    {
        isDisable = true;
    }
    public void EnableMovement()
    {
        isDisable = false;
    }

    private void MovementControl()
    {
        if (isDisable)
        {
            _rb.linearDamping = 40f;
            return;
        }
        Vector2 inputVel = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
            inputVel += Vector2.up;
        if (Input.GetKey(KeyCode.S))
            inputVel += Vector2.down;
        if (Input.GetKey(KeyCode.A))
            inputVel += Vector2.left;
        if (Input.GetKey(KeyCode.D))
            inputVel += Vector2.right;

        if (inputVel != Vector2.zero)
        {
            ChangeMoveState(true);
            inputVel.Normalize();
            _rb.linearDamping = 2;
            _rb.linearVelocity = inputVel * speed;

            float rawAngle = Mathf.Atan2(inputVel.y, inputVel.x) * Mathf.Rad2Deg;
            float snappedAngle = Mathf.Round(rawAngle / 45f) * 45f;
            rotateTween?.Kill();

            rotateTween = arrow
                .DORotate(new Vector3(0, 0, snappedAngle - 90f), rotateDuration, RotateMode.Fast)
                .SetEase(Ease.OutQuad);

            FlipSprite(inputVel);
        }
        else
        {
            ChangeMoveState(false);
            _rb.linearDamping = 40f;
        }
    }

    private void FlipSprite(Vector3 dir)
    {
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

    private void ChangeMoveState(bool isMove)
    {
        if (isMoving != isMove) needUpdate = true;
        isMoving = isMove;
    }
}

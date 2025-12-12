using DG.Tweening;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    [SerializeField] Transform cat;
    [SerializeField] private float speed = 5;
    [SerializeField] private float rotateDuration = 0.15f;

    private Rigidbody2D _rb;
    private Tween rotateTween;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovementControl();
    }

    private void MovementControl()
    {
        Vector3 delta = transform.position - cat.transform.position;
        Vector2 direction = new Vector2(delta.x, delta.y);
        direction.Normalize();
        if (direction != Vector2.zero)
        {
            direction.Normalize();
            _rb.linearDamping = 2;
            _rb.linearVelocity = direction * speed;

            float rawAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float snappedAngle = Mathf.Round(rawAngle / 45f) * 45f;
            rotateTween?.Kill();

            rotateTween = transform
                .DORotate(new Vector3(0, 0, snappedAngle - 90f), rotateDuration, RotateMode.Fast)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            _rb.linearDamping = 40f;
        }
    }
}

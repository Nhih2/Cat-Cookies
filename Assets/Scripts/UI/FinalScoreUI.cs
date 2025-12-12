using DG.Tweening;
using TMPro;
using UnityEngine;

public class FinalScoreUI : MonoBehaviour
{
    private float basePosition, activatedPosition, minorPosition;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.Linear;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        activatedPosition = rect.localPosition.x;
        basePosition = activatedPosition - 1500;
        minorPosition = activatedPosition + 100;
    }

    void Start()
    {
        GameManager.Instance.OnDayStart += OnDayStart;
        GameManager.Instance.OnGameStart += OnDayStart;
        GameManager.Instance.OnScoreEnd += OnDayStart;
    }

    private void OnDayStart()
    {
        transform.localPosition = new(basePosition, transform.localPosition.y, transform.localPosition.z);
    }

    public void Activate(string text, bool isMinor)
    {
        this.text.text = text;
        float targetPosition = activatedPosition;
        if (isMinor) targetPosition = minorPosition;
        rect.DOKill();
        rect.DOLocalMoveX(targetPosition, duration).SetEase(ease);
    }
}

using DG.Tweening;
using TMPro;
using UnityEngine;

public class RatingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI header, rating;

    private CanvasGroup _renderer;

    void Start()
    {
        _renderer = GetComponent<CanvasGroup>();
        GameManager.Instance.OnDayStart += OnDayStart;
        GameManager.Instance.OnGameStart += OnDayStart;
        GameManager.Instance.OnScoreEnd += OnDayStart;
    }

    private void OnDayStart()
    {
        header.text = "";
        rating.text = "";
    }

    public void Activate(string rate)
    {
        transform.localScale = new(1.5f, 1.5f, 1.5f);
        header.text = "Day Rating";
        rating.text = rate;
        _renderer.alpha = 0;

        Sequence seq = DOTween.Sequence();
        seq.Append(_renderer.DOFade(1, 0.1f));
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOScale(new Vector3(1, 1, 1), 0.2f));
    }
}

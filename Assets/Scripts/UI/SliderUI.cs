using UnityEngine;
using DG.Tweening;

public class SliderUI : MonoBehaviour
{
    [SerializeField] private float maxWidth = 2.76f, tweenDuration = 0.25f;
    [SerializeField] private RectTransform sliderFill;
    [SerializeField] private Ease TestEase = Ease.OutQuart;

    public void SetValue(float progressPercent)
    {
        sliderFill.DOKill();

        progressPercent = Mathf.Clamp(progressPercent, 0, 1);
        float targetWidth = progressPercent * maxWidth;

        Sequence seq = DOTween.Sequence();

        seq.Append(sliderFill.DOSizeDelta(
            new Vector2(targetWidth, sliderFill.sizeDelta.y),
            tweenDuration
        ).SetEase(TestEase));

        Vector2 targetPos = sliderFill.anchoredPosition;
        targetPos.x = targetWidth * sliderFill.pivot.x;

        seq.Join(sliderFill.DOAnchorPosX(targetPos.x, tweenDuration).SetEase(TestEase));
    }

    public void SetValueNoAnim(float progressPercent)
    {
        sliderFill.DOKill();
        progressPercent = Mathf.Clamp(progressPercent, 0, 1);

        float targetWidth = progressPercent * maxWidth;
        sliderFill.sizeDelta = new Vector2(targetWidth, sliderFill.sizeDelta.y);

        Vector2 targetPos = sliderFill.anchoredPosition;
        targetPos.x = targetWidth * sliderFill.pivot.x;
        sliderFill.anchoredPosition = new(targetPos.x, sliderFill.anchoredPosition.y);
    }
}

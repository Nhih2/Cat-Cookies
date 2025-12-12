using DG.Tweening;
using UnityEngine;

public class FadeInAndFadeOut : MonoBehaviour
{
    [SerializeField] private float fadeLow = 0.25f, fadeDuration = 2f, fadeInterval = 0.5f;
    private CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(group.DOFade(fadeLow, fadeDuration));
        seq.AppendInterval(fadeInterval);
        seq.Append(group.DOFade(1, fadeDuration));
        seq.AppendInterval(fadeInterval);
        seq.SetLoops(-1);
    }
}

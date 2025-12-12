using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class HumanExpression : MonoBehaviour
{
    [SerializeField] private GameObject expressionHolder;
    [SerializeField] private float tweenDuration = 0.5f, tweenWait = 0.5f, tiltAngle = 15;
    [SerializeField] private Vector3 extendedScale = new(1.5f, 1.5f, 1.5f), baseScale = new(1, 1, 1);
    [SerializeField] private List<SpriteRenderer> renderers;

    private Queue<Expression> renderedExpression = new();
    public Action OnComplete;
    private bool isRendering = false;
    private Sequence tween;


    void Start()
    {
        SetTransparencyAll(0);
    }

    private void SetTransparencyAll(float alpha)
    {
        foreach (SpriteRenderer renderer in renderers) GameSetting.SetTransparency(renderer, alpha);
    }

    private void KillAll()
    {
        foreach (SpriteRenderer renderer in renderers) renderer.DOKill();
    }

    public void OverrideQueue()
    {
        DebugSys.Log("overriden");
        isRendering = false;
        renderedExpression.Clear();
        expressionHolder.transform.DOKill();
        SetTransparencyAll(0);
        KillAll();
        DOTween.Kill(10);
    }

    public void Enqueue(Expression expression)
    {
        if (isRendering) return;
        renderedExpression.Enqueue(expression);
    }

    public void ShowExpression(bool isChain = false)
    {
        if (!isChain && isRendering) return;
        if (renderedExpression.Count == 0)
        {
            if (OnComplete != null) OnComplete();
            isRendering = false;
            OnComplete = null;
            return;
        }
        isRendering = true;
        Expression expression = renderedExpression.Dequeue();
        SpriteRenderer renderer = renderers[(int)expression];

        expressionHolder.transform.DOKill();
        renderer.DOKill();
        if (tween != null) DOTween.Kill(tween);
        SetTransparencyAll(0);

        Sequence seq = DOTween.Sequence();
        tween = seq;
        if (expression == Expression.Notice || expression == Expression.Angry)
        {
            DebugSys.Log("Expressing: " + expression);
            expressionHolder.transform.localScale = baseScale;
            GameSetting.SetTransparency(renderer, 0);
            seq.Append(expressionHolder.transform.DOScale(extendedScale, tweenDuration / 4f).SetEase(Ease.InOutBack));
            seq.Join(renderer.DOFade(1, tweenDuration / 4f));
            seq.Append(expressionHolder.transform.DOScale(baseScale, tweenDuration / 2f).SetEase(Ease.InOutBack));
            seq.AppendInterval(tweenWait / 1.5f);
            seq.Append(renderer.DOFade(0, tweenDuration / 2f));

            if (expression == Expression.Angry)
            {
                expressionHolder.transform.DOPunchPosition(new(0.5f, 0, 0), tweenDuration / 1.5f, 10, 0.8f);
            }
        }
        else if (expression == Expression.Happy)
        {
            expressionHolder.transform.localScale = baseScale;
            GameSetting.SetTransparency(renderer, 0);
            renderer.DOFade(1, tweenDuration / 4f);
            seq.Append(expressionHolder.transform.DORotate(new Vector3(0, 0, tiltAngle), tweenDuration / 2).SetEase(Ease.InOutBack));
            seq.Append(expressionHolder.transform.DORotate(new Vector3(0, 0, -tiltAngle), tweenDuration).SetEase(Ease.InOutBack));
            seq.Append(expressionHolder.transform.DORotate(new Vector3(0, 0, tiltAngle), tweenDuration).SetEase(Ease.InOutBack));
            seq.Append(expressionHolder.transform.DORotate(Vector3.zero, tweenDuration / 2).SetEase(Ease.InOutBack).OnComplete(() =>
            {
                renderer.DOFade(0, tweenDuration / 4f);
            }));
        }
        seq.OnComplete(() => { ShowExpression(true); });
    }
}

public enum Expression
{
    Notice, Angry, Happy
}

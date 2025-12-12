using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        textHolder.alpha = 0;
        currentScore = 0;
        txt_Score.text = currentScore + "";
        txt_AddedScore.text = "";
    }

    [SerializeField] private float waitToFade = 5f, waitToBonusFade = 1f, scaleTweenDuration = 0.2f, fadeDuration = 0.8f;
    [SerializeField] private float scaleOvershoot = 1.2f;
    [SerializeField] private Ease tweenEase = Ease.OutCubic;
    [SerializeField] private CanvasGroup textHolder;
    [SerializeField] private TextMeshProUGUI txt_Score, txt_AddedScore;

    public Dictionary<TaskScoreType, int> taskDoneDict = new();

    private int currentScore = 0;
    private int displayedScore = 0;
    private Tween scoreTween;

    public int score => currentScore;

    private int tweenId = 0;

    void Start()
    {
        GameManager.Instance.OnDayEnd += DayEnd;
    }

    private void DayEnd()
    {
        currentScore += SastisfactoryManager.Instance.sastisfactory;
    }

    public void StartGame()
    {
        textHolder.alpha = 0;
        currentScore = 0;
        txt_Score.text = currentScore + "";
        txt_AddedScore.text = "";
    }

    public void StartDay()
    {
        taskDoneDict = new();
        currentScore = 0;
        foreach (TaskScoreType type in Enum.GetValues(typeof(TaskScoreType)))
            taskDoneDict.Add(type, 0);
    }

    public void AddScore(int amount, TaskScoreType type)
    {
        float mul = BonusManager.Instance.TriggerBonus();
        int oldScore = currentScore;
        currentScore += (int)(amount * mul);
        taskDoneDict[type] += (int)(amount * mul);

        textHolder.alpha = 1;

        scoreTween?.Kill();
        scoreTween = DOTween.To(
            () => displayedScore,
            x =>
                {
                    displayedScore = x;
                    txt_Score.text = displayedScore.ToString();
                },
            currentScore,
            0.5f).SetEase(tweenEase);

        txt_AddedScore.DOKill();
        txt_AddedScore.alpha = 1f;
        txt_AddedScore.rectTransform.localScale = Vector3.one;

        txt_AddedScore.text = $"+{(amount * mul).FormatFloat()}";
        if (mul > 1)
            txt_AddedScore.text += $" (x{mul.FormatFloat()})";

        DOTween.Kill(tweenId);
        textHolder.DOKill();
        txt_AddedScore.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(txt_AddedScore.rectTransform.DOScale(scaleOvershoot, scaleTweenDuration));
        seq.Append(txt_AddedScore.rectTransform.DOScale(1f, scaleTweenDuration));
        seq.AppendInterval(waitToBonusFade);
        seq.Join(txt_AddedScore.DOFade(0, fadeDuration));
        seq.SetId(tweenId);

        Sequence seq2 = DOTween.Sequence();
        seq2.AppendInterval(waitToFade);
        seq2.Append(textHolder.DOFade(0, fadeDuration));
        seq2.SetId(tweenId);
    }
}

public enum TaskScoreType
{
    Clean, WaterPlant, Fix, Biscuit, Other,
    LotBiscuit, Bug, Mouse, Cooking
}
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField] private float maxBonusTimer = 10, minBonusTimer = 5, BonusDecay = 1;
    [SerializeField]
    private List<float> bonusMul = new()
    {
        1, 1.5f, 2f, 3f, 4f, 5f
    };
    [SerializeField] private SliderUI sliderUI;
    [SerializeField] private TextMeshProUGUI bonusText;

    private GameObject textHolder;

    private float bonusMultiplier = 1, bonusTimerDelay = 0.1f;
    private float countdownTick, countdownMaxTimer;
    private float minCountdown = -0.5f;
    private int bonusTriggeredTimes = 0;
    private bool needUpdate = false;

    void Start()
    {
        textHolder = bonusText.transform.parent.gameObject;
        bonusMultiplier = bonusMul[0]; bonusTriggeredTimes = 0;
        countdownTick = minCountdown; needUpdate = false;
        sliderUI.gameObject.SetActive(false);
        textHolder.SetActive(false);
    }

    public void StartGame()
    {
    }

    public void StartDay()
    {
        bonusMultiplier = bonusMul[0]; bonusTriggeredTimes = 0;
        countdownTick = minCountdown; needUpdate = false;
        sliderUI.gameObject.SetActive(false);
        textHolder.SetActive(false);
    }

    public float TriggerBonus()
    {
        if (bonusTriggeredTimes >= bonusMul.Count) bonusMultiplier = bonusMul[bonusMul.Count - 1];
        else bonusMultiplier = bonusMul[bonusTriggeredTimes];
        bonusTriggeredTimes++;
        StartCountdown();
        return bonusMultiplier;
    }

    private void StartCountdown()
    {
        if (countdownTick <= minCountdown)
            StartCoroutine(CountdownBonus());
        else CalculateBonusTimer();
    }

    private void CalculateBonusTimer()
    {
        countdownMaxTimer = maxBonusTimer;
        countdownMaxTimer -= (bonusTriggeredTimes - 1) * BonusDecay;
        countdownMaxTimer = Mathf.Max(minBonusTimer, countdownMaxTimer);
        countdownTick = countdownMaxTimer;
        needUpdate = true;
        DebugSys.Log($"{countdownTick} {countdownMaxTimer} {bonusTriggeredTimes}");
        ShowText();
    }

    private IEnumerator CountdownBonus()
    {
        CalculateBonusTimer();

        sliderUI.gameObject.SetActive(true);
        while (countdownTick > minCountdown)
        {
            sliderUI.SetValue(countdownTick / countdownMaxTimer);
            if (needUpdate)
            {
                needUpdate = false;
                yield return new WaitForSeconds(bonusTimerDelay);
            }
            yield return new WaitForSeconds(bonusTimerDelay);
            countdownTick -= bonusTimerDelay;
        }
        sliderUI.gameObject.SetActive(false);
        bonusTriggeredTimes = 0;
    }

    public void ShowText(float duration = 0.5f, float overshootScale = 1.2f)
    {
        bonusText.text = $"x{bonusMultiplier.FormatFloat()}";

        Transform t = textHolder.transform;
        Vector3 originalScale = t.localScale;

        t.DOKill();
        t.localScale = Vector3.zero;
        textHolder.SetActive(true);

        t.DOScale(originalScale * overshootScale, duration * 0.6f)
         .SetEase(Ease.OutBack)
         .OnComplete(() =>
         {
             t.DOScale(originalScale, duration * 0.4f).SetEase(Ease.OutCubic);
         });
    }
}

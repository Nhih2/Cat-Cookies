using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SastisfactoryManager : MonoBehaviour
{
    public static SastisfactoryManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField] private Sprite Angry, Neutral, Happy;
    [SerializeField] private Image Face;
    [SerializeField] private float fadeDuration = 0.8f, waitToFade = 5f;
    [SerializeField] private int minSastisfactory = -1500, maxSastisfactory = 2500;
    [SerializeField] private CanvasGroup sliderHolder;
    [SerializeField] private SliderUI sliderUI;

    private int currentSastisfactory;
    public int sastisfactory => currentSastisfactory;

    void Start()
    {
        currentSastisfactory = 0;
        sliderHolder.alpha = 1;
        sliderUI.SetValueNoAnim((-minSastisfactory + 0f) / (maxSastisfactory - minSastisfactory));
        GameManager.Instance.OnDayEnd += DayEnd;
    }

    private void DayEnd()
    {
        if (currentSastisfactory <= -500)
        {
            GameManager.Instance.GameOver_Annoyed();
        }
    }

    public void StartGame()
    {

    }

    public void StartDay()
    {
        Face.sprite = Neutral;
        currentSastisfactory = 0;
        sliderHolder.alpha = 1;
        sliderUI.SetValueNoAnim((-minSastisfactory + 0f) / (maxSastisfactory - minSastisfactory));
    }

    public void AddSastisfactory(int amount)
    {
        int humans = RoomManager.Instance.HumansInCatRoom();
        if (humans == 0) return;
        if (amount > 0) RoomManager.Instance.HumanWitness();

        currentSastisfactory += amount * humans;
        sliderHolder.alpha = 1;

        float percentil = (currentSastisfactory - minSastisfactory + 0f) / (maxSastisfactory - minSastisfactory);
        DebugSys.Log("Current Sastisfactory: " + currentSastisfactory + " /percentil: " + percentil);
        sliderUI.SetValue(percentil);

        transform.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(waitToFade);
        //seq.Append(sliderHolder.DOFade(0, fadeDuration)).SetTarget(transform);

        if (currentSastisfactory >= -500 && currentSastisfactory < 500) Face.sprite = Neutral;
        else if (currentSastisfactory >= 500) Face.sprite = Happy;
        else if (currentSastisfactory <= -500) Face.sprite = Angry;
        if (currentSastisfactory <= -1500)
        {
            GameManager.Instance.GameOver_Anger();
        }
        if (currentSastisfactory > 2500) currentSastisfactory = 2500;
    }
}

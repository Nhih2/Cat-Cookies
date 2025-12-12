using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField] private BonusManager bonus;
    [SerializeField] private SastisfactoryManager sastisfactory;
    [SerializeField] private ScoreManager score;
    [SerializeField] private TimerManager time;
    [SerializeField] private RoomManager room;
    [SerializeField] private SpriteRenderer curtain;
    [SerializeField] private GameObject firstTask;

    public event Action OnDayStart, OnDayEnd, OnGameStart, OnScoreEnd;
    public bool isHardcore = false;
    private PlayerManager cat => room.cat;

    void Start()
    {
        StartCoroutine(WaitToStartGame());
    }

    private IEnumerator WaitToStartGame()
    {
        yield return null;
        StartGame();
    }

    public void RestartGame()
    {
        DebugSys.Log("RestartGame");
        OnScoreEnd?.Invoke();
        StartCoroutine(WaitToStartGame());
    }

    public void GameOver_Anger()
    {
        DebugSys.Log("GameOver");
        TimerManager.Instance.EndDay();
        OnScoreEnd?.Invoke();
        cat.DisablePlayer();
        CutsceneManager.Instance.PlayCutscene_KickedAnger(() =>
        {
            RestartGame();
        });
    }

    public void GameOver_Annoyed()
    {
        cat.DisablePlayer();
        CutsceneManager.Instance.PlayCutscene_KickedAnnoyed(() =>
        {
            RestartGame();
        });
    }

    private void StartGame()
    {
        cat.DisablePlayer();
        OnGameStart?.Invoke();
        //
        bonus.StartGame();
        sastisfactory.StartGame();
        score.StartGame();
        //
        CutsceneManager.Instance.PlayCutscene_Opening(() =>
        {
            time.StartGame();
            StartCoroutine(WaitForStart());
        });
    }

    public void NextDay()
    {
        if (ScoreManager.Instance.taskDoneDict[TaskScoreType.LotBiscuit] > 0)
        {
            isHardcore = true;
            CutsceneManager.Instance.PlayCutscene_Hardcore(() =>
            {
                OnScoreEnd?.Invoke();
                StartCoroutine(WaitForStart());
            });
        }
        else if (!isHardcore && TimerManager.Instance.GetDay() == 1)
        {
            CutsceneManager.Instance.PlayCutscene_Desire(() =>
            {
                OnScoreEnd?.Invoke();
                StartCoroutine(WaitForStart());
            });
        }
        else
        {
            OnScoreEnd?.Invoke();
            StartCoroutine(WaitForStart());
        }
    }

    private void EndDay()
    {
        time.EndDay();
        cat.DisablePlayer();
    }

    void Update()
    {
        if (time.GetSecondLeft() <= 0)
        {
            EndDay();
            OnDayEnd?.Invoke();
        }
    }

    private bool hasAssigned = false;

    private IEnumerator WaitForStart()
    {
        cat.EnablePlayer();
        cat.DisableMovement();
        curtain.SetTransparency(1);
        curtain.gameObject.SetActive(true);
        firstTask.SetActive(true);
        while (cat.GetCurrentTask() == null) yield return null;
        DebugSys.Log("Got Task: " + cat.GetCurrentTask().gameObject.name);
        if (!hasAssigned)
        {
            hasAssigned = true;
            cat.GetCurrentTask().GetCurrentTask().onTaskProgress += OnFirstTaskProgress;
            cat.GetCurrentTask().OnTaskDone += () =>
            {
                cat.EnableMovement();
                curtain.gameObject.SetActive(false);
                firstTask.SetActive(false);
                StartDay();
            };
        }
    }

    private void OnFirstTaskProgress(float progress)
    {
        curtain.SetTransparency(1 - progress * 0.15f);
    }

    private void StartDay()
    {
        OnDayStart?.Invoke();
        //
        bonus.StartDay();
        sastisfactory.StartDay();
        time.StartDay();
        score.StartDay();
        //
        GameSetting.TaskChainDictionary = new();
    }

}

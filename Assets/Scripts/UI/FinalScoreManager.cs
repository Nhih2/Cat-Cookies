using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FinalScoreManager : MonoBehaviour
{
    [SerializeField] private Image Background;
    [SerializeField] private GameObject ClickToContinue;
    [SerializeField] private List<FinalScoreUI> scoreUIs;
    [SerializeField] private RatingUI ratingUI;

    private bool isNewDay = false;

    void Start()
    {
        GameManager.Instance.OnDayEnd += DayEnd;
        GameManager.Instance.OnDayStart += OnDayStart;
        GameManager.Instance.OnGameStart += OnDayStart;
        GameManager.Instance.OnScoreEnd += OnDayStart;
    }

    void Update()
    {
        if (isNewDay && Input.GetMouseButton(0))
        {
            isNewDay = false;
            GameManager.Instance.NextDay();
        }
    }

    private void OnDayStart()
    {
        Background.SetTransparency(0);
        ClickToContinue.SetActive(false);
    }

    private void DayEnd()
    {
        StartCoroutine(DayEndAnim());
    }

    private IEnumerator DayEndAnim()
    {
        if (!GameManager.Instance.isHardcore)
        {
            Background.SetTransparency(1);
            List<string> headers = new()
        {
            "Mouse and Mouse Caught", "All Biscuits Eaten", "Others", "Sastisfactory Bonus", "Total Score"
        };
            string text = headers[0];
            text += ": " + (ScoreManager.Instance.taskDoneDict[TaskScoreType.Bug] + ScoreManager.Instance.taskDoneDict[TaskScoreType.Mouse]);
            scoreUIs[0].Activate(text, false);
            yield return new WaitForSeconds(0.25f);
            //
            text = headers[1];
            text += ": " + (ScoreManager.Instance.taskDoneDict[TaskScoreType.Biscuit] + ScoreManager.Instance.taskDoneDict[TaskScoreType.LotBiscuit]);
            scoreUIs[1].Activate(text, false);
            yield return new WaitForSeconds(0.25f);
            //
            text = headers[2];
            text += ": " + ScoreManager.Instance.taskDoneDict[TaskScoreType.Other];
            scoreUIs[2].Activate(text, false);
            yield return new WaitForSeconds(0.25f);
            //
            text = headers[3];
            text += ": " + SastisfactoryManager.Instance.sastisfactory;
            scoreUIs[3].Activate(text, false);
            yield return new WaitForSeconds(0.25f);
            //
            text = headers[4];
            text += ": " + ScoreManager.Instance.score;
            scoreUIs[4].Activate(text, false);
            yield return new WaitForSeconds(0.25f);
        }
        else
        {
            Background.SetTransparency(1);
            List<string> headers = new()
        {
            "Chores Done", "All Biscuits Eaten", "Others", "Satisfactory Bonus", "Total Score"
        };
            string text = headers[0];
            text += ": " + (ScoreManager.Instance.taskDoneDict[TaskScoreType.Bug] + ScoreManager.Instance.taskDoneDict[TaskScoreType.Mouse]
            + ScoreManager.Instance.taskDoneDict[TaskScoreType.Clean] + ScoreManager.Instance.taskDoneDict[TaskScoreType.Cooking]
            + ScoreManager.Instance.taskDoneDict[TaskScoreType.Fix] + ScoreManager.Instance.taskDoneDict[TaskScoreType.WaterPlant]);
            scoreUIs[0].Activate(text, false);
            yield return new WaitForSeconds(0.25f);
            //
            text = headers[1];
            text += ": " + (ScoreManager.Instance.taskDoneDict[TaskScoreType.Biscuit] + ScoreManager.Instance.taskDoneDict[TaskScoreType.LotBiscuit]);
            scoreUIs[1].Activate(text, false);
            yield return new WaitForSeconds(0.25f);
            //
            text = headers[2];
            text += ": " + ScoreManager.Instance.taskDoneDict[TaskScoreType.Other];
            scoreUIs[2].Activate(text, false);
            yield return new WaitForSeconds(0.25f);
            //
            text = headers[3];
            text += ": " + SastisfactoryManager.Instance.sastisfactory;
            scoreUIs[3].Activate(text, false);
            yield return new WaitForSeconds(0.25f);
            //
            text = headers[4];
            text += ": " + ScoreManager.Instance.score;
            scoreUIs[4].Activate(text, false);
            yield return new WaitForSeconds(0.25f);
        }
        string rate = "F";
        if (ScoreManager.Instance.score <= 500) rate = "F";
        else if (ScoreManager.Instance.score <= 1000) rate = "E";
        else if (ScoreManager.Instance.score <= 2500) rate = "D";
        else if (ScoreManager.Instance.score <= 5000) rate = "C";
        else if (ScoreManager.Instance.score <= 10000) rate = "B";
        else if (ScoreManager.Instance.score <= 25000) rate = "A";
        else if (ScoreManager.Instance.score <= 50000) rate = "S";
        else if (ScoreManager.Instance.score <= 100000) rate = "SS";
        else if (ScoreManager.Instance.score <= 250000) rate = "SSS";
        else if (ScoreManager.Instance.score <= 500000) rate = "SSS+";
        else if (ScoreManager.Instance.score <= 1000000) rate = "EX";
        ratingUI.Activate(rate);
        yield return new WaitForSeconds(0.2f);
        ClickToContinue.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        transform.DOPunchPosition(new Vector3(100, 10f, 0), 1f, 15, 0.8f);
        yield return new WaitForSeconds(1f);
        isNewDay = true;
    }
}

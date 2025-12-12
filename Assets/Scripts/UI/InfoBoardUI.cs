using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class InfoBoardUI : MonoBehaviour
{
    [SerializeField] private Vector3 baseScale = new(1, 1, 1);
    [SerializeField] private float TweenDuration = 0.5f;
    [SerializeField] private Ease ShowTweenEase, HideTweenEase;
    [SerializeField] private TextMeshProUGUI Header, ScoreHeader, SastisHeader;
    [SerializeField] private TextMeshProUGUI score, sastisfactory;
    [SerializeField] private GameObject DisplayHolder;

    public void ShowInfoBoard(Task task)
    {
        DisplayHolder.SetActive(true);
        SetInformation(task);
        SetTransform(task.transform);
    }

    public void HideInfoBoard()
    {
        DisplayHolder.transform.DOKill();
        DisplayHolder.transform.DOScale(new Vector3(0, 0, 0), TweenDuration).SetEase(ShowTweenEase);
    }

    private void SetInformation(Task task)
    {
        Header.text = task.gameObject.name;
        if (task.IsNaughtyTask()) Header.color = Color.red;
        else Header.color = Color.black;
        if (task.GetScore() != 0)
        {
            ScoreHeader.text = "Score";
            score.text = task.GetScore() + "";
        }
        else
        {
            score.text = "";
            ScoreHeader.text = "";
        }
        if (task.GetSastisfaction() != 0)
        {
            SastisHeader.text = "Satisfactory";

            if (task.GetSastisfaction() > 0) sastisfactory.text = task.GetSastisfaction() + "";
            else sastisfactory.text = $"<shake a=0.012><color=red>{task.GetSastisfaction()}</color></shake>";

        }
        else
        {
            sastisfactory.text = "";
            SastisHeader.text = "";
        }
    }

    private void SetTransform(Transform task)
    {
        float downScaled = 1.25f;
        DisplayHolder.transform.position = task.position + new Vector3(0, task.localScale.y / downScaled);
        DisplayHolder.transform.SetParent(null);

        SetWorldScale(DisplayHolder.transform, Vector3.zero);

        DisplayHolder.transform.DOKill();
        DisplayHolder.transform.DOScale(baseScale, TweenDuration).SetEase(ShowTweenEase);
    }

    private void SetWorldScale(Transform t, Vector3 worldScale)
    {
        if (t.parent == null)
        {
            t.localScale = worldScale;
        }
        else
        {
            Vector3 parentScale = t.parent.lossyScale;
            t.localScale = new Vector3(
                worldScale.x / parentScale.x,
                worldScale.y / parentScale.y,
                worldScale.z / parentScale.z
            );
        }
    }
}

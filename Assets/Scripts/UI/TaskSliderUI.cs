using UnityEngine;
using DG.Tweening;

public class TaskSliderUI : MonoBehaviour
{
    [SerializeField] private InteractUI interactUI;
    [SerializeField] private Vector3 baseLocalPos = new(0, 0, 0), rightOffset = new(0, 0.25f);
    [SerializeField] private Vector3 baseScale = new(1, 1, 1), rightScale = new(0.8f, 1);

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetType(TaskType type, bool isActivated)
    {
        rect.gameObject.SetActive(true);
        interactUI.SetType(type, isActivated);
        if (type == TaskType.TapAndWait && isActivated)
        {
            rect.localPosition = baseLocalPos;
            rect.localScale = baseScale;
        }
        else if (type == TaskType.TapAndWait)
        {
            rect.gameObject.SetActive(false);
        }
        else if (type == TaskType.Hold || type == TaskType.RepeatedTap)
        {
            rect.localPosition = rightOffset;
            rect.localScale = rightScale;
        }
    }
}

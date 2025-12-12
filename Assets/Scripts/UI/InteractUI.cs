using UnityEngine;
using DG.Tweening;
using System.Collections;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private Vector3 baseLocalPos, leftLocalPos;
    [SerializeField] private GameObject UI;
    [Space]
    [Header("Button")]
    [SerializeField] private GameObject interactUI;
    [SerializeField] private Vector3 unpressedLocalPos = new(-0.04f, -0.04f), pressedLocalPos = new(0.02f, 0.02f);
    [SerializeField] private float pressDelay = 0.5f, holdDuration = 1f, repeatedTap = 0.2f, pressInterval = 0.75f, holdInterval = 1f;

    private TaskType currentType = TaskType.TapAndWait;

    void Start()
    {
        StartCoroutine(StartButtonAnim());
    }

    public void SetType(TaskType type, bool isActivated)
    {
        currentType = type;
        UI.transform.gameObject.SetActive(true);
        if (type == TaskType.TapAndWait && isActivated)
        {
            UI.transform.gameObject.SetActive(false);
        }
        else if (type == TaskType.TapAndWait)
        {
            UI.transform.localPosition = baseLocalPos;
        }
        else if (type == TaskType.Hold || type == TaskType.RepeatedTap)
        {
            UI.transform.localPosition = leftLocalPos;
        }
    }

    private IEnumerator StartButtonAnim()
    {
        interactUI.transform.localPosition = unpressedLocalPos;
        while (true)
        {
            if (currentType == TaskType.TapAndWait)
            {
                interactUI.transform.localPosition = pressedLocalPos;
                yield return new WaitForSeconds(pressDelay);
                interactUI.transform.localPosition = unpressedLocalPos;
                yield return new WaitForSeconds(pressInterval);
            }
            if (currentType == TaskType.Hold)
            {
                interactUI.transform.localPosition = pressedLocalPos;
                yield return new WaitForSeconds(holdDuration);
                interactUI.transform.localPosition = unpressedLocalPos;
                yield return new WaitForSeconds(holdInterval);
            }
            if (currentType == TaskType.RepeatedTap)
            {
                interactUI.transform.localPosition = pressedLocalPos;
                yield return new WaitForSeconds(repeatedTap);
                interactUI.transform.localPosition = unpressedLocalPos;
                yield return new WaitForSeconds(repeatedTap);
            }
            yield return null;
        }
    }
}

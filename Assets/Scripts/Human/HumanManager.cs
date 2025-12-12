using DG.Tweening;
using UnityEngine;

public class HumanManager : MonoBehaviour
{
    [SerializeField] private Transform body;
    private HumanController controller;
    private HumanRoom room;
    private HumanExpression expression;
    private HumanAnim anim;

    public RoomType currentRoom => room.currentRoom;

    void Awake()
    {
        controller = GetComponent<HumanController>();
        room = GetComponent<HumanRoom>();
        expression = GetComponent<HumanExpression>();
        anim = GetComponent<HumanAnim>();
        controller.SetBody(body);
    }

    void Start()
    {
        RoomManager.Instance.humans.Add(this);
    }

    void Update()
    {
        if (controller.needUpdate)
        {
            controller.needUpdate = false;
            if (controller.isMoving) anim.PlayAnim();
            else anim.StopAnim();
        }
    }

    public void WitnessTask()
    {
        DebugSys.Log("Witnessed");
        expression.Enqueue(Expression.Happy);
        expression.ShowExpression();
    }

    public void CaughtNaughtyTask()
    {
        DebugSys.Log("Caught");
        controller.isBusy = true;
        RoomManager.Instance.cat.DisablePlayer();

        EventCameraController.Instance.StartEvent(transform);
        anim.StopIdle();

        expression.OverrideQueue();
        expression.OnComplete = () =>
        {
            controller.ReturnToNormal(() =>
            {
                controller.isBusy = false;
                RoomManager.Instance.cat.EnablePlayer();
                EventCameraController.Instance.EndEvent(transform);
                anim.PlayAnim();
            });
        };
        controller.TurnTowardCat(() =>
        {
            expression.Enqueue(Expression.Notice);
            expression.Enqueue(Expression.Angry);
            expression.ShowExpression();
        });
    }
}

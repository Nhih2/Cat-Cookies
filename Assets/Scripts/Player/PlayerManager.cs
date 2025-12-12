using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform body;

    private PlayerScan scan;
    private PlayerInteraction interaction;
    private PlayerMovement movement;
    private PlayerInteractionDisplay display;
    private PlayerRoom room;
    private PlayerAnim anim;

    void Awake()
    {
        scan = GetComponent<PlayerScan>();
        interaction = GetComponent<PlayerInteraction>();
        movement = GetComponent<PlayerMovement>();
        display = GetComponent<PlayerInteractionDisplay>();
        room = GetComponent<PlayerRoom>();
        anim = GetComponent<PlayerAnim>();
    }

    void Start()
    {
        display.HideDisplay();
    }

    void Update()
    {
        if (movement.needUpdate)
        {
            movement.needUpdate = false;
            if (movement.isMoving) anim.PlayAnim();
            else anim.StopIdle();
        }
        if (scan.needUpdate)
        {
            room.UpdateNewRoom();
            RoomManager.Instance.currentCatTask = scan.targetTask;
            interaction.targetTask = scan.targetTask;
            scan.needUpdate = false;
            if (scan.targetTask == null) display.HideDisplay();
            else if (!scan.targetTask.CheckAvailibility()) display.HideDisplay();
            else display.ShowDisplay(scan.targetTask);
        }
    }

    public Task GetCurrentTask() => scan.targetTask;

    public void DisableMovement()
    {
        movement.DisableMovement();
    }

    public void EnableMovement()
    {
        movement.EnableMovement();
    }

    public void DisablePlayer()
    {
        movement.DisableMovement();
        interaction.DisableMovement();
    }

    public void EnablePlayer()
    {
        movement.EnableMovement();
        interaction.EnableMovement();
    }
}

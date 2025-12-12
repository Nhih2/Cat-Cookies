using UnityEngine;

public class PlayerRoom : MonoBehaviour
{
    [SerializeField] private float hitboxRadius = 0.1f;

    private LayerMask roomLayer;
    private RoomType currentRoom = RoomType.Living;

    void Awake()
    {
        roomLayer = LayerMask.GetMask(GameSetting.ROOM_LAYER);
    }

    void Start()
    {
        UpdateNewRoom();
    }

    void FixedUpdate()
    {
        CheckRoom();
    }

    private void CheckRoom()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitboxRadius, roomLayer);
        foreach (Collider2D hit in hits)
        {
            Room room = hit.GetComponent<Room>();
            if (!room) continue;
            if (room.GetRoomType() == currentRoom) continue;

            currentRoom = room.GetRoomType();
            UpdateNewRoom();
            break;
        }
    }

    public void UpdateNewRoom()
    {
        //DebugSys.Log("UpdateRoom: " + currentRoom);
        RoomManager.Instance.UpdateCurrentRoom(currentRoom);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, hitboxRadius);
    }
}

using System;
using UnityEngine;

public class HumanRoom : MonoBehaviour
{
    [SerializeField] private float hitboxRadius = 0.1f;

    private LayerMask roomLayer;
    public RoomType currentRoom { get; private set; } = RoomType.None;

    void Awake()
    {
        roomLayer = LayerMask.GetMask(GameSetting.ROOM_LAYER);
    }

    void Start()
    {
        CheckRoom(true);
    }

    void FixedUpdate()
    {
        CheckRoom();
    }

    private void CheckRoom(bool isStart = false)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitboxRadius, roomLayer);
        foreach (Collider2D hit in hits)
        {
            Room room = hit.GetComponent<Room>();
            if (!room) continue;
            if (room.GetRoomType() == currentRoom && !isStart) continue;

            currentRoom = room.GetRoomType();
            break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, hitboxRadius);
    }
}

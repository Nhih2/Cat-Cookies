using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    //

    [SerializeField] private List<Room> rooms;

    public PlayerManager cat;
    public List<HumanManager> humans = new();
    public RoomType currentCatRoom { get; private set; } = RoomType.Living;
    public bool isCatDoingNaughtyTask => currentCatTask ? (currentCatTask.IsNaughtyTask() && currentCatTask.IsWorking()) : false;
    public Task currentCatTask;

    void Start()
    {
        foreach (Room room in rooms) room.OnSwitch += UpdateCurrentRoom;
    }

    void Update()
    {
        if (isCatDoingNaughtyTask)
        {
            foreach (HumanManager human in humans)
                if (human.currentRoom == currentCatRoom)
                {
                    CaughtNaughtyTask();
                    break;
                }
        }
    }

    private void CaughtNaughtyTask()
    {
        currentCatTask.CaughtNaughtyTask();
        EventCameraController.Instance.ResetEvent();
        foreach (HumanManager human in humans)
        {
            if (human.currentRoom == currentCatRoom) human.CaughtNaughtyTask();
        }
    }

    public void HumanWitness()
    {
        foreach (HumanManager human in humans)
        {
            if (human.currentRoom == currentCatRoom) human.WitnessTask();
        }
    }

    public int HumansInCatRoom()
    {
        int cnt = 0;
        foreach (HumanManager human in humans)
        {
            if (human.currentRoom == currentCatRoom) cnt++;
        }
        return cnt;
    }

    public void UpdateCurrentRoom(RoomType catRoom)
    {
        currentCatRoom = catRoom;
        foreach (Room room in rooms)
        {
            RoomType roomType = room.GetRoomType();
            if (roomType == catRoom) room.SetVisible(true);
            else room.SetVisible(GetRoomVisibility(catRoom, roomType));
        }
    }

    private bool GetRoomVisibility(RoomType catRoom, RoomType targetRoom)
    {
        switch (catRoom)
        {
            case RoomType.Living:
                if (targetRoom == RoomType.Kitchen || targetRoom == RoomType.Hallway) return true;
                else return false;
            case RoomType.Kitchen:
                if (targetRoom == RoomType.Living) return true;
                else return false;
            case RoomType.Laundry:
                if (targetRoom == RoomType.Hallway) return true;
                else return false;
            case RoomType.Toilet:
                if (targetRoom == RoomType.Hallway) return true;
                else return false;
            case RoomType.Kid1:
                if (targetRoom == RoomType.Hallway) return true;
                else return false;
            case RoomType.Kid2:
                if (targetRoom == RoomType.Hallway) return true;
                else return false;
            case RoomType.Parent:
                if (targetRoom == RoomType.Hallway || targetRoom == RoomType.Storage) return true;
                else return false;
            case RoomType.Storage:
                if (targetRoom == RoomType.Parent) return true;
                else return false;
            case RoomType.Hallway:
                if (targetRoom == RoomType.Parent || targetRoom == RoomType.Kid1 || targetRoom == RoomType.Kid2 || targetRoom == RoomType.Toilet || targetRoom == RoomType.Laundry || targetRoom == RoomType.Living) return true;
                else return false;

            default: return false;
        }
    }
}

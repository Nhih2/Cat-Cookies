using System;
using DG.Tweening;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private RoomType roomType;
    [SerializeField] private SpriteRenderer Shadow;
    public bool isLocked = true, isLockedHardcore = false;
    private bool baseLocked;

    [SerializeField] private Switchable switchable;
    public event Action<RoomType> OnSwitch;

    void Start()
    {
        baseLocked = isLocked;
        if (switchable)
            switchable.OnActivate += () =>
            {
                isLocked = !isLocked;
                OnSwitch?.Invoke(RoomManager.Instance.currentCatRoom);
            };
        GameManager.Instance.OnDayStart += DayStart;
    }

    private void DayStart()
    {
        isLocked = baseLocked;
        if (isLockedHardcore && GameManager.Instance.isHardcore) isLocked = false;
    }


    public RoomType GetRoomType() => roomType;
    public void SetVisible(bool isVisible)
    {
        float targetAlpha = isVisible ? 0f : 1f;
        if (isLocked && roomType != RoomManager.Instance.currentCatRoom) targetAlpha = 1;

        Shadow.DOKill();
        Shadow.DOFade(targetAlpha, GameSetting.FADE_DURATION);
    }
}

public enum RoomType
{
    Living, Kitchen, Laundry, Toilet, Kid1, Kid2, Parent, Storage, Hallway, None
}
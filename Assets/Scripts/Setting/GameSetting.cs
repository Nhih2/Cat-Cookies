using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GameSetting
{
    public static string INTERACTABLE_LAYER = "Interactable", ROOM_LAYER = "Room";
    public static float FADE_DURATION = 0.25f;
    public static Dictionary<string, int> TaskChainDictionary = new();
    public static string FormatFloat(this float value)
    {
        return value.ToString("0.##");
    }
    public static int GetTotalMinutes(this TimeCapsule time)
    {
        return time.hour * 60 + time.minute;
    }
    public static void SetTransparency(this SpriteRenderer renderer, float a)
    {
        renderer.color = new(renderer.color.r, renderer.color.g, renderer.color.b, a);
    }
    public static void SetTransparency(this Image renderer, float a)
    {
        renderer.color = new(renderer.color.r, renderer.color.g, renderer.color.b, a);
    }
}

public enum TaskType
{
    TapAndWait, Hold, RepeatedTap
}

public enum TaskMode
{
    Single, Chain
}

public class TaskInfo
{

}